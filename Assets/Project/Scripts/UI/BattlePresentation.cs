using UnityEngine;
using System.Collections.Generic;

public class BattlePresentation : MonoBehaviour {
    // [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private DamagePopupManager _damagePopupManager;

    private readonly HashSet<Character> _registeredCharacters = new();

    public void Register(Character character) {
        if (!_registeredCharacters.Add(character)) return;  // 二重配線にならない用の安全装置

        character.OnDamaged += OnDamaged;
        // character.OnDead += OnDead;
    }

    public void Unregister(Character character) {
        character.OnDamaged -= OnDamaged;
    }

    private void OnDamaged(DamageContext ctx) {
        _damagePopupManager.Show(ctx);
    }
}