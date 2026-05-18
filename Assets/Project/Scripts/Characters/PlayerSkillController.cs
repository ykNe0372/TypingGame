using UnityEngine;

public class PlayerSkillController : MonoBehaviour {
    [SerializeField] private Character _player;

    private InputManager _inputManager;
    private int _lastIndex = -1;

    private void Awake() {
        _inputManager = FindFirstObjectByType<InputManager>();
    }

    private void OnEnable() {
        _inputManager.OnDigitInput += OnDigitInput;
    }

    private void OnDisable() {
        _inputManager.OnDigitInput += OnDigitInput;
    }

    public void OnDigitInput(int index) {
        // 属性・技を切り替える
        if (_lastIndex == index) _player.CycleElement();
        else _player.ChangeSkill(index);

        _lastIndex = index;
    }
}