using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;

public class MapSelectInput : MonoBehaviour {
    private MapNavigator _navigator;

    private void Awake() {
        _navigator = RunManager.Instance.Navigator;
    }

    private void Update() {
        if (!GameStateManager.Instance.IsState(GameState.MapSelect)) return;

        _navigator = RunManager.Instance.Navigator;
        if (_navigator == null) return;

        var selectable = _navigator.GetSelectableNodes();
        CheckSelect(0, Keyboard.current.digit1Key, selectable);
        CheckSelect(1, Keyboard.current.digit2Key, selectable);
        CheckSelect(2, Keyboard.current.digit3Key, selectable);
    }

    private void CheckSelect(int index, KeyControl key, List<MapNode> selectable) {
        if (!key.wasPressedThisFrame) return;
        if (index >= selectable.Count) return;

        MapNode nextNode = selectable[index];
        RunManager.Instance.EnterNode(nextNode);
        // GameStateManager.Instance.ChangeState(GameState.Battle);
    }
}