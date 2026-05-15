using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NUnit.Framework.Internal;
using UnityEngine.InputSystem;

[Serializable]
public class Question {
    public string display;
    public string reading;
    public string ruby;
}

[Serializable]
public class QuestionList {
    public List<Question> questions;
}


[RequireComponent(typeof(AudioSource))]
public partial class TypingManager : MonoBehaviour {
    public AudioClip correct;
    public AudioClip wrong;

    [SerializeField] private TextAsset _questionJson;
    [SerializeField] private TextMeshProUGUI textJapanese;
    [SerializeField] private TextMeshProUGUI textRuby;
    [SerializeField] private TextMeshProUGUI textRoman;
    [SerializeField] private TextMeshProUGUI textNext;
    [SerializeField] private int _minLength = 1;
    [SerializeField] private int _maxLength = 15;

    [SerializeField] private CombatSystem _combatSystem;

    private readonly TypingInput _typingInput = new();
    private InputManager _inputManager;
    private List<Question> _currentQuestions;
    private readonly List<Question> _filteredQuestions = new();
    private Question _currentQuestion;
    private readonly List<char> _roman = new();
    private int _romanIndex;
    private int _correctStreak;  // 連続正解数
    private int _bonusChain;     // ボーナス段階
    private int _nextQuestionIndex = -1;
    private bool _isRubyEnabled = true;

    AudioSource aud;

    private void Start() {
        aud = GetComponent<AudioSource>();
    
        LoadQuestionsFromJson();
        if (_currentQuestions == null || _currentQuestions.Count == 0) return;
        InitializeQuestion();
    }

    private void Awake() {
        _inputManager = FindFirstObjectByType<InputManager>();  // Start() よりも前に取る
    }

    private void OnEnable() {
        // Keyboard.current.onTextInput += OnTextInput;
        _inputManager.OnCharInput += OnTextInput;
    }

    private void OnDisable() {
        // Keyboard.current.onTextInput -= OnTextInput;
        _inputManager.OnCharInput -= OnTextInput;
    }

    private void OnTextInput(char c) {
        int result = _typingInput.Input(c);

        switch (result) {
            case 1: // 正解タイプ時
                ++_romanIndex;
                textRoman.text = GenerateTextRoman();
                aud.PlayOneShot(correct);
                break;
            case 2: // タイプ完了時
                InitializeQuestion();
                _combatSystem.RequestAttack();
                ++_correctStreak;

                if (_correctStreak >= 5) {
                    _correctStreak = 0;
                    _combatSystem.RequestBonusAttack(_bonusChain);
                    ++_bonusChain;
                    Debug.Log($"Bonus: {_bonusChain}");
                }
                break;
            case 0:
                _correctStreak = 0;
                _bonusChain = 0;
                Debug.Log("リセット");
                aud.PlayOneShot(wrong);
                break;
        }
    }

    void LoadQuestionsFromJson() {
        if (_questionJson == null) {
            Debug.LogError("JSONファイルが設定されていません。");
            return;
        }
        QuestionList data = JsonUtility.FromJson<QuestionList>(_questionJson.text);
        if (data == null || data.questions == null) {
            Debug.LogError("JSONファイルの読み込みに失敗しました。");
            return;
        }
        _currentQuestions = data.questions;
    }

    void FilterQuestions() {
        _filteredQuestions.Clear();
        foreach (var q in _currentQuestions) {
            int len = q.reading?.Length ?? 0;
            if (len >= _minLength && len <= _maxLength) {
                _filteredQuestions.Add(q);
            }
        }
        if (_filteredQuestions.Count == 0) Debug.LogWarning($"該当する問題無し（条件: {_minLength}-{_maxLength}）");
    }

    public void SetLengthRange(int min, int max) {
        if (min > max) {
            (min, max) = (max, min);
        }
        _minLength = Mathf.Max(1, min);
        _maxLength = Mathf.Max(_minLength, max);

        Debug.Log($"[DEBUG] minLength: {_minLength}, maxLength: {_maxLength}");

        _nextQuestionIndex = -1;
        InitializeQuestion();
    }

    // 問題切り替え（重複有）
    void InitializeQuestion() {
        FilterQuestions();

        // 次になる問題文を決定（1つ前の問題文とは重複しない）
        int questionCount = _filteredQuestions.Count;
        int newIndex = (_nextQuestionIndex == -1)
            ? UnityEngine.Random.Range(0, questionCount) // 最初だけランダム
            : _nextQuestionIndex;                        // 二回目以降は next から

        // 現在の問題をセット
        _currentQuestion = _filteredQuestions[newIndex];
        Question question = _currentQuestion;

        var candidates = RomajiConverter.Convert(question.reading);
        _typingInput.SetCandidates(candidates);

        string displayRoman = candidates[0];
        _roman.Clear();
        _romanIndex = 0;
        foreach (char c in displayRoman) _roman.Add(c);
        _roman.Add('@');

        ApplyText();

        // 次の問題文を決定（今の問題文とは重複しない）
        int nextIndex;
        do {
            nextIndex = UnityEngine.Random.Range(0, questionCount);
        } while (questionCount > 1 && nextIndex == newIndex);

        // _lastQuestionIndex = newIndex;
        _nextQuestionIndex = nextIndex;

        // nextText に次の問題文を格納
        textNext.text = _currentQuestions[_nextQuestionIndex].display;
    }

    private void ApplyText() {
        if (_currentQuestion == null) return;
        textJapanese.text = _currentQuestion.display;
        textRoman.text = GenerateTextRoman();
        if (_isRubyEnabled) textRuby.text = _currentQuestion.reading;
        else textRuby.text = "";
    }

    // ローマ字の表示を管理
    string GenerateTextRoman() {
        string text = "<style=typed>";
        string current = _typingInput.GetCurrent();
        string candidate = _typingInput.GetCurrentCandidate();

        text += current;
        text += "</style><style=untyped>";
        text += candidate[current.Length..];
        text += "</style>";
        return text;
    }

    // ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭  DEBUG MODE  ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭

    public void Debug_NextQuestion() {
        InitializeQuestion();
        Debug.Log($"[DEBUG] Next Question");
    }

    public void Debug_ForceBonus() {
        InitializeQuestion();
        _correctStreak = 0;
        _combatSystem.RequestBonusAttack(_bonusChain);
        ++_bonusChain;
        Debug.Log($"[DEBUG] Bonus: {_bonusChain}");
    }

    public void Debug_ToggleRuby() {
        _isRubyEnabled = !_isRubyEnabled;
        ApplyText();
        Debug.Log($"[DEBUG] Toggle Ruby");
    }
}
