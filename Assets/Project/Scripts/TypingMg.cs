using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using NUnit.Framework.Internal;

[Serializable]
public class Question {
    public string japanese;
    public string roman;
}

public enum Difficulty {
    Easy,
    Normal,
    Hard,
    VeryHard,
    Impossible,
}

[Serializable]
public class DifficultyQuestions {
    public Difficulty difficulty;
    public List<Question> questions = new();
}

public partial class TypingMg : MonoBehaviour {
    public AudioClip correct;
    public AudioClip wrong;

    [SerializeField] private List<DifficultyQuestions> allQuestions;
    [SerializeField] private Difficulty selectedDifficulty;
    [SerializeField] private TextMeshProUGUI textJapanese;
    [SerializeField] private TextMeshProUGUI textRoman;

    private List<Question> currentQuestions;
    private readonly List<char> _roman = new();
    private int _romanIndex;
    private int _correctStreak;
    private bool _isWindows;
    private bool _isMac;
    private bool _isBonus = false;

    AudioSource aud;

    private void Start() {
        aud = GetComponent<AudioSource>();
        currentQuestions = allQuestions.Find(dq => dq.difficulty == selectedDifficulty)?.questions;
        if (currentQuestions == null || currentQuestions.Count == 0) {
            Debug.LogError("選択した難易度の問題がありません。");
            return;
        }

        InitializeQuestion();

        if (SystemInfo.operatingSystem.Contains("Windows")) {
            _isWindows = true;
        }
        if (SystemInfo.operatingSystem.Contains("Mac")) {
            _isMac = true;
        }
    }

    private void OnGUI() {
        if (Event.current.type == EventType.KeyDown) {
            switch (InputKey(GetCharFromKeyCode(Event.current.keyCode))) {
                case 1: // 正解タイプ時
                    _romanIndex++;

                    if (_roman[_romanIndex] == '@') {
                        InitializeQuestion(); // 「@」がタイピングの終わりの判定となる。
                        _correctStreak++;
                        if (_correctStreak == 5) { // ボーナス判定
                            _isBonus = true;
                            _correctStreak = 0;
                            Debug.Log("ボーナス！");
                        }
                    } else textRoman.text = GenerateTextRoman();

                    aud.PlayOneShot(correct);
                    break;
                case 2: // ミスタイプ時
                    _correctStreak = 0;
                    Debug.Log("リセット");
                    aud.PlayOneShot(wrong);
                    break;
            }
        }
    }

    // 問題切り替え（重複有）
    void InitializeQuestion() {
        Question question = currentQuestions[UnityEngine.Random.Range(0, currentQuestions.Count)];
        _roman.Clear();
        _romanIndex = 0;
        char[] characters = question.roman.ToCharArray();

        foreach (char character in characters) {
            _roman.Add(character);
        }

        _roman.Add('@');
        textJapanese.text = question.japanese;
        textRoman.text = GenerateTextRoman();
    }

    // ローマ字の表示を管理
    string GenerateTextRoman() {
        string text = "<style=typed>";
        for (int i = 0; i < _roman.Count; i++) {
            if (_roman[i] == '@') break;
            if (i == _romanIndex) text += "</style><style=untyped>";
            text += _roman[i];
        }
        text += "</style>";
        return text;
    }
}
