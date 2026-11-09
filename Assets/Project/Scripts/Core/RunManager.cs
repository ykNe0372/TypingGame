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
    [Header("マップ選択UI")]
    [SerializeField] private DestinationSelectionUI _destinationSelectionUI;

    private int _currentHierarchy = 1;    // ボス撃破までは「階層」とする
    private MapData _nextHierarchyMap;
    private MapSelectionMode _selectionMode;

    public static RunManager Instance;

    public MapData MapData { get; private set; }
    public MapNavigator Navigator { get; private set; }
    public int CurrentHierarchy => _currentHierarchy;
    public MapSelectionMode SelectionMode => _selectionMode;

    private void Start() {
        _combatSystem.OnBattleVictory += HandleBattleVictory;
        _combatSystem.OnBattleDefeat += HandleBattleDefeat;
        _rewardSystem.OnRewardFinished += HandleRewardClosed;
        SubscribeGameState();

        MapData mapData = _mapGenerator.Generate(_currentHierarchy);
        StartRun(mapData);
    }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private bool _isSubscribedState;

    // GameStateManager.Awake より先に OnEnable が呼ばれると購読できないため、Start でも再試行する
    private void OnEnable() {
        SubscribeGameState();
    }

    private void OnDisable() {
        if (!_isSubscribedState) return;
        if (GameStateManager.Instance != null) GameStateManager.Instance.OnStateChanged -= HandleGameStateChanged;
        _isSubscribedState = false;
    }

    private void SubscribeGameState() {
        if (_isSubscribedState || GameStateManager.Instance == null) return;
        GameStateManager.Instance.OnStateChanged += HandleGameStateChanged;
        _isSubscribedState = true;
    }

    public void StartRun(MapData mapData) {
        MapData = mapData;
        Navigator = new MapNavigator(mapData);

        MapNode startNode = mapData.StartNodes[0];
        Navigator.SelectStartNode(startNode);
        EnterNode(startNode);
        // if (_currentHierarchy == 1) EnterNode(mapData.StartNodes[0]);
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
        _itemShopSystem.EnterShop(_player, _rewardSelectionUI);
    }

    private void OpenRelicShop() {
        Debug.Log("Open RelicShop");
        GameStateManager.Instance.ChangeState(GameState.RelicShop);
        _relicShopSystem.EnterRelicShop(_player, _rewardSelectionUI);
    }

    private void OpenRest() {
        Debug.Log("Open Rest");
        GameStateManager.Instance.ChangeState(GameState.Rest);
        _restSystem.EnterRest(_player, _rewardSelectionUI);
    }

    private void StartBossBattle(MapNode node) {
        Debug.Log("Start Boss Battle");
        GameStateManager.Instance.ChangeState(GameState.Battle);
        List<Character> boss = CreateEnemies(node);

        _bossAreaSystem.EnterBossArea(_player, boss);
    }

    private List<Character> CreateEnemies(MapNode node) {
        List<Character> enemies = new();
        EnemyDataBase.EnemyHierarchyTable hierarchyTable = _enemyDataBase.GetHierarchyTable(_currentHierarchy);

        if (hierarchyTable == null) {
            Debug.LogError($"Enemy hierarchy table missing for hierarchy {_currentHierarchy}.");
            return enemies;
        }

        // ボス戦
        if (node.Type == MapType.Boss) {
            EnemyData bossData = GetRandomEnemy(hierarchyTable.BossEnemies);
            if (bossData == null) return enemies;
            Character boss = _enemyFactory.CreateEnemy(bossData, 1);
            enemies.Add(boss);
            return enemies;
        }

        // 通常戦
        int enemyCount = Random.Range(2, 4);
        for (int i=0; i<enemyCount; ++i) {
            EnemyData enemyData = GetRandomEnemy(hierarchyTable.NormalEnemies);
            if (enemyData == null) continue;
            Character enemy = _enemyFactory.CreateEnemy(enemyData, i);
            enemies.Add(enemy);
        }
        return enemies;
    }

    private EnemyData GetRandomEnemy(List<EnemySpawnEntry> entries) {
        if (entries == null || entries.Count == 0) {
            Debug.LogError("EnemySpawnEntry Missing");
            return null;
        }

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

    public void GenerateNextHierarchy() {
        ++_currentHierarchy;
        _nextHierarchyMap = _mapGenerator.Generate(_currentHierarchy);
        _selectionMode = MapSelectionMode.NextHierarchy;

        Debug.Log($"Hierarchy {_currentHierarchy} Generated");
    }

    public IReadOnlyList<MapNode> GetCurrentSelectableNodes() {
        return _selectionMode switch {
            MapSelectionMode.NextHierarchy => _nextHierarchyMap.StartNodes,
            _ => Navigator.GetSelectableNodes(),
        };

    }

    public void SelectNode(int index) {
        var selectable = GetCurrentSelectableNodes();
        if (index < 0 || index >= selectable.Count) return;

        _destinationSelectionUI.Hide();

        switch (_selectionMode) {
            case MapSelectionMode.Normal:
                SelectNormalNode(index);
                break;
            case MapSelectionMode.NextHierarchy:
                SelectNextHierarchyNode(index);
                break;
        }
    }

    private void SelectNormalNode(int index) {
        var selectable = Navigator.GetSelectableNodes();
        MapNode node = selectable[index];
        EnterNode(node);
    }

    private void SelectNextHierarchyNode(int index) {
        MapNode node = _nextHierarchyMap.StartNodes[index];

        _nextHierarchyMap.CurrentNode = node;
        node.IsVisited = true;

        MapData = _nextHierarchyMap;
        Navigator = new MapNavigator(_nextHierarchyMap);
        _selectionMode = MapSelectionMode.Normal;
        EnterNode(node);
    }

    // レリック効果: 行き先候補を再抽選する（ボスは対象外）
    public bool TryRerollDestinations() {
        if (!GameStateManager.Instance.IsState(GameState.MapSelect)) return false;

        var selectable = GetCurrentSelectableNodes();
        bool hasRerollable = false;
        foreach (var node in selectable) {
            if (node.Type != MapType.Boss) hasRerollable = true;
        }
        if (!hasRerollable) return false;
        if (!_player.Hooks.TryConsumeAreaReroll()) return false;

        foreach (var node in selectable) _mapGenerator.RerollNode(node, _currentHierarchy);
        _destinationSelectionUI.Show(selectable);

        Debug.Log($"[Map] Reroll Destinations (remaining: {_player.Hooks.GetRemainingAreaRerolls()})");
        return true;
    }

    public IReadOnlyList<MapNode> GetNextHierarchyStartNodes() {
        return _nextHierarchyMap.StartNodes;
    }

    public void EnterNextHierarchyNode(MapNode node) {
        _nextHierarchyMap.CurrentNode = node;
        node.IsVisited = true;

        EnterNode(node);
    }

    private void HandleRewardClosed() {
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    private void HandleGameStateChanged(GameState state) {
        if (state != GameState.MapSelect) return;
        _destinationSelectionUI.Show(GetCurrentSelectableNodes());
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