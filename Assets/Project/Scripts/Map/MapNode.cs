using UnityEngine;
using System.Collections.Generic;

public class MapNode {
    public int Id;
    public MapType Type;
    public int AreaIndex;  // 階層内の区画位置
    public Vector2 Position;   // マップ上での表示座標
    public List<MapNode> ConnectedNodes = new();  // 接続先ノード
    public List<BattleModifierBase> BattleModifiers { get; set; }  // ハプニング用
    public bool IsVisited; // 到達済みか
    public bool IsHappening => BattleModifiers != null;

    // ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭  DEBUG  ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭
    public override string ToString() {
        return $"Node[{Id}] " + $"Area: {AreaIndex} " + $"Type: {Type}";
    }
}
