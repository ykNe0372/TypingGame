using System.Text;

public static class TypingInputNormalizer {
    public static char Normalize(char c) {
        string normalized = c.ToString().Normalize(NormalizationForm.FormKC);
        c = normalized[0];

        c = char.ToLowerInvariant(c);
        if (c == 'ー') c = '-';

        return c;
    }

    public static bool IsTyppingCharacter(char c) {
        return char.IsLetter(c) || c == '-';
    }
}