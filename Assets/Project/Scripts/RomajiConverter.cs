using System.Collections.Generic;
using System.Linq;

public static class RomajiConverter {
    public static List<string> Convert(string kana) {
        var tokens = KanaParser.Parse(kana);
        var lists = new List<List<string>>();

        for (int i=0; i<tokens.Count; ++i) {
            string token = tokens[i];
            if (token == "ん") {
                if (i == tokens.Count - 1) lists.Add(new List<string> { "n", "xn" });
                else {
                    string next = tokens[i+1];
                    if (RomajiDictionary.Map.TryGetValue(next, out var nextRomajis)) {
                        char head = nextRomajis[0][0];
                        if ("aiueoy".Contains(head)) lists.Add(new List<string> { "nn", "xn" });
                        else lists.Add(new List<string> {"n", "nn", "xn" });
                    } else lists.Add(new List<string> { "n" });
                }
            continue;
            }

            if (RomajiDictionary.Map.TryGetValue(token, out var romajis)) lists.Add(romajis.ToList());
            else lists.Add(new List<string> { token });
        }

        // 直積で全候補生成
        List<string> results = new() {""};
        foreach (var list in lists) {
            results = results
                .SelectMany(r => list.Select(s => r + s))
                .ToList();
        }
        return results;
    }
}