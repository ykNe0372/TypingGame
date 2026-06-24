using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    [Header("階層数")]
    [SerializeField] private int _floorCount;
    [Header("候補数")]
    [SerializeField] private int _minNodesPerFloor;
    [SerializeField] private int _maxNodesPerFloor;
    [Header("出現確率")]
    [SerializeField] private float _battleRate;
    [SerializeField] private float _itemShopRate;
    [SerializeField] private float _relicShopRate;
    [SerializeField] private float _happeningRate;
    
    [SerializeField, Header("ハプニング効果一覧")] private List<BattleModifierBase> _allBattleModifiers = new();

    private int _nodeId;

    public MapData Generate(int sectionNumber) {
        MapData mapData = new();
        List<List<MapNode>> floors = new();

        // 各階層生成
        for (int x=0; x<_floorCount; ++x) {
            int nodeCount;
            if (x == 0) nodeCount = sectionNumber == 1 ? 1 : Random.Range(_minNodesPerFloor, _maxNodesPerFloor + 1);  // 第1区画 or 第2区画以降 で分類
            else if (x == _floorCount - 1) nodeCount = 1;   // 最後層はボス戦用に必ず1つ
            else nodeCount = Random.Range(_minNodesPerFloor, _maxNodesPerFloor + 1);

            List<MapNode> floor = new();
            for (int y=0; y<nodeCount; ++y) {
                MapNode node = new() {
                    Id = ++_nodeId,
                    FloorIndex = x,
                    Position = new Vector2(x, y),
                    Type = GetRandomNodeType(x, sectionNumber)
                };

                floor.Add(node);
                mapData.Nodes.Add(node);
            }

            floors.Add(floor);
        }

        // ノード接続
        for (int x=0; x<floors.Count-1; ++x) {
            List<MapNode> currentFloor = floors[x];
            List<MapNode> nextFloor = floors[x+1];

            foreach (var node in currentFloor) {
                AssignHappening(node);
                int connectionCount = Random.Range(1, Mathf.Min(2, nextFloor.Count) + 1);
                List<MapNode> shuffled = new(nextFloor);
                Shuffle(shuffled);
                for (int i=0; i<connectionCount; ++i) node.ConnectedNodes.Add(shuffled[i]);
            }
        }

        EnsureAllNodesConnected(floors);

        mapData.StartNodes = new(floors[0]);
        mapData.CurrentNode = null;  // null で生成→選択時に上書き
        return mapData;
    }

    // 後方向保証
    private void EnsureAllNodesConnected(List<List<MapNode>> floors) {
        for (int x=1; x<floors.Count; ++x) {
            List<MapNode> floor = floors[x];

            foreach (var node in floor) {
                bool hasConnection = false;
                List<MapNode> prevFloor = floors[x-1];

                foreach (var prev in prevFloor) {
                    if (prev.ConnectedNodes.Contains(node)) {
                        hasConnection = true;
                        break;
                    }
                }

                if (!hasConnection) {
                    MapNode randomPrev = prevFloor[Random.Range(0, prevFloor.Count)];
                    randomPrev.ConnectedNodes.Add(node);
                }
            }
        }
    }

    // エリア生成
    private MapType GetRandomNodeType(int floorIndex, int sectionNumber) {
        if (floorIndex == 0 && sectionNumber == 1) return MapType.Battle;  // 第1区画 かつ 第1層 -> 戦闘固定
        if (floorIndex == _floorCount - 1) return MapType.Boss;  // 最終層はボス固定

        float random = Random.value;
        if (random < (_battleRate / 100f)) return MapType.Battle;
        if (random < (_battleRate / 100f + _itemShopRate / 100f)) return MapType.ItemShop;
        if (random < (_battleRate / 100f + _itemShopRate / 100f + _relicShopRate / 100)) return MapType.RelicShop;
        return MapType.Rest;
    }

    private void Shuffle<T>(List<T> list) {
        for (int i=0; i<list.Count; ++i) {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }

    private void AssignHappening(MapNode node) {
        if (node.Type != MapType.Battle) return;
        if (Random.value > (_happeningRate / 100f)) return;

        int index = Random.Range(0, _allBattleModifiers.Count);
        node.BattleModifiers = new List<BattleModifierBase> { _allBattleModifiers[index] };
    }
}