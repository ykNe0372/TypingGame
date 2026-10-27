using UnityEngine;

[CreateAssetMenu(fileName = "EvasionSettings", menuName = "Data/Character/Evasion Settings")]
public class EvasionSettings : ScriptableObject {
    [Header("回避率設定")]
    public float maxEvasionRate = 45f;

    [Min(0.01f)]
    public float curveK = 2f;
}