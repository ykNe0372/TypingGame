using UnityEngine;
using System;
using System.Collections.Generic;

public class DebugController : MonoBehaviour {
    [SerializeField] private bool _isDebugMode = true;
    [SerializeField] private GameObject _debugPanel;
    [SerializeField] private TypingManager _typingMg;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F12)) _debugPanel.SetActive(!_debugPanel.activeSelf);
        if (!_isDebugMode) return;
    }

    public void Debug_NextQuestion() => _typingMg.Debug_NextQuestion();
    public void Debug_ForceBonus() => _typingMg.Debug_ForceBonus();
    public void Debug_ToggleRuby() => _typingMg.Debug_ToggleRuby();
}