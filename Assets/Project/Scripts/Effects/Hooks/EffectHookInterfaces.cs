// ─────────────────────────────────────────────────────────────
// 強化アイテム（Effect）/ レリック（RelicEffect）が実装できるフック一覧
//
// Effect / RelicEffect を継承した ScriptableObject に、必要なインターフェースだけを実装すれば
// Character が自動で拾って呼び出す（EffectHooks 参照）。
// ScriptableObject はアセットとして共有されるため、戦闘中に変化する値（スタック数・タイマー等）は
// フィールドに持たず、owner.EffectStates.Get<T>(this) に保存すること。
// ─────────────────────────────────────────────────────────────

// ステータス補正（GetFinalStatus 内で呼ばれる）
// add: 加算値 / multiplier: 乗算値（1f = 変化なし）
// ※ 必ず最初に type を判定し、無関係なステータスなら即 return すること（負荷・再帰対策）
public interface IStatusModifierHook {
    void ModifyStatus(Character owner, StatusType type, ref float add, ref float multiplier);
}

// 自分が付与した状態異常にかかっている相手のステータス補正
// （状態異常の敵の攻撃力・防御力を下げる等）
public interface IAfflictedTargetStatusHook {
    float ModifyAfflictedTargetStatus(Character owner, Character target, StatusEffectInstance status, StatusType type, float value);
}

// 戦闘開始時
public interface IBattleStartHook {
    void OnBattleStart(Character owner, BattleContext ctx);
}

// 戦闘中の毎フレーム（CombatState.Playing の間のみ）
public interface IBattleUpdateHook {
    void OnBattleUpdate(Character owner, float deltaTime);
}

// 戦闘終了時（勝利 / 敗北）
public interface IBattleEndHook {
    void OnBattleEnd(Character owner, BattleEndContext ctx);
}

// 自分を対象にした攻撃が発動する直前。true を返すとその攻撃を自分に対して無効化する
public interface IIncomingAttackCancelHook {
    bool ShouldCancelIncomingAttack(Character owner, AttackContext ctx);
}

// 自分が攻撃を発動する直前（既存の OnAttackEffect より前）
public interface IBeforeAttackHook {
    void OnBeforeAttack(Character owner, AttackContext ctx);
}

// 自分が与えるダメージの補正（会心判定の前に呼ばれる）
public interface IOutgoingDamageHook {
    void ModifyOutgoingDamage(Character owner, AttackContext attack, AttackInstance instance, DamageContext damage);
}

// 自分が受けるダメージの補正（HP 減少の直前。環境ダメージも含む）
public interface IIncomingDamageHook {
    void ModifyIncomingDamage(Character owner, DamageContext ctx);
}

// 自分がダメージを受けた直後（環境ダメージも含むので必要に応じて ctx.IsEnvironmentDamage で弾く）
public interface IAfterDamagedHook {
    void OnAfterDamaged(Character owner, DamageContext ctx);
}

// 自分が相手にダメージを与えた直後（相手が生存している場合のみ）
public interface IDealtDamageHook {
    void OnDealtDamage(Character owner, DamageContext ctx);
}

// 自分が相手を倒した時
public interface IKillHook {
    void OnKill(Character owner, DamageContext ctx);
}

// 自分が回避した時
public interface IDodgeHook {
    void OnDodge(Character owner, DamageContext ctx);
}

// MP を消費した時
public interface IMPConsumedHook {
    void OnMPConsumed(Character owner, int amount);
}

// 状態異常の付与に成功した時
public interface IStatusInflictedHook {
    void OnStatusInflicted(Character owner, Character target, StatusEffectData data);
}

// タイプミスした時
public interface ITypoHook {
    void OnTypo(Character owner, TypoContext ctx);
}

// 1文打ち切った時（攻撃発動後）
public interface IWordCompletedHook {
    void OnWordCompleted(Character owner);
}

// ノーミスボーナス攻撃が発動した時
public interface IBonusAttackHook {
    void OnBonusAttack(Character owner);
}

// 敵討伐時の回復量に掛かる倍率
public interface IKillRecoveryModifierHook {
    float GetKillRecoveryMultiplier(Character owner);
}

// クリア報酬を受け取った後、もう一度受け取れるか
public interface IRewardRepeatHook {
    bool ShouldRepeatReward(Character owner);
}

// 回避できるか（false を返すと回避判定そのものを行わない）
public interface IEvasionBlockHook {
    bool CanEvade(Character owner);
}

// 次のエリアの再抽選
public interface IAreaRerollHook {
    int GetRemainingRerolls(Character owner);
    bool TryConsumeReroll(Character owner);
}
