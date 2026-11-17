using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using TMPro;

public class TitleManager : MonoBehaviour {
    private enum TitleCommand {
        None,
        Start,
        Tutorial
    }

    private const string StartWord = "start";
    private const string TutorialWord = "tutorial";

    [SerializeField] private TMP_Text _startText;
    [SerializeField] private TMP_Text _tutorialText;

    private bool _isLoading;
    private TitleCommand _activeCommand;
    private int _typedCharacters;
    private Keyboard _keyboard;

    private void Awake() {
        if (_startText == null || _tutorialText == null) {
            Debug.LogError("Start and tutorial prompt texts must be assigned.", this);
            enabled = false;
            return;
        }

        UpdatePrompts();
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

        ProcessCommandInput(normalized);
        UpdatePrompts();

        if (_activeCommand == TitleCommand.Start && _typedCharacters == StartWord.Length) LoadScene(SceneNames.Typing);
        else if (_activeCommand == TitleCommand.Tutorial && _typedCharacters == TutorialWord.Length) LoadScene(SceneNames.Tutorial);
    }

    private void ProcessCommandInput(char input) {
        if (_activeCommand == TitleCommand.None) {
            StartCommand(input);
            return;
        }

        string currentWord = GetWord(_activeCommand);
        if (_typedCharacters < currentWord.Length && input == currentWord[_typedCharacters]) {
            ++_typedCharacters;
            return;
        }

        if (input == currentWord[0]) {
            _typedCharacters = 1;
            return;
        }

        TitleCommand otherCommand = _activeCommand == TitleCommand.Start
            ? TitleCommand.Tutorial
            : TitleCommand.Start;
        if (input == GetWord(otherCommand)[0]) {
            _activeCommand = otherCommand;
            _typedCharacters = 1;
            return;
        }

        _activeCommand = TitleCommand.None;
        _typedCharacters = 0;
    }

    private void StartCommand(char input) {
        if (input == StartWord[0]) {
            _activeCommand = TitleCommand.Start;
            _typedCharacters = 1;
        } else if (input == TutorialWord[0]) {
            _activeCommand = TitleCommand.Tutorial;
            _typedCharacters = 1;
        }
    }

    private void UpdatePrompts() {
        int startProgress = _activeCommand == TitleCommand.Start ? _typedCharacters : 0;
        int tutorialProgress = _activeCommand == TitleCommand.Tutorial ? _typedCharacters : 0;
        _startText.text = FormatPrompt(StartWord, startProgress);
        _tutorialText.text = FormatPrompt(TutorialWord, tutorialProgress);
    }

    private static string FormatPrompt(string word, int typedCharacters) {
        string typed = word[..typedCharacters].ToUpperInvariant();
        string remaining = word[typedCharacters..].ToUpperInvariant();
        return $"[ <color=#FFD166>{typed}</color>{remaining} ]";
    }

    private static string GetWord(TitleCommand command) {
        return command switch {
            TitleCommand.Start => StartWord,
            TitleCommand.Tutorial => TutorialWord,
            _ => string.Empty
        };
    }

    private void LoadScene(string sceneName) {
        if (_isLoading) return;
        if (!Application.CanStreamedLevelBeLoaded(sceneName)) {
            Debug.LogError($"Scene '{sceneName}' is not included in the build settings.", this);
            return;
        }

        _isLoading = true;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }
}