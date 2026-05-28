using UnityEngine;
using System.Collections.Generic;

public enum NodeType {
    Battle,
    Shop,
    Medical,
    Boss
}

public class MapNode {
    public int Id;
    public NodeType Type;
    public int FloorIndex;  // 何層目にいるか
    public Vector2 Position;   // マップ上での表示座標
    public List<MapNode> ConnectedNodes = new();  // 接続先ノード
    public List<BattleModifierBase> BattleModifiers = new();  // ハプニング用
    public bool IsVisited; // 到達済みか

    // ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭  DEBUG  ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭
    public override string ToString() {
        return $"Node[{Id}] " + $"Floor: {FloorIndex} " + $"Type: {Type}";
    }
}
