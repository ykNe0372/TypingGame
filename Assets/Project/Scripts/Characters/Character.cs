using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterBaseStatus _baseStatus;   // 基礎ステータス
    [SerializeField] private RelicData _relic;                  // レリック
    [SerializeField] private List<SkillData> _skills;
    [SerializeField] private List<BonusAttackData> _bonusAttacks;
    [SerializeField] private List<SpecialAttackData> _specialAttacks;
    [SerializeField] private ElementType _currentElement = ElementType.None;
    [SerializeField] private CombatSystem _combatSystems;
    

    private StatusManager _statusManager;
    private readonly ItemInventory _itemInventory = new();
    private readonly SubItemInventory _subInventory = new();
    private readonly List<RelicEffect> _relicEffects = new();       // レリック効果
    private readonly List<Effect> _growthItemEffects = new();       // 強化アイテムによるステータス上昇
    private readonly List<BuffInstance> _tempBuffs = new();         // 一時的なバフデバフ
    private readonly List<OnAttackEffect> _attackEffects = new();   // 攻撃時に発動する効果（ヒット時に〇〇する系）
    private readonly List<OnDamagedEffect> _damagedEffects = new();  // 被弾時に発動する効果（被弾時に〇〇する系）
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
    public ItemInventory Inventory => _itemInventory;
    public SubItemInventory SubInventory => _subInventory;
    public IReadOnlyList<OnAttackEffect> AttackEffects => _attackEffects;

    public event Action<Character> OnDead;

    private void Awake() {
        _statusManager = new StatusManager(this);

        foreach (var item in _startItems) AddItem(item);  // 初期アイテム（多分デバッグのみ）
        
        BuildEffectList();
        InitializeHP();
        InitializeMP();
    }

    private void Update() {
        UpdateMPRegeneration();
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
        if (_itemInventory == null) return;
        
        _growthItemEffects.Clear();
        _attackEffects.Clear();
        _relicEffects.Clear();

        foreach (var item in _itemInventory.Items) {
            foreach (var effect in item.GetEffects()) {
                if (effect is OnAttackEffect attackEffect) _attackEffects.Add(attackEffect);
                else if (effect is OnDamagedEffect damageEffect) _damagedEffects.Add(damageEffect);
                else _growthItemEffects.Add(effect);
            }
        }

        if (_relic != null) {
            foreach (var effect in _relic.GetEffects()) _relicEffects.Add(effect);
        }
    }

    // レリック装備用
    public void EquipRelic(RelicData relic) {
        _relic = relic;
        Debug.Log($"[Relic] Get Relic: {_relic.RelicName}");
    }

    public bool HasRelic() {
        return _relic != null;
    }

    public void AddItem(GrowthItem item) {
        if (_itemInventory.HasSpace()) {
            _itemInventory.AddItem(item);
            BuildEffectList();  // 効果一覧を再構築
            Debug.Log($"[Inventory] Get Item: {item.ItemName}");
            return;
        }

        if (_subInventory.HasSpace()) {
            _subInventory.AddItem(item);
            Debug.Log($"[SubInventory] Get Item: {item.ItemName}");
            return;
        }

        Debug.Log($"Item Discarded: {item.ItemName}");
    }

    public void RemoveItem(GrowthItem item) {
        if (_subInventory.RemoveItem(item)) {
            BuildEffectList();
            Debug.Log($"[SubInventory] Remove Item: {item.ItemName}");
            return;
        }
        if (_itemInventory.RemoveItem(item)) {
            BuildEffectList();
            Debug.Log($"[Inventory] Remove Item: {item.ItemName}");
            return;
        }
    }

    public int CountItemsByRarity(ItemRarity rarity) {
        return _itemInventory.CountByRarity(rarity) + _subInventory.CountByRarity(rarity);
    }

    // 指定レアリティのアイテムを取得
    public List<GrowthItem> GetItemsByRarity(ItemRarity rarity, int count) {
        List<GrowthItem> result = new();
        foreach (var item in _subInventory.GetItemsByRarity(rarity)) {
            result.Add(item);
            if (result.Count >= count) return result;
        }

        // 不足分はメインインベントリから
        foreach (var item in _itemInventory.GetItemsByRarity(rarity)) {
            result.Add(item);
            if (result.Count >= count) return result;
        }

        return result;
    }

    // 仮実装（本実装でアイテム一覧を返してプレイヤーが選べるようにする）
    public GrowthItem SelectPaymentItem() {
        if (_subInventory.Count > 0) return _subInventory.Items[0];
        if (_itemInventory.Count > 0) return _itemInventory.Items[0];
        return null;
    }

    public void AddBuff(BuffData data) {
        _tempBuffs.Add(new BuffInstance(data));

        // Debug.Log($"[DEBUG] PhysicalAttack: {GetFinalStatus(StatusType.PhysicalAttack)}");
        // Debug.Log($"[DEBUG] MagicAttack: {GetFinalStatus(StatusType.MagicAttack)}");
    }

    private void RemoveExpiredBuffs() {
        for (int i=_tempBuffs.Count-1; i>=0; --i) {
            if (!_tempBuffs[i].Duration.IsExpired) continue;

            Debug.Log($"Buff Expired: {_tempBuffs[i].Data.BuffName}");
            _tempBuffs.RemoveAt(i);
        }
    }

    public void OnBattleEnd() {
        foreach (var buff in _tempBuffs) buff.Duration.OnBattleEnd();  // BuffDuration に通知
        RemoveExpiredBuffs();
    }

    public void TickBuffs(float deltaTime) {
        foreach (var buff in _tempBuffs) buff.Duration.Tick(deltaTime);
        RemoveExpiredBuffs();
    }

    public void NotifyAttack() {
        foreach (var buff in _tempBuffs) buff.Duration.OnAttack();
        RemoveExpiredBuffs();
    }

    public void NotifyDamaged() {
        foreach (var buff in _tempBuffs) buff.Duration.OnDamaged();
        RemoveExpiredBuffs();
    }

    public float GetBuffTotal(StatusType statusType) {
        float totalMultiplier = 1f;

        foreach (var buff in _tempBuffs) {
            foreach (var modifier in buff.Data.Modifiers) {
                if (modifier.StatusType == statusType) totalMultiplier += modifier.Value;
            }
        }

        return totalMultiplier;
    }

    public float GetFinalStatus(StatusType type) {
        float baseValue = _baseStatus.GetStatus(type);
        float bonus = 0f;
        
        foreach (var effect in _growthItemEffects) bonus += effect.GetStatusBonus(type);
        foreach (var effect in _relicEffects) bonus += effect.GetStatusBonus(type);
        
        float value =  baseValue + bonus;
        value *= GetBuffTotal(type);

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

    public SpecialAttackData GetSpecialAttack(int level) {
        if (_specialAttacks.Count == 0) return null;

        return _specialAttacks[level - 1];
    }

    public void TriggerAttack(AttackContext ctx) {
        if (_isDead) return;

        AttackExecutor.Execute(ctx);
    }

    // ダメージ適応（仮）
    public void TakeDamage(DamageContext ctx) {
        if (!ctx.IsEnvironmentDamage)
            foreach (var effect in _damagedEffects) effect.OnDamage(ctx);

        for (int i=_statusManager.Effects.Count-1; i>=0; --i) {  // 要素を削除しても大丈夫なように逆順にする 
            var status = _statusManager.Effects[i];
            status.Data.behaviour.OnDamage(this, status, ctx);
        }

        int damage = Mathf.FloorToInt(Mathf.Max(0f, ctx.FinalDamage));
        _currentHP -= damage;
        _currentHP = Mathf.Max(0, _currentHP);

        if (_currentHP <= 0) Die();
    
        Debug.Log($"{name} HP: {_currentHP}/{MaxHP}");
    }

    public void TakePercentDamage(float percent) {
        float damage = MaxHP * (percent / 100f);
        if (_currentHP <= damage) return;

        var dmgCtx = DamageContextFactory.CreateFixed(null, this, damage, true);
        TakeDamage(dmgCtx);
    }

    public void ConsumePercentMP(float percent) {
        float consume = MaxMP * (percent / 100f);
        if (_currentMP < consume) return;

        TryConsumeMP(Mathf.FloorToInt(consume));
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

    public bool TryApplyStatus(Character attacker, StatusEffectData data) {
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

    public float GetCurrentHPRatio() {
        return _currentHP / GetFinalStatus(StatusType.MaxHP);
    }

    public float GetCurrentMPRatio() {
        return _currentMP / GetFinalStatus(StatusType.MaxMP);
    }

    // MP の自然回復効果
    private void UpdateMPRegeneration() {
        float regen = GetFinalStatus(StatusType.MPRegen);
        if (regen <= 0f) return;

        float interval = 1f / regen;
        _regenTimer += Time.deltaTime;

        if (_regenTimer >= interval) {
            RecoverMP(1);
            _regenTimer -= interval;
        }
    }

    public void RecoverHP(int amount) {
        if (amount <= 0) return;

        _currentHP += amount;
        _currentHP = Mathf.Min(_currentHP, MaxHP);
    }

    public void RecoverMP(int amount) {
        if (amount <= 0) return;

        _currentMP += amount;
        _currentMP = Mathf.Min(_currentMP, MaxMP);
    }

    public bool TryConsumeMP(int amount) {
        if (_currentMP < amount) return false;
        _currentMP -= amount;
        Debug.Log($"{name} HP: {_currentMP}/{MaxMP}");
        return true;
    }

    // ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭  DEBUG MODE  ▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭▬▭

    [SerializeField] private List<GrowthItem> _startItems = new();
    
    public void Debug_PrintItems() {
        Debug.Log("xxx--- ITEM LIST ---xxx");
        foreach (var item in _itemInventory.Items) Debug.Log($"{item.ItemName} [{item.Rarity}]");
    }
}