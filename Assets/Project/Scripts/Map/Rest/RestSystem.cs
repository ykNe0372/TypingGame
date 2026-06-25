using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RestSystem : MonoBehaviour {
    [SerializeField] private float _hpRecoverPercent = 30f;
    [SerializeField] private float _mpRecoverPercent = 30f;
    [SerializeField] private BlessingDataBase _dataBase;

    private Character _player;
    private RestState _state;
    private bool _isFreeAvailable;  // 初回かどうか
    private bool _hasTakenBlessing; // 恩恵を受けたかどうか
    private readonly List<BuffData> _currentBlessings = new();

    public RestState State => _state;

    public void EnterRest(Character player) {
        _player = player;
        _state = RestState.MainMenu;
        _isFreeAvailable = true;
        _hasTakenBlessing = false;
    }

    public void ExitRest() {
        Debug.Log("Rest Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
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
        if (!TryConsumeItem()) return false;

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

    public bool SelectBlessing(int index) {
        if (_state != RestState.BlessingSelect) return false;
        if (index < 0 || index >= _currentBlessings.Count) return false;

        BuffData blessing = _currentBlessings[index];
        _player.AddBuff(blessing);
        _hasTakenBlessing = true;    // 恩恵は1回まで
        _currentBlessings.Clear();
        _state = RestState.MainMenu;

        Debug.Log($"Receive Blessing: {blessing.BuffName}");
        return true;
    }
}