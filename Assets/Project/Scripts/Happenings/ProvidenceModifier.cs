using UnityEngine;

[CreateAssetMenu(menuName = "BattleModifier/Providence")]
public class ProvidenceModifier : BattleModifierBase {
    [SerializeField] private float _interval = 1f;

    private float _timer;

    public override void OnBattleStart(BattleContext ctx) {
        _timer = 0f;
    }

    public override void OnUpdate(BattleContext ctx, float deltaTime) {
        _timer += deltaTime;
        if (_timer < _interval) return;

        _timer -= _interval;
        RecoverPlayer(ctx.Player);
    }

    private void RecoverPlayer(Character player) {
        int hpRecover = Mathf.FloorToInt(player.MaxHP * 0.02f);
        int mpRecover = Mathf.FloorToInt(player.MaxMP * 0.01f);

        if (hpRecover == 0) hpRecover = 1;
        if (mpRecover == 0) mpRecover = 1;

        player.RecoverHP(hpRecover);
        player.RecoverMP(mpRecover);
        Debug.Log($"[Providence] HP: {hpRecover}, MP: {mpRecover}");
    }
}