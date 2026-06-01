using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public event Action<char> OnCharInput;

    private void OnEnable() {
        Keyboard.current.onTextInput += HandleTextInput;
    }

    private void OnDisable() {
        Keyboard.current.onTextInput -= HandleTextInput;
    }

    private void HandleTextInput(char c) {
        char normalized = TypingInputNormalizer.Normalize(c);

        if (!TypingInputNormalizer.IsTyppingCharacter(normalized)) return;
        OnCharInput?.Invoke(normalized);

    }
}