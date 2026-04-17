using System.Collections.Generic;
using System.Linq;

public static class RomajiConverter {
    public static List<string> Convert(string kana) {
        var tokens = KanaParser.Parse(kana);

        // 各トークンの候補を取得
        var lists = tokens
            .Select(t => RomajiDictionary.Map.ContainsKey(t)
                ? RomajiDictionary.Map[t]
                : new[] { t }) // 未定義はそのまま
            .ToList();

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