using System.Collections.Generic;

public class MapNavigator {
    private MapData _mapData;
    
    public MapNode CurrentNode => _mapData.CurrentNode;
    public MapNavigator(MapData mapData) {
        _mapData = mapData;
        if (_mapData.CurrentNode != null) _mapData.CurrentNode.IsVisited = true;
    }

    // 開始地点候補
    public IReadOnlyList<MapNode> GetStartNodes() {
        return _mapData.StartNodes;
    }

    // 移動可能なノード一覧を取得
    public List<MapNode> GetSelectableNodes() {
        return CurrentNode.ConnectedNodes;
    }

    // 開始地点を選択した時の処理
    public void SelectStartNode(MapNode startNode) {
        if (!_mapData.StartNodes.Contains(startNode)) return;

        startNode.IsVisited = true;
        _mapData.CurrentNode = startNode;
    }

    // 指定ノードへ移動
    public bool MoveTo(MapNode nextNode) {
        if (!CurrentNode.ConnectedNodes.Contains(nextNode)) return false;

        nextNode.IsVisited = true;
        _mapData.CurrentNode = nextNode;
        return true;
    }
}