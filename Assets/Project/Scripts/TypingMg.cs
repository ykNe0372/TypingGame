using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class Question {
    public string japanese;
    public string roman;
}

public partial class TypingMg : MonoBehaviour {
    [SerializeField] private Question[] questions;
    [SerializeField] private TextMeshProUGUI textJapanese; // ここに日本語表示のTextMeshProをアタッチする。
    [SerializeField] private TextMeshProUGUI textRoman; // ここにローマ字表示のTextMeshProをアタッチする。

    private readonly List<char> _roman = new();
    private int _romanIndex;
    private readonly bool _isWindows;
    private readonly bool _isMac;

    private void Start() {
        InitializeQuestion();
    }

    private void OnGUI() {
        if (Event.current.type == EventType.KeyDown) {
            switch (InputKey(GetCharFromKeyCode(Event.current.keyCode))) {
                case 1: // 正解タイプ時
                    _romanIndex++;
                    if (_roman[_romanIndex] == '@') InitializeQuestion(); // 「@」がタイピングの終わりの判定となる。
                    else textRoman.text = GenerateTextRoman();
                    break;
                case 2: // ミスタイプ時
                    break;
            }
        }
    }

    void InitializeQuestion() {
        Question question = questions[UnityEngine.Random.Range(0, questions.Length)];
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
