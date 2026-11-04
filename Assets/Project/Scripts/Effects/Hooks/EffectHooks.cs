using System.Collections.Generic;

// キャラクターが所持している強化アイテム / レリックの効果のうち、
// フックインターフェースを実装しているものを呼び出す窓口
// 呼び出し順は「レリック → 強化アイテム（所持順）」
public class EffectHooks {
    private readonly Character _owner;
    private readonly List<object> _hooks = new();

    public EffectHooks(Character owner) {
        _owner = owner;
    }

    public void Rebuild(IEnumerable<object> sources) {
        _hooks.Clear();
        foreach (var source in sources) {
            if (source is UnityEngine.Object unityObject) {
                if (unityObject == null) continue;  // Inspector 上の空スロット対策
            } else if (source == null) continue;

            _hooks.Add(source);
        }
    }

    // ── ステータス ────────────────────────────────

    public void ModifyStatus(StatusType type, ref float add, ref float multiplier) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IStatusModifierHook hook) hook.ModifyStatus(_owner, type, ref add, ref multiplier);
        }
    }

    public float ModifyAfflictedTargetStatus(Character target, StatusEffectInstance status, StatusType type, float value) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IAfflictedTargetStatusHook hook) value = hook.ModifyAfflictedTargetStatus(_owner, target, status, type, value);
        }
        return value;
    }

    // ── 戦闘の進行 ────────────────────────────────

    public void BattleStart(BattleContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IBattleStartHook hook) hook.OnBattleStart(_owner, ctx);
        }
    }

    public void BattleUpdate(float deltaTime) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IBattleUpdateHook hook) hook.OnBattleUpdate(_owner, deltaTime);
        }
    }

    public void BattleEnd(BattleEndContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IBattleEndHook hook) hook.OnBattleEnd(_owner, ctx);
        }
    }

    // ── 攻撃・ダメージ ────────────────────────────

    public bool ShouldCancelIncomingAttack(AttackContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IIncomingAttackCancelHook hook && hook.ShouldCancelIncomingAttack(_owner, ctx)) return true;
        }
        return false;
    }

    public void BeforeAttack(AttackContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IBeforeAttackHook hook) hook.OnBeforeAttack(_owner, ctx);
        }
    }

    public void ModifyOutgoingDamage(AttackContext attack, AttackInstance instance, DamageContext damage) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IOutgoingDamageHook hook) hook.ModifyOutgoingDamage(_owner, attack, instance, damage);
        }
    }

    public void ModifyIncomingDamage(DamageContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IIncomingDamageHook hook) hook.ModifyIncomingDamage(_owner, ctx);
        }
    }

    public void AfterDamaged(DamageContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IAfterDamagedHook hook) hook.OnAfterDamaged(_owner, ctx);
        }
    }

    public void DealtDamage(DamageContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IDealtDamageHook hook) hook.OnDealtDamage(_owner, ctx);
        }
    }

    public void Kill(DamageContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IKillHook hook) hook.OnKill(_owner, ctx);
        }
    }

    public void Dodge(DamageContext ctx) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IDodgeHook hook) hook.OnDodge(_owner, ctx);
        }
    }

    public bool CanEvade() {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IEvasionBlockHook hook && !hook.CanEvade(_owner)) return false;
        }
        return true;
    }

    // ── MP・状態異常 ──────────────────────────────

    public void MPConsumed(int amount) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IMPConsumedHook hook) hook.OnMPConsumed(_owner, amount);
        }
    }

    public void StatusInflicted(Character target, StatusEffectData data) {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IStatusInflictedHook hook) hook.OnStatusInflicted(_owner, target, data);
        }
    }

    // ── タイピング ────────────────────────────────

    public TypoContext Typo() {
        var ctx = new TypoContext();
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is ITypoHook hook) hook.OnTypo(_owner, ctx);
        }
        return ctx;
    }

    public void WordCompleted() {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IWordCompletedHook hook) hook.OnWordCompleted(_owner);
        }
    }

    public void BonusAttack() {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IBonusAttackHook hook) hook.OnBonusAttack(_owner);
        }
    }

    // ── 回復・報酬・マップ ────────────────────────

    public float GetKillRecoveryMultiplier() {
        float multiplier = 1f;
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IKillRecoveryModifierHook hook) multiplier *= hook.GetKillRecoveryMultiplier(_owner);
        }
        return multiplier;
    }

    public bool ShouldRepeatReward() {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IRewardRepeatHook hook && hook.ShouldRepeatReward(_owner)) return true;
        }
        return false;
    }

    public int GetRemainingAreaRerolls() {
        int total = 0;
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IAreaRerollHook hook) total += hook.GetRemainingRerolls(_owner);
        }
        return total;
    }

    public bool TryConsumeAreaReroll() {
        for (int i = 0; i < _hooks.Count; ++i) {
            if (_hooks[i] is IAreaRerollHook hook && hook.TryConsumeReroll(_owner)) return true;
        }
        return false;
    }
}
