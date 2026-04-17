using System.Collections.Generic;

public static class RomajiDictionary {
    public static readonly Dictionary<string, string[]> Map = new() {
        // あ行
        { "あ", new[]{ "a" } }, { "い", new[]{ "i" } }, { "う", new[]{ "u" } }, { "え", new[]{ "e" } }, { "お", new[]{ "o" } },

        // か行
        { "か", new[]{ "ka","ca" } }, { "き", new[]{ "ki" } }, { "く", new[]{ "ku","cu","qu" } }, { "け", new[]{ "ke" } }, { "こ", new[]{ "ko","co" } },
        // が行
        { "が", new[]{ "ga" } }, { "ぎ", new[]{ "gi" } }, { "ぐ", new[]{ "gu" } }, { "げ", new[]{ "ge" } }, { "ご", new[]{ "go" } },

        // さ行
        { "さ", new[]{ "sa" } }, { "し", new[]{ "shi","si","ci" } }, { "す", new[]{ "su" } }, { "せ", new[]{ "se","ce" } }, { "そ", new[]{ "so" } },
        // ざ行
        { "ざ", new[]{ "za" } }, { "じ", new[]{ "zi","ji" } }, { "ず", new[]{ "zu" } }, { "ぜ", new[]{ "ze" } }, { "ぞ", new[]{ "zo" } },

        // た行
        { "た", new[]{ "ta" } }, { "ち", new[]{ "chi","ti" } }, { "つ", new[]{ "tsu","tu" } }, { "て", new[]{ "te" } }, { "と", new[]{ "to" } },
        // だ行
        { "だ", new[]{ "da" } }, { "ぢ", new[]{ "di" } }, { "づ", new[]{ "du" } }, { "で", new[]{ "de" } }, { "ど", new[]{ "do" } },

        // な行
        { "な", new[]{ "na" } }, { "に", new[]{ "ni" } }, { "ぬ", new[]{ "nu" } }, { "ね", new[]{ "ne" } }, { "の", new[]{ "no" } },

        // は行
        { "は", new[]{ "ha" } }, { "ひ", new[]{ "hi" } }, { "ふ", new[]{ "fu","hu" } }, { "へ", new[]{ "he" } }, { "ほ", new[]{ "ho" } },
        // ば行
        { "ば", new[]{ "ba" } }, { "び", new[]{ "bi" } }, { "ぶ", new[]{ "bu" } }, { "べ", new[]{ "be" } }, { "ぼ", new[]{ "bo" } },
        // ぱ行
        { "ぱ", new[]{ "pa" } }, { "ぴ", new[]{ "pi" } }, { "ぷ", new[]{ "pu" } }, { "ぺ", new[]{ "pe" } }, { "ぽ", new[]{ "po" } },

        // ま行
        { "ま", new[]{ "ma" } }, { "み", new[]{ "mi" } }, { "む", new[]{ "mu" } }, { "め", new[]{ "me" } }, { "も", new[]{ "mo" } },

        // や行
        { "や", new[]{ "ya" } }, { "ゆ", new[]{ "yu" } }, { "よ", new[]{ "yo" } },

        // ら行
        { "ら", new[]{ "ra" } }, { "り", new[]{ "ri" } }, { "る", new[]{ "ru" } }, { "れ", new[]{ "re" } }, { "ろ", new[]{ "ro" } },

        // わ行
        { "わ", new[]{ "wa" } }, { "を", new[]{ "wo","o" } },

        // ん
        { "ん", new[]{ "n","nn","xn" } },

        // 拗音
        { "きゃ", new[]{ "kya","kixya","kilya" } }, { "きゅ", new[]{ "kyu","kixyu","kilyu" } }, { "きょ", new[]{ "kyo","kixyo","kilyo" } },
        { "しゃ", new[]{ "sha","sya","cixya","cilya","sixya","shixya","silya","shilya" } }, { "しゅ", new[]{ "shu","syu","cixyu","cilyu","sixyu","shixyu","silyu","shilyu" } }, { "しょ", new[]{ "sho","syo","cixyo","cilyo","sixyo","shixyo","silyo","shilyo" } },
        { "ちゃ", new[]{ "cha","tya","cya","tixya", "chixya" } }, { "ちゅ", new[]{ "chu","tyu","cyu","tixyu", "chixyu" } }, { "ちょ", new[]{ "cho","tyo","cyo","tixyo", "chixyo" } },
    };
}