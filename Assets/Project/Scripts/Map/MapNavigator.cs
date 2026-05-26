using System.Collections.Generic;

public class MapNavigator {
    private MapData _mapData;
    
    public MapNode CurrentNode => _mapData.CurrentNode;
    public MapNavigator(MapData mapData) {
        _mapData = mapData;
    }

    // 移動可能なノード一覧を取得
    public List<MapNode> GetSelectableNodes() {
        return CurrentNode.ConnectedNodes;
    }

    // 指定ノードへ移動
    public bool MoveTo(MapNode nextNode) {
        if (!CurrentNode.ConnectedNodes.Contains(nextNode)) return false;

        nextNode.IsVisited = true;
        _mapData.CurrentNode = nextNode;
        return true;
    }
}