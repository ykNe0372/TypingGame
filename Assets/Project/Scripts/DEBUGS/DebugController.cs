using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour {
    [SerializeField] private bool _isDebugMode = true;
    [SerializeField] private GameObject _debugPanel;
    [SerializeField] private Character _player;
    [SerializeField] private TypingManager _typingManager;

    private void Update() {
        if (Keyboard.current.f12Key.wasPressedThisFrame) _debugPanel.SetActive(!_debugPanel.activeSelf);
        if (!_isDebugMode) return;
    }

    public void Debug_NextQuestion() => _typingManager.Debug_NextQuestion();
    public void Debug_ForceBonus() => _typingManager.Debug_ForceBonus();
    public void Debug_ToggleRuby() => _typingManager.Debug_ToggleRuby();
    public void Debug_PrintItems() => _player.Debug_PrintItems();
}