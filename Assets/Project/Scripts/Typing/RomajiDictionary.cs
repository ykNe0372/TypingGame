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
        { "うぁ", new[]{ "wha","uxa","ula" } }, { "うぃ", new[]{ "wi","whi","uxi","uli" } }, { "うぇ", new[]{ "we","whe","uxe","ule" } }, { "うぉ", new[]{ "who","uxo","ulo" } },

        { "きゃ", new[]{ "kya","kixya","kilya" } }, { "きゅ", new[]{ "kyu","kixyu","kilyu" } }, { "きょ", new[]{ "kyo","kixyo","kilyo" } },
        { "ぎゃ", new[]{ "gya","gixya","gilya" } }, { "ぎゅ", new[]{ "gyu","gixyu","gilyu" } }, { "ぎょ", new[]{ "gyo","gixyo","gilyo" } },
        { "くゃ", new[]{ "qya","kuxya","cuxya","kulya","culya" } }, { "くゅ", new[]{ "qyu","kuxyu","cuxyu","kulyu","culyu" } }, { "くょ", new[]{ "qyo","kuxyo","cuxyo","kulyo","culyo" } },
        { "くぁ", new[]{ "qa","qwa","kuxa","cuxa","kula","cula" } }, { "くぇ", new[]{ "qe","qwe","kuxe","cuxe","kule","cule","kuxye","cuxye","quxye" } }, { "くぉ", new[]{ "qo","qwo","kuxo","cuxo","kulo","culo" } },

        { "しゃ", new[]{ "sha","sya","cixya","cilya","sixya","shixya","silya","shilya" } }, { "しゅ", new[]{ "shu","syu","cixyu","cilyu","sixyu","shixyu","silyu","shilyu" } }, { "しぇ", new[]{ "she","sye","cixe","cile","sixe","sile","shixe","shile" }}, { "しょ", new[]{ "sho","syo","cixyo","cilyo","sixyo","shixyo","silyo","shilyo" } },
        { "じゃ", new[]{ "ja","zya","jixya","jilya","zixya","zilya" } }, { "じゅ", new[]{ "ju","zyu","jixyu","jilyu","zixyu","zilyu" } }, { "じょ", new[]{ "jo","zyo","jixyo","jilyo","zixyo","zilyo" } },

        { "ちゃ", new[]{ "cha","tya","cya","tixya","chixya","tilya","chilya" } }, { "ちぃ", new[]{ "tyi","cyi","tixi","tili","chixi","chili" } }, { "ちゅ", new[]{ "chu","tyu","cyu","tixyu","chixyu","tilya","chilya" } }, { "ちぇ", new[]{ "tye","cye","che","tixe","tile","chixe","chile" } }, { "ちょ", new[]{ "cho","tyo","cyo","tixyo", "chixyo","tilya","chilya" } },
        { "つぁ", new[]{ "tsa","tuxa","tsuxa","tula","tsula" } }, { "つぃ", new[]{ "tsi","tuxi","tsuxi","tuli","tsuli" } }, { "つぇ", new[]{ "tse","tuxe","tsuxe","tule","tsule" } }, { "つぉ", new[]{ "tso","tuxo","tsuxo","tulo","tsulo" } },
        { "てゃ", new[]{ "tha","texya","telya" } }, { "てぃ", new[]{ "thi","texi","teli" } }, { "てゅ", new[]{ "thu","texyu","telyu" } }, { "てぇ", new[]{ "the","texe","tele" } }, { "てょ", new[]{ "tho","texyo","telyo" } },
        { "でゃ", new[]{ "dha","dexya","delya" } }, { "でぃ", new[]{ "dhi","dexi","deli" } }, { "でゅ", new[]{ "dhu","dexyu","delyu" } }, { "でぇ", new[]{ "dhe","dexe","dele" } }, { "でょ", new[]{ "dho","dexyo","delyo" } },
        { "とぁ", new[]{ "twa","toxa","tola" } }, { "とぃ", new[]{ "twi","toxi","toli" } }, { "とぅ", new[]{ "twu","toxu","tolu" } }, { "とぇ", new[]{ "twe","toxe","tole" } }, { "とぉ", new[]{ "two","toxo","tolo" } },
        { "どぁ", new[]{ "dwa","doxa","dola" } }, { "どぃ", new[]{ "dwi","doxi","doli" } }, { "どぅ", new[]{ "dwu","doxu","dolu" } }, { "どぇ", new[]{ "dwe","doxe","dole" } }, { "どぉ", new[]{ "dwo","doxo","dolo" } },

        { "にゃ", new[]{ "nya","nixya","nilya" } }, { "にゅ", new[]{ "nyu","nixyu","nilyu" } }, { "にょ", new[]{ "nyo","nixyo","nilyo" } }, 

        { "ひゃ", new[]{ "hya","hixya","hilya" } }, { "ひゅ", new[]{ "hyu","hixyu","hilyu" } }, { "ひょ", new[]{ "hyo","hixyo","hilyo" } },
        { "びゃ", new[]{ "bya","bixya","bilya" } }, { "びゅ", new[]{ "byu","bixyu","bilyu" } }, { "びょ", new[]{ "byo","bixyo","bilyo" } },
        { "ぴゃ", new[]{ "pya","pixya","pilya" } }, { "ぴゅ", new[]{ "pyu","pixyu","pilyu" } }, { "ぴょ", new[]{ "pyo","pixyo","pilyo" } },
        { "ふぁ", new[]{ "fa","fwa","huxa","fuxa","hula","fula" } }, { "ふぃ", new[]{ "fi","fwi","huxi","fuxi","huli","fuli" } }, { "ふぇ", new[]{ "fe","fwe","huxe","fuxe","hule","fule" } }, { "ふぉ", new[]{ "fo","fwo","huxo","fuxo","hulo","fulo" } },

        { "みゃ", new[]{ "mya","mixya","milya" } }, { "みゅ", new[]{ "myu","mixyu","milyu" } }, { "みょ", new[]{ "myo","mixyo","milyo" } },
        
        { "りゃ", new[]{ "rya","rixya","rilya" } }, { "りゅ", new[]{ "ryu","rixyu","rilyu" } }, { "りょ", new[]{ "ryo","rixyo","rilyo" } },

        // 記号
        { "ー", new[]{ "-" } }, { "！", new[]{ "!" } }, { "？", new[]{ "?" } }, { "。", new[]{ "." } }, { "、", new[]{ "," } },
    };
}