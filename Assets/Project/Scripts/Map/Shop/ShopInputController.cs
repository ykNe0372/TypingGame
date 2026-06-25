using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class ShopInputController : MonoBehaviour {
    [SerializeField] private GameState _shopState;
    [SerializeField] private MonoBehaviour _shopBehaviour;

    private bool _waitKeyRelease;
    private GameState _previousState;
    private IShop _shop;

    private void Awake() {
        _shop = _shopBehaviour as IShop;
    }

    private void Update() {
        // 入店時に押す数字キーで購入しないようにキーを離すまでロック
        GameState currentState = GameStateManager.Instance.CurrentState;
        if (_previousState != _shopState && currentState == _shopState) _waitKeyRelease = true;
        
        _previousState = currentState;
        if (_waitKeyRelease) {
            if (!Keyboard.current.anyKey.isPressed) _waitKeyRelease = false;
            return;
        }

        if (!GameStateManager.Instance.IsState(_shopState)) return;

        CheckPurchase(0, Keyboard.current.digit1Key);
        CheckPurchase(1, Keyboard.current.digit2Key);
        CheckPurchase(2, Keyboard.current.digit3Key);
        CheckPurchase(3, Keyboard.current.digit4Key);
        CheckPurchase(4, Keyboard.current.digit5Key);

        if (Keyboard.current.rKey.wasPressedThisFrame) _shop.Reroll();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) _shop.ExitShop();
    }

    private void CheckPurchase(int index, KeyControl key) {
        if (!key.wasPressedThisFrame) return;

        _shop.Purchase(index);
    }
}