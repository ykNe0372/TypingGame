using UnityEngine;
using UnityEngine.InputSystem;

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
        // 入店時に押すキーで即時操作しないように、キーを離すまで待つ
        GameState currentState = GameStateManager.Instance.CurrentState;
        if (_previousState != _shopState && currentState == _shopState) _waitKeyRelease = true;
        
        _previousState = currentState;
        if (_waitKeyRelease) {
            if (!Keyboard.current.anyKey.isPressed) _waitKeyRelease = false;
            return;
        }

        if (!GameStateManager.Instance.IsState(_shopState)) return;

        if (Keyboard.current.rKey.wasPressedThisFrame) _shop.Reroll();
        if (Keyboard.current.escapeKey.wasPressedThisFrame) _shop.ExitShop();
    }
}