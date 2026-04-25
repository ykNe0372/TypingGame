using UnityEngine;
using UnityEngine.Pool;

public class PlayerSkillController : MonoBehaviour {
    [SerializeField] private Character _player;

    private InputManager _inputManager;

    private void Awake() {
        _inputManager = FindFirstObjectByType<InputManager>();
    }

    private void OnEnable() {
        _inputManager.OnDigitInput += HandleDigit;
    }

    private void OnDisable() {
        _inputManager.OnDigitInput -= HandleDigit;
    }

    private void HandleDigit(int index) {
        _player.ChangeSkill(index);
    }
}