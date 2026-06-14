using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Buffs/Buff")]
public class BuffData : ScriptableObject {
    [SerializeField] private string _buffName;
    [SerializeField] private int _durationBattleCount;
    [SerializeField] private List<BuffModifier> _modifiers = new();

    public string BuffName => _buffName;
    public int DurationBattleCount => _durationBattleCount;
    public IReadOnlyList<BuffModifier> Modifiers => _modifiers;
}