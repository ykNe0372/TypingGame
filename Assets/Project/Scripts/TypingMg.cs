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

public partial class TypingMg : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textJapanese;
    [SerializeField] private TextMeshProUGUI textRoman;

    private Question[] questions;
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

    private void Awake() {
        LoadQuesitonsFromJson();
    }

    void LoadQuesitonsFromJson() {
        // Resources フォルダから questions.json を読み込む
        TextAsset jsonText = Resources.Load<TextAsset>("questions");
        if (jsonText != null) {
            questions = JsonHelper.FromJson<Question>(jsonText.text);
        } else {
            Debug.LogError("questions.json が見つかりません");
        }
    }

    // 問題切り替え（重複有）
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

// JsonHelper ユーティリティー
public static class JsonHelper {
    public static T[] FromJson<T>(string json) {
        // JSONのルートが配列の場合、JsonUtilityが直接扱えないため、"array"というキーを持つオブジェクトでラップする
        string newJson = "{ \"array\": " + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.array;
    }

    [System.Serializable]
    private class Wrapper<T> { // FromJsonメソッド内でのみ使用されるヘルパー的な内部クラス
        public T[] array;
    }
}
