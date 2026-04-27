using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public enum ElementType {
    None,
    Fire,
    Ice,
    Lightning
}

public class Character : MonoBehaviour {

    [SerializeField] private CharacterBaseStatus _baseStatus;   // 基礎ステータス
    [SerializeField] private List<GrowthItem> _items = new();   // 強化アイテム
    [SerializeField] private List<SkillData> _skills;
    [SerializeField] private ElementType _currentElement = ElementType.None;
    [SerializeField] private List<StatusEffectInstance> _statusEffects = new();
    [SerializeField] private List<StatusEffectInstance> _removeQuere = new();

    [SerializeField] private TextMeshProUGUI _element;
    [SerializeField] private TextMeshProUGUI _effectText;
    

    private List<Effect> _passiveEffects = new();           // ステータス変更系
    private List<OnAttackEffect> _attackEffects = new();    // 攻撃変更（連撃）系
    private List<OnDamageEffect> _damageEffects = new();    // ダメージ計算
    private Dictionary<StatusEffectType, float> _inflictionBonus = new();
    private float _currentHP;
    private float _currentMP;
    private float _regenTimer = 0f;
    private int _currentSkillIndex = 0;
    private bool _isFrozen;

    public float MaxHP => GetFinalStatus(StatusType.MaxHP);
    public float MaxMP => GetFinalStatus(StatusType.MaxMP);
    public bool IsFrozen => _isFrozen;
    public SkillData CurrentSkill => _skills[_currentSkillIndex];
    public ElementType CurrentElement => _currentElement;

    private void Awake() {
        BuildEffectList();
        InitializeHP();
        InitializeMP();
    }

    private void Update() {
        RecoverMP();
        UpdateStatusEffects();
        ProcessRemoveQuere();
    }

    public void InitializeHP() {
        _currentHP = MaxHP;
    }

    public void InitializeMP() {
        _currentMP = Mathf.FloorToInt(MaxMP);
    }

    // 強化アイテム一覧
    private void BuildEffectList() {
        _passiveEffects.Clear();
        _attackEffects.Clear();

        foreach (var item in _items) {
            foreach (var effect in item.GetEffects()) {
                if (effect is OnAttackEffect attackEffect) _attackEffects.Add(attackEffect);
                else if (effect is OnDamageEffect damageEffect) _damageEffects.Add(damageEffect);
                else _passiveEffects.Add(effect);
            }
        }
    }

    public float GetFinalStatus(StatusType type) {
        float baseValue = _baseStatus.GetStatus(type);
        float bonus = 0f;
        
        foreach (var effect in _passiveEffects) bonus += effect.GetStatusBonus(type);
        return baseValue + bonus;
    }

    public void TriggerAttack(AttackContext ctx) {
        foreach (var effect in _attackEffects) effect.OnAttack(ctx);
        for (int i=0; i<ctx.AttackCount; ++i) ExecuteAttack(ctx);
    }

    private void ExecuteAttack(AttackContext ctx) {
        foreach (var target in ctx.Targets) {
            var dmgCtx = new DamageContext {
                Attacker = this,
                Target = target,
                BaseDamage = DamageCalculator.Calculate(this, target) // 基礎ダメージ計算
            };
            dmgCtx.FinalDamage = Mathf.RoundToInt(dmgCtx.BaseDamage * CurrentSkill.powerMultiplier);
            target.TakeDamage(dmgCtx);   // 被弾処理
            if (ctx.StatusEffect != null) {
                target.TryApplyStatus(ctx.Attacker, ctx.StatusEffect);  // 状態異常付与
            }
        }
    }

    // ダメージ適応（仮）
    public void TakeDamage(DamageContext ctx) {
        foreach (var effect in _damageEffects) effect.OnDamage(ctx);
        for (int i=_statusEffects.Count-1; i>=0; --i) {  // 要素を削除しても大丈夫なように逆順にする 
            var status = _statusEffects[i];
            status.Data.behaviour.OnDamage(this, status, ctx);
        }

        int damage = Mathf.FloorToInt(Mathf.Max(0f, ctx.FinalDamage));
        _currentHP -= damage;
        _currentHP = Mathf.Max(0, _currentHP);
    
        Debug.Log($"{name} HP: {_currentHP}/{MaxHP}");;
    }
    
