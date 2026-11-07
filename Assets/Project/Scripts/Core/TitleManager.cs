using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class TitleManager : MonoBehaviour {
    [SerializeField] private TMP_Text _startPrompt;

    private bool _isLoading = false;
    private int _typedCharacters;
    private Keyboard _keyboard;

    private void Awake() {
        if (_startPrompt == null) {
            Debug.LogError("Start prompt text is not assigned.", this);
            enabled = false;
            return;
        }

        UpdateStartPrompt();
    }

    private void OnEnable() {
        _keyboard = Keyboard.current;
        if (_keyboard == null) {
            Debug.LogError("A keyboard is required to start the game.", this);
            return;
        }

        _keyboard.onTextInput += HandleTextInput;
    }

    private void OnDisable() {
        if (_keyboard == null) return;
        _keyboard.onTextInput -= HandleTextInput;
        _keyboard = null;
    }

    private void HandleTextInput(char input) {
        if (_isLoading) return;

        char normalized = TypingInputNormalizer.Normalize(input);
        if (!char.IsLetter(normalized)) return;

        const string startWord = "start";
        if (normalized == startWord[_typedCharacters]) {
            ++_typedCharacters;
        } else {
            _typedCharacters = normalized == startWord[0] ? 1 : 0;
        }

        UpdateStartPrompt();
        if (_typedCharacters == startWord.Length) StartGame();
    }

    private void UpdateStartPrompt() {
        const string startWord = "START";
        string typed = startWord[.._typedCharacters];
        string remaining = startWord[_typedCharacters..];
        _startPrompt.text = $"[ <color=#FFD166>{typed}</color>{remaining} ]";
    }

    private void StartGame() {
        if (_isLoading) return;
        _isLoading = true;

        SceneManager.LoadScene(SceneNames.Typing);
    }

    public void QuitGame() {
        Application.Quit();
    }
}