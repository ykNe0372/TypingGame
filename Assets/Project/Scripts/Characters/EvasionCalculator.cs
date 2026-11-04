using UnityEngine;

public static class EvasionCalculator {
    public static float Calculate(Character character) {
        if (character == null || character.EvasionSettings == null) return 0f;

        float baseRate = Mathf.Clamp(character.GetFinalStatus(StatusType.EvasionRate), 0f, 100f);
        float k = character.EvasionSettings.curveK;
        float maxRate = character.EvasionSettings.maxEvasionRate;
        
        if (Mathf.Approximately(k, 0f)) return maxRate * baseRate / 100f;
        return maxRate * (1f - Mathf.Exp(-k * baseRate / 100)) / (1f - Mathf.Exp(-k));
    }

    public static bool Roll(Character character) {
        if (character == null || character.EvasionSettings == null) return false;

        float evasionRate = Calculate(character);
        float bonus = character.GetTemporaryStatusBonus(StatusType.EvasionRate);
        float maxRate = character.EvasionSettings.maxEvasionRate;

        float finalRate = Mathf.Min(evasionRate + bonus, maxRate);
        // Debug.Log($"[EvasionCalculator] evasionRate: {evasionRate}, bonus: {bonus}, finalRate: {finalRate}");
        return Random.value < finalRate / 100f;
    }
}