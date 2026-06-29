using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public event Action<char> OnCharInput;
    public event Action OnSpecialInput;

    private void OnEnable() {
        Keyboard.current.onTextInput += HandleTextInput;
    }

    private void OnDisable() {
        Keyboard.current.onTextInput -= HandleTextInput;
    }

    private void Update() {
        if (Keyboard.current.enterKey.wasPressedThisFrame) OnSpecialInput?.Invoke();
    }

    private void HandleTextInput(char c) {
        char normalized = TypingInputNormalizer.Normalize(c);

        if (!TypingInputNormalizer.IsTyppingCharacter(normalized)) return;
        OnCharInput?.Invoke(normalized);

    }
}