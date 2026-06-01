using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerSkillController : MonoBehaviour {
    [SerializeField] private Character _player;

    private int _lastIndex = -1;

    private void Update() {
        if (!GameStateManager.Instance.IsState(GameState.Battle)) return;

        CheckDigitKey(0, Keyboard.current.digit1Key);
        CheckDigitKey(1, Keyboard.current.digit2Key);
        CheckDigitKey(2, Keyboard.current.digit3Key);
        CheckDigitKey(3, Keyboard.current.digit4Key);
    }

    private void CheckDigitKey(int index, KeyControl key) {
        if (!key.wasPressedThisFrame) return;
        
        if (_lastIndex == index) _player.CycleElement();
        else _player.ChangeSkill(index);

        _lastIndex = index;
    }
}