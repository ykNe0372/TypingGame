using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private EnemyDataBase _enemyDataBase;
    [SerializeField] private EnemyFactory _enemyFactory;
    [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private ShopSystem _shopSystem;
    [SerializeField] private RewardSelectionUI _rewardSelectionUI;
    [SerializeField] private MapGenerator _mapGenerator;

    public static RunManager Instance;

    public MapData MapData { get; private set; }
    public MapNavigator Navigator { get; private set; }

    private void Start() {
        _rewardSelectionUI.OnRewardClosed += HandleRewardClosed;
        MapData mapData = _mapGenerator.Generate();
        StartRun(mapData);
    }

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void StartRun(MapData mapData) {
        MapData = mapData;
        Navigator = new MapNavigator(mapData);
        MapNode startNode = mapData.CurrentNode;
        EnterNode(startNode);
    }

    public void EnterNode(MapNode node) {
        Navigator.MoveTo(node);
        Debug.Log($"Enter Node: {node}");

        switch (node.Type) {
            case NodeType.Battle:
                StartBattle(node);
                break;
            case NodeType.Shop:
                OpenShop(node);
                break;
            case NodeType.Medical:
                OpenMedical(node);
                break;
            case NodeType.Boss:
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

        _combatSystem.BeginBattle(_player, enemies);
    }

    private void OpenShop(MapNode node) {
        Debug.Log("Open Shop");
        GameStateManager.Instance.ChangeState(GameState.Shop);
        _shopSystem.EnterShop(node);
        // GameStateManager.Instance.ChangeState(GameState.MapSelect);  // 仮実装、即 Map に戻す
    }

    private void OpenMedical(MapNode node) {
        Debug.Log("Open Medical");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);  // 仮実装、即 Map に戻す
    }

    private void StartBossBattle(MapNode node) {
        Debug.Log("Start Boss Battle");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);  // 仮実装、即 Map に戻す
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
        if (node.Type == NodeType.Boss) {
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

    private void HandleRewardClosed() {
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }
}