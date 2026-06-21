using UnityEngine;
using UnityEngine.InputSystem;

public class DebugController : MonoBehaviour {
    [SerializeField] private bool _isDebugMode = true;
    [SerializeField] private GameObject _debugPanel;
    [SerializeField] private TestStatus _testStatus;
    [SerializeField] private Character _player;
    [SerializeField] private TypingManager _typingManager;
    [SerializeField] private RunManager _runManager;
    [SerializeField] private DebugMapViewer _mapViewer;

    private void Update() {
        if (Keyboard.current.f12Key.wasPressedThisFrame) _debugPanel.SetActive(!_debugPanel.activeSelf);
        if (!_isDebugMode) return;
    }

    public void Debug_NextQuestion() => _typingManager.Debug_NextQuestion();
    public void Debug_ForceBonus() => _typingManager.Debug_ForceBonus();
    public void Debug_ToggleRuby() => _typingManager.Debug_ToggleRuby();
    public void Debug_PrintItems() => _player.Debug_PrintItems();
    public void Debug_EnterBattle() => _runManager.Debug_MoveTo(MapType.Battle);
    public void Debug_EnterShop() => _runManager.Debug_MoveTo(MapType.Shop);
    public void Debug_EnterRest() => _runManager.Debug_MoveTo(MapType.Rest);
    public void Debug_EnterBoss() => _runManager.Debug_MoveTo(MapType.Boss);
    public void Debug_StatusCheck() => _testStatus.Debug_StatusCheckPMD();
    public void Debug_CompleteBattle() => _runManager.Debug_CompleteBattle();
    public void Debug_MapVier() => _mapViewer.PrintMap();
}