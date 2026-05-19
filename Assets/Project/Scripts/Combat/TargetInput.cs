using UnityEngine;
using UnityEngine.InputSystem;

public class TargetInput : MonoBehaviour {
    [SerializeField] private CombatSystem _combatSystem;

    private void Update() {
        if (_combatSystem.State != CombatState.Playing) return;
    
        if (Keyboard.current.leftArrowKey.wasPressedThisFrame) _combatSystem.MoveTargetLeft();
        if (Keyboard.current.rightArrowKey.wasPressedThisFrame) _combatSystem.MoveTargetRight();
    }
}