using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestSystem : MonoBehaviour {
    [SerializeField] private float _hpRecoverPercent = 30f;
    [SerializeField] private float _mpRecoverPercent = 30f;
    [SerializeField] private BlessingDataBase _dataBase;

    private RestState _state;
    private bool _isFreeAvailable;  // 初回かどうか
    private bool _hasTakenBlessing; // 恩恵を受けたかどうか
    private readonly List<BuffData> _currentBlessings = new();

    public RestState State => _state;

    public void EnterRest() {
        _state = RestState.MainMenu;
        _isFreeAvailable = true;
        _hasTakenBlessing = false;
    }

    public void ExitRest() {
        Debug.Log("Rest Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    private bool TryConsumeItem(Character player) {
        if (_isFreeAvailable) {
            _isFreeAvailable = false;
            return true;
        }

        GrowthItem item = player.SelectPaymentItem();
        if (item == null) return false;

        ConsumePaymentItem(player, item);
        return true;
    }

    private void ConsumePaymentItem(Character player, GrowthItem item) {
        player.RemoveItem(item);
    }

    public void RecoverHP(Character player) {
        if (!TryConsumeItem(player)) return;

        int amount = Mathf.FloorToInt(player.MaxHP * (_hpRecoverPercent / 100f));
        player.RecoverHP(amount);
        Debug.Log($"[Rest] HP Recover: {amount}");
    }

    public void RecoverMP(Character player) {
        if (!TryConsumeItem(player)) return;

        int amount = Mathf.FloorToInt(player.MaxMP * (_mpRecoverPercent / 100f));
        player.RecoverMP(amount);
        Debug.Log($"[Rest] MP Recover: {amount}");
    }

    public bool OpenBlessingMenu(Character player) {
        if (_hasTakenBlessing) return false;
        if (!TryConsumeItem(player)) return false;

        GenerateBlessings(3);
        _state = RestState.BlessingSelect;
        PrintBlessings();
        return true;
    }

    private void GenerateBlessings(int count) {
        _currentBlessings.Clear();
        List<BuffData> pool = new(_dataBase.Items);

        for (int i=0; i<count; ++i) {
            int index = Random.Range(0, pool.Count);
            _currentBlessings.Add(pool[index]);
            pool.RemoveAt(index);
        }
    }

    private void PrintBlessings() {
        Debug.Log("xxx--- Blessings ---xxx");
        for (int i=0; i<_currentBlessings.Count; ++i) {
            Debug.Log($"{i+1}: {_currentBlessings[i].BuffName}");
        }
    }

    public bool SelectBlessing(Character player, int index) {
        if (_state != RestState.BlessingSelect) return false;
        if (index < 0 || index >= _currentBlessings.Count) return false;

        BuffData blessing = _currentBlessings[index];
        player.AddBuff(blessing);
        _hasTakenBlessing = true;    // 恩恵は1回まで
        _currentBlessings.Clear();
        _state = RestState.MainMenu;

        Debug.Log($"Receive Blessing: {blessing.BuffName}");
        return true;
    }
}