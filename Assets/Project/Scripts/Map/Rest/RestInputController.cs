using UnityEngine;
using UnityEngine.InputSystem;

public class RestInputController : MonoBehaviour {
    [SerializeField] private RestSystem _restSystem;

    private bool _waitKeyRelease;
    private GameState _previousState;

    private void Update() {
        // 入場時に押す数字キーで選択しないようにキーを離すまでロック
        GameState currentState = GameStateManager.Instance.CurrentState;
        if (_previousState != GameState.Rest && currentState == GameState.Rest) _waitKeyRelease = true;
        
        _previousState = currentState;
        if (_waitKeyRelease) {
            if (!Keyboard.current.anyKey.isPressed) _waitKeyRelease = false;
            return;
        }

        if (!GameStateManager.Instance.IsState(GameState.Rest)) return;

        if (_restSystem.State == RestState.BlessingSelect) {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) _restSystem.SelectBlessing(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) _restSystem.SelectBlessing(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) _restSystem.SelectBlessing(2);
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) _restSystem.RecoverHP();
        if (Keyboard.current.digit2Key.wasPressedThisFrame) _restSystem.RecoverMP();
        if (Keyboard.current.digit3Key.wasPressedThisFrame) _restSystem.OpenBlessingMenu();

        if (Keyboard.current.escapeKey.wasPressedThisFrame && _restSystem.State == RestState.MainMenu) _restSystem.ExitRest();
    }
}