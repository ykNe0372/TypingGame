using System.Collections.Generic;
using UnityEngine;

public class StatusManager {
    private readonly Character _owner;
    private readonly List<StatusEffectInstance> _effects = new();
    private readonly List<StatusEffectInstance> _removeQueue = new();
    private readonly Dictionary<StatusEffectType, float> _inflictionBonus = new();

    public List<StatusEffectInstance> Effects => _effects;

    public StatusManager(Character owner) {
        _owner = owner;
    }

    // 状態異常付与の試行
    public bool TryApply(Character attacker, StatusEffectData data) {
        float bonus = _inflictionBonus.GetValueOrDefault(data.type, 0f);
        float chance = StatusCalculator.CalculateChance(attacker, _owner, data, bonus);

        if (UnityEngine.Random.value < chance) {
            Apply(data, attacker);
            _inflictionBonus[data.type] = 0f;
            return true;
        } else {
            float rate = attacker.GetFinalStatus(StatusType.StatusInflictionRate) / 100f;  // 付与上昇率を (StatusInflictionRate) % アップ
            bonus += data.accumulationPerFail * (1f + rate);
            bonus = Mathf.Min(bonus, data.maxBonus);    // 上昇率が 100% を超えないように

            _inflictionBonus[data.type] = bonus;
            return false;
        }
    }

    // 状態異常を実際に付与
    private void Apply(StatusEffectData data, Character source) {
        var existing = Get(data.type);
        if (existing != null) {
            if (existing.Data.behaviour.OnReapply(_owner, source, data)) return;   // true が返れば新規付与しない
        }

        var instance = new StatusEffectInstance(data, source);
        _effects.Add(instance);
        data.behaviour.OnApply(_owner, instance);
    }

    // 状態異常の更新
    public void Update(float deltaTime) {
        for (int i=_effects.Count-1; i>=0; --i) {
            var e = _effects[i];
            e.Data.behaviour.OnUpdate(_owner, e, deltaTime);
            e.RemainingTime -= deltaTime;

            if (e.RemainingTime <= 0f) {
                e.Data.behaviour.OnRemove(_owner, e);
                _effects.RemoveAt(i);
            }
        }
        ProcessRemoveQueue();
    }

    public StatusEffectInstance Get(StatusEffectType type) {
        return _effects.Find(x => x.Data.type == type);
    }

    // 削除予約（ループ中に変化させないため）
    public void RequestRemoveStatus(StatusEffectInstance instance) {
        if (!_removeQueue.Contains(instance)) _removeQueue.Add(instance);
    }

    // 削除を実行
    private void ProcessRemoveQueue() {
        foreach (var instance in _removeQueue) {
            if (_effects.Remove(instance)) instance.Data.behaviour.OnRemove(_owner, instance);
        }
        _removeQueue.Clear();
    }
}