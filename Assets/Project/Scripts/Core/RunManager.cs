using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour {
    [Header("プレイヤー")]
    [SerializeField] private Character _player;
    [Header("敵データ")]
    [SerializeField] private EnemyDataBase _enemyDataBase;
    [SerializeField] private EnemyFactory _enemyFactory;
    [Header("エリア処理")]
    [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private ItemShopSystem _itemShopSystem;
    [SerializeField] private RelicShopSystem _relicShopSystem;
    [SerializeField] private RestSystem _restSystem;
    [SerializeField] private BossAreaSystem _bossAreaSystem;
    [Header("報酬処理")]
    [SerializeField] private RewardSystem _rewardSystem;
    [SerializeField] private RewardSelectionUI _rewardSelectionUI;
    [Header("マップ生成機")]
    [SerializeField] private MapGenerator _mapGenerator;

    private int _currentSection = 1;    // 何区画目か（スタート〜ボスで1区画）
    private MapData _nextMap;  // 次の区画
    private MapSelectionMode _selectionMode;

    public static RunManager Instance;

    public MapData MapData { get; private set; }
    public MapNavigator Navigator { get; private set; }
    public int CurrentSection => _currentSection;
    public MapSelectionMode SelectionMode => _selectionMode;

    private void Start() {
        _combatSystem.OnBattleVictory += HandleBattleVictory;
        _combatSystem.OnBattleDefeat += HandleBattleDefeat;
        _rewardSelectionUI.OnRewardClosed += HandleRewardClosed;

        MapData mapData = _mapGenerator.Generate(_currentSection);
        StartRun(mapData);
    }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartRun(MapData mapData) {
        MapData = mapData;
        Navigator = new MapNavigator(mapData);

        MapNode startNode = mapData.StartNodes[0];
        Navigator.SelectStartNode(startNode);
        EnterNode(startNode);
        // if (_currentSection == 1) EnterNode(mapData.StartNodes[0]);
        // else ShowStartNodeSelection(mapData.StartNodes);
    }

    public void EnterNode(MapNode node) {
        Navigator.MoveTo(node);
        Debug.Log($"Enter Node: {node}");

        switch (node.Type) {
            case MapType.Battle:
                StartBattle(node);
                break;
            case MapType.ItemShop:
                OpenShop();
                break;
            case MapType.RelicShop:
                OpenRelicShop();
                break;
            case MapType.Rest:
                OpenRest();
                break;
            case MapType.Boss:
                StartBossBattle(node);
                break;
        }
    }

    private void StartBattle(MapNode node) {
        Debug.Log("Start Battle");
        GameStateManager.Instance.ChangeState(GameState.Battle);
        List<Character> enemies = CreateEnemies(node);

        List<BattleModifierBase> modifiers = new();
        if (node.BattleModifiers != null) 
            foreach (var modifier in node.BattleModifiers) modifiers.Add(modifier);
        _combatSystem.SetBattleModifiers(modifiers);
        Debug.Log($"[Happening] {node.IsHappening}");

        _combatSystem.BeginBattle(_player, enemies, BattleType.Normal);
    }

    private void OpenShop() {
        Debug.Log("Open Shop");
        GameStateManager.Instance.ChangeState(GameState.ItemShop);
        _itemShopSystem.EnterShop(_player);
    }

    private void OpenRelicShop() {
        Debug.Log("Open RelicShop");
        GameStateManager.Instance.ChangeState(GameState.RelicShop);
        _relicShopSystem.EnterRelicShop(_player);
    }

    private void OpenRest() {
        Debug.Log("Open Rest");
        GameStateManager.Instance.ChangeState(GameState.Rest);
        _restSystem.EnterRest(_player);
    }

    private void StartBossBattle(MapNode node) {
        Debug.Log("Start Boss Battle");
        GameStateManager.Instance.ChangeState(GameState.Battle);
        List<Character> boss = CreateEnemies(node);

        _bossAreaSystem.EnterBossArea(_player, boss);
    }

    private List<Character> CreateEnemies(MapNode node) {
        List<Character> enemies = new();
        int floorGroup = GetFloorGroup(node.FloorIndex);
        Debug.Log($"FloorGroup: {floorGroup}");
        EnemyDataBase.EnemyFloorTable floorSet = _enemyDataBase.GetFloorSet(floorGroup);

        if (floorSet == null) {
            Debug.LogError("FloorEnemySet Missing");
            return enemies;
        }

        // ボス戦
        if (node.Type == MapType.Boss) {
            EnemyData bossData = GetRandomEnemy(floorSet.BossEnemy);
            Character boss = _enemyFactory.CreateEnemy(bossData, 1);
            enemies.Add(boss);
            return enemies;
        }

        // 通常戦
        int enemyCount = Random.Range(2, 4);
        for (int i=0; i<enemyCount; ++i) {
            EnemyData enemyData = GetRandomEnemy(floorSet.NormalEnemies);
            Character enemy = _enemyFactory.CreateEnemy(enemyData, i);
            enemies.Add(enemy);
        }
        return enemies;
    }

    private EnemyData GetRandomEnemy(List<EnemySpawnEntry> entries) {
        int totalWeight = 0;
        foreach (var entry in entries) totalWeight += entry.SpawnWeight;

        int random = Random.Range(0, totalWeight);
        int current = 0;
        foreach (var entry in entries) {
            current += entry.SpawnWeight;
            if (random < current) return entry.EnemyData;
        }

        return entries[0].EnemyData;
    }

    // 仮実装
    private int GetFloorGroup(int floor) {
        return floor / 5;
    }

    private void HandleBattleVictory(BattleType battleType) {
        switch(battleType) {
            case BattleType.Normal:
                Debug.Log("[Battle] Victory");
                _rewardSystem.ShowReward(_player);
                break;
            case BattleType.Boss:
                Debug.Log("[Boss] Victory");
                _rewardSystem.ShowBossReward(_player);
                break;
        }

        // TODO: 勝利演出・リザルトUIなど
    }

    private void HandleBattleDefeat() {
        Debug.Log("[Battle] GAME OVER");
        // TODO: ゲームオーバーUI・BGM停止など
    }

    public void GenerateNextSection() {
        ++_currentSection;
        _nextMap = _mapGenerator.Generate(_currentSection);
        _selectionMode = MapSelectionMode.NextSection;

        Debug.Log($"Section {_currentSection} Generated");
    }

    public IReadOnlyList<MapNode> GetCurrentSelectableNodes() {
        return _selectionMode switch {
            MapSelectionMode.NextSection => _nextMap.StartNodes,
            _ => Navigator.GetSelectableNodes(),
        };

    }

    public void SelectNode(int index) {
        switch (_selectionMode) {
            case MapSelectionMode.Normal:
                SelectNormalNode(index);
                break;
            case MapSelectionMode.NextSection:
                SelectNextSectionNode(index);
                break;
        }
    }

    private void SelectNormalNode(int index) {
        var selectable = Navigator.GetSelectableNodes();
        MapNode node = selectable[index];
        EnterNode(node);
    }

    private void SelectNextSectionNode(int index) {
        MapNode node = _nextMap.StartNodes[index];

        _nextMap.CurrentNode = node;
        node.IsVisited = true;

        MapData = _nextMap;
        Navigator = new MapNavigator(_nextMap);
        _selectionMode = MapSelectionMode.Normal;
        EnterNode(node);
    }

    public IReadOnlyList<MapNode> GetStartNodes() {
        return _nextMap.StartNodes;
    }

    public void EnterNextFloor(MapNode node) {
        _nextMap.CurrentNode = node;
        node.IsVisited = true;

        EnterNode(node);
    }

    private void HandleRewardClosed() {
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    // ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭  DEBUG MODE  ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭

    public void Debug_MoveTo(MapType type) {
        MapNode node = new() {
            Type = type
        };

        switch(type) {
            case MapType.Battle:
                StartBattle(node);
                break;
            case MapType.ItemShop:
                OpenShop();
                break;
            case MapType.RelicShop:
                OpenRelicShop();
                break;
            case MapType.Rest:
                OpenRest();
                break;
            case MapType.Boss:
                StartBossBattle(node);
                break;
        }
    }

    public void Debug_CompleteBattle() {
        _player.OnBattleEnd();
    }
}