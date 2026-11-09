using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/StE Modifier")]
public class StatusEffectModifierEffect : Effect {
    [SerializeField] private List<StatusEffectModifier> _modifiers = new();

    public override void GetStatusEffectModifiers(List<StatusEffectModifier> results) {
        results.AddRange(_modifiers);
    }

}