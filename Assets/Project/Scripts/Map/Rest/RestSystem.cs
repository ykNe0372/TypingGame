using System.Collections.Generic;
using UnityEngine;

public class RestSystem : MonoBehaviour {
    [SerializeField] private float _hpRecoverPercent = 30f;
    [SerializeField] private float _mpRecoverPercent = 30f;
    [SerializeField] private BlessingDataBase _dataBase;

    private Character _player;
    private RewardSelectionUI _selectionUI;
    private RestState _state;
    private bool _isFreeAvailable;  // 初回かどうか
    private bool _hasTakenBlessing; // 恩恵を受けたかどうか
    private readonly List<BuffData> _currentBlessings = new();

    public RestState State => _state;

    public void EnterRest(Character player, RewardSelectionUI selectionUI) {
        _player = player;
        _selectionUI = selectionUI;
        _state = RestState.MainMenu;
        _isFreeAvailable = true;
        _hasTakenBlessing = false;
        ShowMainMenu();
    }

    public void ExitRest() {
        _selectionUI.OnSelected -= HandleMainMenuSelected;
        _selectionUI.OnSelected -= HandleBlessingSelected;
        _selectionUI.Close();

        Debug.Log("Rest Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    private void ShowMainMenu() {
        _selectionUI.OnSelected -= HandleMainMenuSelected;
        _selectionUI.OnSelected -= HandleBlessingSelected;
        _selectionUI.Close();

        string paymentDescription = _isFreeAvailable
            ? "初回は無料、以降はアイテム1個を消費します。"
            : _player.SelectPaymentItem() == null
                ? "支払いに使えるアイテムがありません。"
                : "アイテム1個を消費します。";
        _selectionUI.OpenOptions(
            new[] { "HPを回復", "MPを回復", "恩恵を受ける" },
            new[] {
                $"最大HPの{_hpRecoverPercent}%を回復します。{paymentDescription}",
                $"最大MPの{_mpRecoverPercent}%を回復します。{paymentDescription}",
                _hasTakenBlessing ? "この休憩所では既に恩恵を受けています。" : $"恩恵を1つ選びます。{paymentDescription}"
            });
        _selectionUI.OnSelected += HandleMainMenuSelected;
    }

    private void HandleMainMenuSelected(int index) {
        switch (index) {
            case 0:
                RecoverHP();
                ShowMainMenu();
                break;
            case 1:
                RecoverMP();
                ShowMainMenu();
                break;
            case 2:
                if (!OpenBlessingMenu()) ShowMainMenu();
                break;
        }
    }

    private bool TryConsumeItem() {
        if (_isFreeAvailable) {
            _isFreeAvailable = false;
            return true;
        }

        GrowthItem item = _player.SelectPaymentItem();
        if (item == null) return false;

        ConsumePaymentItem(item);
        return true;
    }

    private void ConsumePaymentItem(GrowthItem item) {
        _player.RemoveItem(item);
    }

    public void RecoverHP() {
        if (!TryConsumeItem()) return;

        int amount = Mathf.FloorToInt(_player.MaxHP * (_hpRecoverPercent / 100f));
        _player.RecoverHP(amount);
        Debug.Log($"[Rest] HP Recover: {amount}");
    }

    public void RecoverMP() {
        if (!TryConsumeItem()) return;

        int amount = Mathf.FloorToInt(_player.MaxMP * (_mpRecoverPercent / 100f));
        _player.RecoverMP(amount);
        Debug.Log($"[Rest] MP Recover: {amount}");
    }

    public bool OpenBlessingMenu() {
        if (_hasTakenBlessing) return false;
        if (_dataBase.Items.Count == 0) {
            Debug.LogError("Cannot open blessing selection because the blessing database is empty.");
            return false;
        }
        if (!TryConsumeItem()) return false;

        GenerateBlessings(3);
        _state = RestState.BlessingSelect;
        _selectionUI.OnSelected -= HandleMainMenuSelected;
        _selectionUI.Close();
        _selectionUI.OpenBuffs(_currentBlessings);
        _selectionUI.OnSelected += HandleBlessingSelected;
        return true;
    }

    private void GenerateBlessings(int count) {
        _currentBlessings.Clear();
        List<BuffData> pool = new(_dataBase.Items);

        for (int i = 0; i < count && pool.Count > 0; ++i) {
            int index = Random.Range(0, pool.Count);
            _currentBlessings.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    private void HandleBlessingSelected(int index) {
        SelectBlessing(index);
    }

    public bool SelectBlessing(int index) {
        if (_state != RestState.BlessingSelect) return false;
        if (index < 0 || index >= _currentBlessings.Count) return false;

        BuffData blessing = _currentBlessings[index];
        _player.AddBuff(blessing);
        _hasTakenBlessing = true;    // 恩恵は1回まで
        _currentBlessings.Clear();
        _state = RestState.MainMenu;
        ShowMainMenu();

        Debug.Log($"Receive Blessing: {blessing.BuffName}");
        return true;
    }
}