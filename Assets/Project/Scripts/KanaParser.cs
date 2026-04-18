using System.Collections.Generic;
using System.Linq;

// かなを入力単位に分解
public static class KanaParser {
    public static List<string> Parse(string kana) {
        var result = new List<string>();

        for (int i=0; i<kana.Length; ++i) {
            if (kana[i] == 'っ') {      // 促音チェック
                if (i+1 < kana.Length) {
                    string next;
                    if (i+2 < kana.Length) {    // 2文字チェック（拗音）
                        string two = kana.Substring(i+1, 2);
                        if (RomajiDictionary.Map.ContainsKey(two)) next = two;
                        else next = kana[i+1].ToString();
                    } else next = kana[i+1].ToString();

                    if (RomajiDictionary.Map.TryGetValue(next, out var romajis)) {
                        var consonants = romajis.Select(r => r[0].ToString()).Distinct().ToArray();
                        result.Add(consonants[0]);
                    }
                }
                continue;
            }

            if (i+1 < kana.Length) {
                string two = kana.Substring(i, 2);
                if (RomajiDictionary.Map.ContainsKey(two)) {
                    result.Add(two);
                    ++i;
                    continue;
                }
            }
            result.Add(kana[i].ToString()); // 1文字
        }
        return result;
    }
}