    // 数字キーで技を変える
    public void ChangeSkill(int index) {
        if (index < 0 || index >= _skills.Count) return;
        _currentSkillIndex = index;

        Debug.Log($"Skill Changed: {CurrentSkill.skillName}");
    }

    // 属性を次に回す（無→炎→氷→電→無→...）
    public void CycleElement() {
        int next = ((int)_currentElement + 1) % Enum.GetValues(typeof(ElementType)).Length;
        _currentElement = (ElementType)next;

        Debug.Log($"Element Changed: {_currentElement}");
        _element.text = $"{_currentElement}";   // 仮表示
    }

    private void UpdateStatusEffects() {
        for (int i=_statusEffects.Count-1; i>=0; --i) {
            var effect = _statusEffects[i];
            effect.Data.behaviour.OnUpdate(this, effect, Time.deltaTime); // 挙動を更新
            effect.RemainingTime -= Time.deltaTime;

            // 終了処理
            if (effect.RemainingTime <= 0f) {
                effect.Data.behaviour.OnRemove(this, effect);
                _statusEffects.RemoveAt(i);
            }
        }
    }

    private bool TryApplyStatus(Character attacker, StatusEffectData data) {
        float bonus = _inflictionBonus.GetValueOrDefault(data.type, 0f);

        float infliction = attacker.GetFinalStatus(StatusType.StatusInfliction) / 100f;  // 付与確率を (StatusInfliction) % アップ
        float resistance = GetFinalStatus(StatusType.StatusResistance) / 100f;           // 付与確率を (StatusResistance) % ダウン
        float chance = (data.baseChance + bonus) * (1 + infliction) * (1f - resistance);

        if (UnityEngine.Random.value < chance) {
            ApplyStatus(data, attacker);
            _inflictionBonus[data.type] = 0f;
            return true;
        } else {
            float rate = attacker.GetFinalStatus(StatusType.StatusInflictionRate) / 100f;  // 付与上昇率を (StatusInflictionRate) % アップ
            bonus += data.accumulationPerFail * (1 + rate);
            bonus = Mathf.Min(bonus, data.maxBonus);    // 上昇率が 100% を超えないように

            _inflictionBonus[data.type] = bonus;
            return false;
        }
    }

    private void ApplyStatus(StatusEffectData data, Character source) {
        if (data.type == StatusEffectType.Freeze) {
            var existing = GetStatus(StatusEffectType.Freeze);
            if (existing != null) {
                // 既に凍結している → 重ねがけせずに解除
                RequestRemoveStatus(existing);
                Debug.Log("Freeze Shattered");
                return;  // 新規付与しない
            }
        }

        var instance = new StatusEffectInstance(data, source);
        _statusEffects.Add(instance);
        data.behaviour.OnApply(this, instance);

        _effectText.text = $"{data.type}!";  // 仮表示
    }

    public StatusEffectInstance GetStatus(StatusEffectType type) {
        return _statusEffects.Find(x => x.Data.type == type);
    }

    public void SetFrozen(bool value) {
        _isFrozen = value;
    }

    public void RequestRemoveStatus(StatusEffectInstance instance) {
        if (!_removeQuere.Contains(instance)) _removeQuere.Add(instance);
    }

    private void ProcessRemoveQuere() {
        foreach (var instance in _removeQuere) {
            if (_statusEffects.Remove(instance)) instance.Data.behaviour.OnRemove(this, instance);
        }
        _removeQuere.Clear();
    }

    public bool TryConsumeMP(int amount) {
        if (_currentMP < amount) return false;
        _currentMP -= amount;
        return true;
    }

    private void RecoverMP() {
        float regen = GetFinalStatus(StatusType.MPRegen);
        if (regen <= 0f) return;

        float interval = 1f / regen;
        _regenTimer += Time.deltaTime;

        if (_regenTimer >= interval) {
            _currentMP += 1;
            _regenTimer -= interval;
            _currentMP = Mathf.Min(_currentMP, MaxMP);
        }
    }
}