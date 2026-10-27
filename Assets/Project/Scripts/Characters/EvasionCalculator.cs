using UnityEngine;

public static class EvasionCalculator {
    public static float Calculate(Character character) {
        if (character == null || character.EvasionSettings == null) return 0f;

        float evasionRate = Mathf.Clamp(character.GetFinalStatus(StatusType.EvasionRate), 0f, 100f);
        float k = character.EvasionSettings.curveK;
        float maxRate = character.EvasionSettings.maxEvasionRate;

        return maxRate * (1f - Mathf.Exp(-k * evasionRate / 100)) / (1f - Mathf.Exp(-k));
    }

    public static bool Roll(Character character) {
        float evasionRate = Calculate(character);
        return Random.value < evasionRate / 100f;
    }
}