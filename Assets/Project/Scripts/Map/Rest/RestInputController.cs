using UnityEngine;
using UnityEngine.InputSystem;

public class RestInputController : MonoBehaviour {
    [SerializeField] private Character _player;
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

        if (Keyboard.current.digit1Key.wasPressedThisFrame) _restSystem.RecoverHP(_player);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) _restSystem.RecoverMP(_player);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) _restSystem.ReceiveBlessingAttack(_player);
        // if (Keyboard.current.digit4Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 3);
        // if (Keyboard.current.digit5Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 4);

        if (Keyboard.current.escapeKey.wasPressedThisFrame) _restSystem.ExitRest();
    }
}