using UnityEngine;
using UnityEngine.InputSystem;

public class ShopInputController : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private ShopSystem _shopSystem;

    private bool _waitKeyRelease;
    private GameState _previousState;

    private void Update() {
        // 入店時に押す数字キーで購入しないようにキーを離すまでロック
        GameState currentState = GameStateManager.Instance.CurrentState;
        if (_previousState != GameState.Shop && currentState == GameState.Shop) _waitKeyRelease = true;
        
        _previousState = currentState;
        if (_waitKeyRelease) {
            if (!Keyboard.current.anyKey.isPressed) _waitKeyRelease = false;
            return;
        }

        if (Keyboard.current.digit1Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) _shopSystem.Purchase(_player, 4);

        if (Keyboard.current.rKey.wasPressedThisFrame) _shopSystem.Reroll();

        if (Keyboard.current.escapeKey.wasPressedThisFrame) _shopSystem.ExitShop(_player);
    }
}