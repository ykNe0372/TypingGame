using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public event Action<char> OnCharInput;
    public event Action<int> OnDigitInput;

    private void Update() {
        foreach (char c in Input.inputString) {
            if (char.IsDigit(c)) {
                int index = Mathf.Max(0, c - '1');  // (char) '1' → (int) 0
                OnDigitInput?.Invoke(index);
            } else OnCharInput?.Invoke(c);
        }
    }
}