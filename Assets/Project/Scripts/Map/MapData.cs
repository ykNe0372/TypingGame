using System.Collections.Generic;

[System.Serializable]
public class MapData {
    public List<MapNode> Nodes = new();
    public MapNode StartNode;
    public MapNode CurrentNode;
}