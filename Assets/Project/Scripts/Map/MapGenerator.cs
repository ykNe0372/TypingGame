using UnityEngine;
using UnityEngine.Serialization;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    [Header("1階層あたりの区画数")]
    [FormerlySerializedAs("_floorCount")]
    [SerializeField] private int _areaCount;
    [Header("候補数")]
    [FormerlySerializedAs("_minNodesPerFloor")]
    [SerializeField] private int _minNodesPerArea;
    [FormerlySerializedAs("_maxNodesPerFloor")]
    [SerializeField] private int _maxNodesPerArea;
    [Header("出現確率")]
    [SerializeField] private float _battleRate;
    [SerializeField] private float _itemShopRate;
    [SerializeField] private float _relicShopRate;
    [SerializeField] private float _happeningRate;
    
    [SerializeField, Header("ハプニング効果一覧")] private List<BattleModifierBase> _allBattleModifiers = new();

    private int _nodeId;

    public MapData Generate(int hierarchyNumber) {
        MapData mapData = new();
        List<List<MapNode>> areas = new();

        // 階層内の各区画を生成
        for (int x=0; x<_areaCount; ++x) {
            int nodeCount;
            if (x == 0) nodeCount = hierarchyNumber == 1 ? 1 : Random.Range(_minNodesPerArea, _maxNodesPerArea + 1);
            else if (x == _areaCount - 1) nodeCount = 1;   // 最終区画はボス戦用に必ず1つ
            else nodeCount = Random.Range(_minNodesPerArea, _maxNodesPerArea + 1);

            List<MapNode> area = new();
            for (int y=0; y<nodeCount; ++y) {
                MapNode node = new() {
                    Id = ++_nodeId,
                    AreaIndex = x,
                    Position = new Vector2(x, y),
                    Type = GetRandomNodeType(x, hierarchyNumber)
                };

                area.Add(node);
                mapData.Nodes.Add(node);
            }

            areas.Add(area);
        }

        // ノード接続
        for (int x=0; x<areas.Count-1; ++x) {
            List<MapNode> currentArea = areas[x];
            List<MapNode> nextArea = areas[x+1];

            foreach (var node in currentArea) {
                AssignHappening(node);
                int connectionCount = Random.Range(1, Mathf.Min(2, nextArea.Count) + 1);
                List<MapNode> shuffled = new(nextArea);
                Shuffle(shuffled);
                for (int i=0; i<connectionCount; ++i) node.ConnectedNodes.Add(shuffled[i]);
            }
        }

        EnsureAllNodesConnected(areas);

        mapData.StartNodes = new(areas[0]);
        mapData.CurrentNode = null;  // null で生成→選択時に上書き
        return mapData;
    }

    // 後方向保証
    private void EnsureAllNodesConnected(List<List<MapNode>> areas) {
        for (int x=1; x<areas.Count; ++x) {
            List<MapNode> area = areas[x];

            foreach (var node in area) {
                bool hasConnection = false;
                List<MapNode> previousArea = areas[x-1];

                foreach (var prev in previousArea) {
                    if (prev.ConnectedNodes.Contains(node)) {
                        hasConnection = true;
                        break;
                    }
                }

                if (!hasConnection) {
                    MapNode randomPrev = previousArea[Random.Range(0, previousArea.Count)];
                    randomPrev.ConnectedNodes.Add(node);
                }
            }
        }
    }

    // エリア生成
    private MapType GetRandomNodeType(int areaIndex, int hierarchyNumber) {
        if (areaIndex == 0 && hierarchyNumber == 1) return MapType.Battle;
        if (areaIndex == _areaCount - 1) return MapType.Boss;

        float random = Random.value;
        if (random < (_battleRate / 100f)) return MapType.Battle;
        if (random < (_battleRate / 100f + _itemShopRate / 100f)) return MapType.ItemShop;
        if (random < (_battleRate / 100f + _itemShopRate / 100f + _relicShopRate / 100)) return MapType.RelicShop;
        return MapType.Rest;
    }

    // ノードの種類を再抽選する（ボスノードはそのまま）
    public void RerollNode(MapNode node, int hierarchyNumber) {
        if (node == null || node.Type == MapType.Boss) return;

        node.Type = GetRandomNodeType(node.AreaIndex, hierarchyNumber);
        node.BattleModifiers = null;
        AssignHappening(node);
    }

    private void Shuffle<T>(List<T> list) {
        for (int i=0; i<list.Count; ++i) {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void AssignHappening(MapNode node) {
        if (node.Type != MapType.Battle) return;
        if (_allBattleModifiers.Count == 0) return;  // ハプニング未設定時のエラー防止
        if (Random.value > (_happeningRate / 100f)) return;

        int index = Random.Range(0, _allBattleModifiers.Count);
        node.BattleModifiers = new List<BattleModifierBase> { _allBattleModifiers[index] };
    }
}