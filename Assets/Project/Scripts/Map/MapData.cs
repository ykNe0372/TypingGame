using System.Collections.Generic;

public class MapData {
    public List<MapNode> Nodes = new();
    public List<MapNode> StartNodes = new();  // 開始地点一覧
    public MapNode CurrentNode;
}