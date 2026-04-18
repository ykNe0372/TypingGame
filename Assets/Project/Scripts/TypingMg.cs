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
}

[Serializable]
public class QuestionList {
    public List<Question> questions;
}


[RequireComponent(typeof(AudioSource))]
public partial class TypingMg : MonoBehaviour {
    public AudioClip correct;
    public AudioClip wrong;

    [SerializeField] private TextAsset _questionJson;
    [SerializeField] private TextMeshProUGUI textJapanese;
    [SerializeField] private TextMeshProUGUI textRoman;
    [SerializeField] private TextMeshProUGUI textNext;

    private TypingInput _input = new();
    private List<Question> currentQuestions;
    private readonly List<char> _roman = new();
    private int _romanIndex;
    private int _correctStreak;
    private int _nextQuestionIndex = -1;
    // private bool _isBonus = false;

    AudioSource aud;

    private void Start() {
        aud = GetComponent<AudioSource>();
        LoadQuestionsFromJson();
        if (currentQuestions == null || currentQuestions.Count == 0) {
            Debug.LogError("タイピング問題が存在しません。");
            return;
        }
        InitializeQuestion();
    }

    private void OnEnable() {
        Keyboard.current.onTextInput += OnTextInput;
    }

    private void OnDisable() {
        Keyboard.current.onTextInput -= OnTextInput;
    }

    private void OnTextInput(char c) {
        int result = _input.Input(c);

        switch (result) {
            case 1: // 正解タイプ時
                ++_romanIndex;
                textRoman.text = GenerateTextRoman();
                aud.PlayOneShot(correct);
                break;
            case 2: // タイプ完了時
                InitializeQuestion();
                ++_correctStreak;
                if (_correctStreak == 5) {
                    // _isBonus = true;
                    _correctStreak = 0;
                    Debug.Log("ボーナス！");
                }
                break;
            case 0:
                _correctStreak = 0;
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
        currentQuestions = data.questions;
    }

    // 問題切り替え（重複有）
    void InitializeQuestion() {
        // 次になる問題文を決定（1つ前の問題文とは重複しない）
        int questionCount = currentQuestions.Count;
        int newIndex = (_nextQuestionIndex == -1)
            ? UnityEngine.Random.Range(0, questionCount) // 最初だけランダム
            : _nextQuestionIndex;                        // 二回目以降は next から

        // 現在の問題をセット
        Question question = currentQuestions[newIndex];
        var candidates = RomajiConverter.Convert(question.reading);
        _input.SetCandidates(candidates);

        string displayRoman = candidates[0];
        _roman.Clear();
        _romanIndex = 0;
        foreach (char c in displayRoman) _roman.Add(c);
        _roman.Add('@');

        textJapanese.text = question.display;
        textRoman.text = GenerateTextRoman();

        // 次の問題文を決定（今の問題文とは重複しない）
        int nextIndex;
        do {
            nextIndex = UnityEngine.Random.Range(0, questionCount);
        } while (questionCount > 1 && nextIndex == newIndex);

        // _lastQuestionIndex = newIndex;
        _nextQuestionIndex = nextIndex;

        // nextText に次の問題文を格納
        textNext.text = currentQuestions[_nextQuestionIndex].display;
    }

    // ローマ字の表示を管理
    string GenerateTextRoman() {
        string text = "<style=typed>";
        string current = _input.GetCurrent();
        string candidate = _input.GetCurrentCandidate();
        text += current;

        int index = _input.GetCurrent().Length;

        text += "</style><style=untyped>";
        text += candidate.Substring(current.Length);
        text += "</style>";
        return text;
    }
}
