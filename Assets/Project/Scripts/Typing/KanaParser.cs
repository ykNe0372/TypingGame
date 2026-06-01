using System.Collections.Generic;

// かなを入力単位に分解
public static class KanaParser {
    public static List<string> Parse(string kana) {
        var result = new List<string>();

        for (int i=0; i<kana.Length; ++i) {
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