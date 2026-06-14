using TMPro;
using UnityEngine;

public class RestSystem : MonoBehaviour {
    [SerializeField] private float _hpRecoverPercent = 30f;
    [SerializeField] private float _mpRecoverPercent = 30f;
    [SerializeField] private BuffData _attackBuff;

    private bool _isFreeAvailable;
    private Character _player;

    public void EnterRest(Character player) {
        _isFreeAvailable = true;
        _player = player;
    }

    public void ExitRest() {
        Debug.Log("Shop Rest");
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

        int amount = Mathf.FloorToInt(_player.MaxHP * (_hpRecoverPercent / 100f));
        _player.RecoverHP(amount);
        Debug.Log($"[Rest] HP Recover: {amount}");
    }

    public void RecoverMP(Character player) {
        if (!TryConsumeItem(player)) return;

        int amount = Mathf.FloorToInt(_player.MaxMP * (_mpRecoverPercent / 100f));
        _player.RecoverMP(amount);
        Debug.Log($"[Rest] MP Recover: {amount}");
    }

    public void ReceiveBlessingAttack(Character player) {
        if (!TryConsumeItem(player)) return;

        player.AddBuff(_attackBuff);
        Debug.Log("攻撃の恩恵を獲得");        
    }
}