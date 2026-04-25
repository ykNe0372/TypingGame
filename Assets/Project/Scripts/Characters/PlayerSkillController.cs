using UnityEngine;
using UnityEngine.Pool;

public class PlayerSkillController : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private float _doublePressThreshold = 0.3f;  // 押し間違い対策

    private InputManager _inputManager;
    private int _lastIndex = -1;
    private float _lastInputTime;

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
        float now = Time.time;

        // 属性・技を切り替える
        if (_lastIndex == index && now - _lastInputTime < _doublePressThreshold) _player.CycleElement();
        else _player.ChangeSkill(index);

        _lastIndex = index;
        _lastInputTime = now;
    }
}