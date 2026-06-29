using UnityEngine;
using System.Text;

public class DebugMapViewer : MonoBehaviour {
    [SerializeField] private MapGenerator _mapGenerator;

    public void PrintMap() {
        MapData mapData = RunManager.Instance.MapData;

        StringBuilder builder = new();
        builder.AppendLine("===== MAP START =====");

        foreach (var node in mapData.Nodes) {
            builder.AppendLine($"{node}");
            foreach (var connected in node.ConnectedNodes) {
                builder.AppendLine($" -> {connected}");
            }
        }

        builder.AppendLine("===== MAP END =====");
        Debug.Log(builder.ToString());
    }
}