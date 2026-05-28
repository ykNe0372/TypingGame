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
    [SerializeField] private RelicData _relic;                  // レリック
    [SerializeField] private List<GrowthItem> _items = new();   // 強化アイテム
    [SerializeField] private List<SkillData> _skills;
    [SerializeField] private List<BonusAttackData> _bonusAttacks;
    [SerializeField] private ElementType _currentElement = ElementType.None;
    [SerializeField] private CombatSystem _combatSystems;
    

    private StatusManager _statusManager;
    private readonly List<RelicEffect> _relicEffects = new();        // レリック効果
    private readonly List<Effect> _passiveEffects = new();           // ステータス変更系
    private readonly List<OnAttackEffect> _attackEffects = new();    // 攻撃変更（連撃）系
    private readonly List<OnDamageEffect> _damageEffects = new();    // ダメージ計算
    private float _currentHP;
    private float _currentMP;
    private bool _isDead;
    private float _regenTimer = 0f;
    private int _currentSkillIndex = 0;

    public float MaxHP => GetFinalStatus(StatusType.MaxHP);
    public float MaxMP => GetFinalStatus(StatusType.MaxMP);
    public bool IsDead => _isDead;
    public SkillData CurrentSkill => _skills[_currentSkillIndex];
    public ElementType CurrentElement => _currentElement;

    public event Action<Character> OnDead;

    private void Awake() {
        _statusManager = new StatusManager(this);
        
        BuildEffectList();
        InitializeHP();
        InitializeMP();
    }

    private void Update() {
        RecoverMP();
        UpdateStatusEffects();
    }

    public void Initialize() {
        InitializeHP();
        InitializeMP();
        _isDead = false;
    }

    private void InitializeHP() {
        _currentHP = MaxHP;
    }

    private void InitializeMP() {
        _currentMP = Mathf.FloorToInt(MaxMP);
    }

    // 強化アイテム一覧、レリック効果
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

        if (_relic != null) {
            foreach (var effect in _relic.effects) _relicEffects.Add(effect);
        }
    }

    public void AddItem(GrowthItem item) {
        _items.Add(item);
        BuildEffectList();  // 効果一覧を再構築
        Debug.Log($"Get Item: {item.ItemName}");
    }

    public float GetFinalStatus(StatusType type) {
        float baseValue = _baseStatus.GetStatus(type);
        float bonus = 0f;
        
        foreach (var effect in _passiveEffects) bonus += effect.GetStatusBonus(type);
        foreach (var effect in _relicEffects) bonus += effect.GetStatusBonus(type);
        float value =  baseValue + bonus;

        foreach (var s in _statusManager.Effects) {
            value = s.Data.behaviour.ModifyStat(type, value, s);
        }

        return value;
    }

    public BonusAttackData GetBonusAttack(int chain) {
        if (_bonusAttacks.Count == 0) return null;

        int index = chain % _bonusAttacks.Count;
        return _bonusAttacks[index];
    }

    public void TriggerAttack(AttackContext ctx) {
        if (_isDead) return;

        foreach (var effect in _attackEffects) effect.OnAttack(ctx);
        foreach (var attack in ctx.AttackInstances) ExecuteAttack(ctx, attack);
    }

    private void ExecuteAttack(AttackContext ctx, AttackInstance attack) {
        foreach (var target in ctx.Targets) {
            var dmgCtx = DamageContextFactory.CreateAttack(this, target);

            dmgCtx.BaseDamage *= attack.PowerMultiplier;
            dmgCtx.FinalDamage = dmgCtx.BaseDamage * CurrentSkill.powerMultiplier;
            CriticalCalculator.Apply(dmgCtx);
            
            target.TakeDamage(dmgCtx);   // 被弾処理
            if (ctx.StatusEffect != null) {
                target.TryApplyStatus(ctx.Attacker, ctx.StatusEffect);  // 状態異常付与
            }
        }
    }

    public void TriggerBonusAttack(List<Character> targets, BonusAttackData bonus) {
        ExecuteBonusAttack(targets, bonus);
    }

    private void ExecuteBonusAttack(List<Character> targets, BonusAttackData bonus) {
        foreach (var target in targets) {
            var dmgCtx = DamageContextFactory.CreateAttack(this, target);
            dmgCtx.FinalDamage = dmgCtx.BaseDamage * bonus.multiplier;
            CriticalCalculator.Apply(dmgCtx);
            Debug.Log("Bonus Atatck Executed.");

            target.TakeDamage(dmgCtx);
        }
    }

    // ダメージ適応（仮）
    public void TakeDamage(DamageContext ctx) {
        foreach (var effect in _damageEffects) effect.OnDamage(ctx);
        for (int i=_statusManager.Effects.Count-1; i>=0; --i) {  // 要素を削除しても大丈夫なように逆順にする 
            var status = _statusManager.Effects[i];
            status.Data.behaviour.OnDamage(this, status, ctx);
        }

        int damage = Mathf.FloorToInt(Mathf.Max(0f, ctx.FinalDamage));
        _currentHP -= damage;
        _currentHP = Mathf.Max(0, _currentHP);

        if (_currentHP <= 0) Die();
    
        Debug.Log($"{name} HP: {_currentHP}/{MaxHP}");;
    }

    private void Die() {
        if (_isDead) return;
        
        _isDead = true;
        OnDead?.Invoke(this);

        // TODO: 死亡アニメーション・死亡エフェクトの再生など
        gameObject.SetActive(false); // 仮実装
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
    }

    private void UpdateStatusEffects() {
        for (int i=_statusManager.Effects.Count-1; i>=0; --i) {
            var effect = _statusManager.Effects[i];
            effect.Data.behaviour.OnUpdate(this, effect, Time.deltaTime); // 挙動を更新
            effect.RemainingTime -= Time.deltaTime;

            // 終了処理
            if (effect.RemainingTime <= 0f) {
                effect.Data.behaviour.OnRemove(this, effect);
                _statusManager.Effects.RemoveAt(i);
            }
        }
    }

    private bool TryApplyStatus(Character attacker, StatusEffectData data) {
        return _statusManager.TryApply(attacker, data);
    }

    public StatusEffectInstance GetStatus(StatusEffectType type) {
        return _statusManager.Get(type);
    }

    // 削除予約（ループ中に変化させないため）
    public void RequestRemoveStatus(StatusEffectInstance instance) {
        _statusManager.RequestRemoveStatus(instance);
    }

    public List<Character> GetCombatTargets(Character source) {
        return _combatSystems.GetEnemies(source);
    }

    // 行動可能か
    public bool CanAct() {
        foreach (var s in _statusManager.Effects) {
            if (s.Data.behaviour.ShouldBlockAction(this, s)) return false;
        }
        return true;
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