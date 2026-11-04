using System;
using System.Collections.Generic;
using UnityEngine;

// 効果実装で共通して使う処理
public static class EffectUtility {
    private static readonly StatusEffectType[] AllStatusEffectTypes = (StatusEffectType[])Enum.GetValues(typeof(StatusEffectType));

    public static bool IsAlive(Character character) {
        return character != null && !character.IsDead;
    }

    public static bool Roll(float percent) {
        return UnityEngine.Random.value < percent / 100f;
    }

    // 最大値の percent% を整数で返す（percent > 0 なら最低 1）
    public static int PercentOf(float max, float percent) {
        if (percent <= 0f) return 0;
        return Mathf.Max(1, Mathf.FloorToInt(max * percent / 100f));
    }

    // 最大 HP / MP の割合で回復
    public static void RecoverPercent(Character target, float hpPercent, float mpPercent, float multiplier = 1f) {
        if (!IsAlive(target)) return;

        target.RecoverHP(PercentOf(target.MaxHP, hpPercent * multiplier));
        target.RecoverMP(PercentOf(target.MaxMP, mpPercent * multiplier));
    }

    // 属性 → 対応する状態異常
    public static bool TryGetStatusOfElement(ElementType element, out StatusEffectType statusType) {
        switch (element) {
            case ElementType.Fire: statusType = StatusEffectType.Burn; return true;
            case ElementType.Ice: statusType = StatusEffectType.Freeze; return true;
            case ElementType.Lightning: statusType = StatusEffectType.Shock; return true;
            default: statusType = default; return false;
        }
    }

    // 何らかの状態異常にかかっているか（source を指定するとその付与者のものに限定）
    public static bool HasAnyStatus(Character target, Character source = null) {
        if (target == null) return false;

        foreach (var type in AllStatusEffectTypes) {
            var status = target.GetStatus(type);
            if (status == null) continue;
            if (source == null || status.Source == source) return true;
        }
        return false;
    }

    // source が付与した状態異常のうち、最初に見つかったもの
    // 複数の状態異常で同じ補正が重複しないようにする判定に使う
    public static StatusEffectInstance GetFirstStatusFrom(Character target, Character source) {
        if (target == null) return null;

        foreach (var type in AllStatusEffectTypes) {
            var status = target.GetStatus(type);
            if (status != null && status.Source == source) return status;
        }
        return null;
    }

    // このダメージで HP が 0 になるか（Character.TakeDamage と同じ丸め方で判定）
    public static bool IsLethal(Character target, DamageContext ctx) {
        int damage = Mathf.FloorToInt(Mathf.Max(0f, ctx.FinalDamage));
        return target.CurrentHP - damage <= 0f;
    }

    // HP が 1 残るようにダメージを抑える
    public static void ClampToSurvive(Character target, DamageContext ctx) {
        ctx.FinalDamage = Mathf.Max(0f, Mathf.Ceil(target.CurrentHP) - 1f);
    }

    // BuffModifier のリストを補正値に反映する（scale 倍して適用）
    // Adding: value * scale を加算 / Multiplier: 1 + (value - 1) * scale を乗算（1.05 を 3 倍 → 1.15）
    public static void ApplyModifiers(IReadOnlyList<BuffModifier> modifiers, StatusType type, float scale, ref float add, ref float multiplier) {
        if (modifiers == null || scale == 0f) return;

        for (int i = 0; i < modifiers.Count; ++i) {
            var modifier = modifiers[i];
            if (modifier == null || modifier.StatusType != type) continue;

            switch (modifier.BuffType) {
                case BuffType.Adding:
                    add += modifier.Value * scale;
                    break;
                case BuffType.Multiplier:
                    multiplier *= 1f + (modifier.Value - 1f) * scale;
                    break;
            }
        }
    }

    public static bool ContainsStatus(IReadOnlyList<BuffModifier> modifiers, StatusType type) {
        if (modifiers == null) return false;
        for (int i = 0; i < modifiers.Count; ++i) {
            if (modifiers[i] != null && modifiers[i].StatusType == type) return true;
        }
        return false;
    }

    public static BuffModifier Modifier(StatusType statusType, BuffType buffType, float value) {
        return new BuffModifier { StatusType = statusType, BuffType = buffType, Value = value };
    }
}
