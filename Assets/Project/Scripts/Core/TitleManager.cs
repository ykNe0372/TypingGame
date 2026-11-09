using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {
    [SerializeField] private TMP_Text _startPrompt;
    [SerializeField] private Image _instructionImage;
    [SerializeField] private Sprite[] _instructionPages;

    private bool _isLoading = false;
    private int _instructionPageIndex = -1;
    private int _typedCharacters;
    private Keyboard _keyboard;

    private void Awake() {
        if (_startPrompt == null) {
            Debug.LogError("Start prompt text is not assigned.", this);
            enabled = false;
            return;
        }

        if (_instructionImage != null) {
            _instructionImage.enabled = false;
        }

        if (_instructionImage == null || _instructionPages == null || _instructionPages.Length == 0) {
            Debug.LogError("Instruction image and at least one instruction page must be assigned.", this);
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

        string targetWord = _instructionPageIndex >= 0 ? "ok" : "start";
        if (normalized == targetWord[_typedCharacters]) {
            ++_typedCharacters;
        } else {
            _typedCharacters = normalized == targetWord[0] ? 1 : 0;
        }

        UpdateStartPrompt();
        if (_typedCharacters != targetWord.Length) return;

        if (_instructionPageIndex < 0) {
            ShowInstructions();
        } else if (_instructionPageIndex < _instructionPages.Length - 1) {
            ShowNextInstructionPage();
        } else {
            StartGame();
        }
    }

    private void UpdateStartPrompt() {
        string targetWord = _instructionPageIndex >= 0 ? "OK" : "START";
        string typed = targetWord[.._typedCharacters];
        string remaining = targetWord[_typedCharacters..];
        _startPrompt.text = $"[ <color=#FFD166>{typed}</color>{remaining} ]";
    }

    private void ShowInstructions() {
        _instructionPageIndex = 0;
        ShowCurrentInstructionPage();
    }

    private void ShowNextInstructionPage() {
        ++_instructionPageIndex;
        ShowCurrentInstructionPage();
    }

    private void ShowCurrentInstructionPage() {
        _typedCharacters = 0;
        _instructionImage.sprite = _instructionPages[_instructionPageIndex];
        _instructionImage.enabled = true;
        UpdateStartPrompt();
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