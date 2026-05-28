using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour {
    [SerializeField] private int _floorCount = 5;
    [SerializeField] private int _minNodesPerFloor = 2;
    [SerializeField] private int _maxNodesPerFloor = 4;

    private int _nodeId;

    public MapData Generate() {
        MapData mapData = new();
        List<List<MapNode>> floors = new();

        // 各階層生成
        for (int x=0; x<_floorCount; ++x) {
            int nodeCount = (x == 0 || x == _floorCount - 1) ? 1 : Random.Range(_minNodesPerFloor, _maxNodesPerFloor + 1);
            List<MapNode> floor = new();

            for (int y=0; y<nodeCount; ++y) {
                MapNode node = new() {
                    Id = ++_nodeId,
                    FloorIndex = x,
                    Position = new Vector2(x, y),
                    Type = GetRandomNodeType(x)
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
                int connectionCount = Random.Range(1, Mathf.Min(2, nextFloor.Count) + 1);
                List<MapNode> shuffled = new(nextFloor);
                Shuffle(shuffled);
                for (int i=0; i<connectionCount; ++i) node.ConnectedNodes.Add(shuffled[i]);
            }
        }

        EnsureAllNodesConnected(floors);

        mapData.StartNode = floors[0][0];
        mapData.CurrentNode = mapData.StartNode;
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

    // エリア生成（仮）
    private NodeType GetRandomNodeType(int floorIndex) {
        if (floorIndex == 0) return NodeType.Battle;
        if (floorIndex == _floorCount - 1) return NodeType.Boss;  // 最終層はボス固定

        float random = Random.value;
        if (random < 0.65f) return NodeType.Battle;
        if (random < 0.85f) return NodeType.Shop;
        return NodeType.Medical;
    }

    private void Shuffle<T>(List<T> list) {
        for (int i=0; i<list.Count; ++i) {
            int randomIndex = Random.Range(i, list.Count);
            (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
        }
    }
}