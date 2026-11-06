using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private float _freezeDelay = 2f;
    [SerializeField] private CombatState _state = CombatState.Playing;
    [SerializeField] private StatusEffectResolver _resolver;
    [SerializeField] private BattlePresentation _battlePresentation;

    private Character _player;
    private List<Character> _enemies;
    private int _currentTargetIndex = 0;
    private float _gameOverTimer;
    private BattleContext _battleContext;
    private BattleType _battleType;
    private readonly List<BattleModifierBase> _activeModifiers = new();

    public CombatState State => _state;
    public BattleType BattleType => _battleType;

    public event Action<BattleType> OnBattleVictory;
    public event Action OnBattleDefeat;

    private void Update() {
        if (_player == null) return;  // 戦闘開始前

        if (_battleType == BattleType.Normal && _state == CombatState.Playing) {  // 戦闘終了後はハプニングを止める
            foreach (var modifier in _activeModifiers) modifier.OnUpdate(_battleContext, Time.deltaTime);
        }

        _player.TickBuffs(Time.deltaTime);
        if (_state == CombatState.Playing) _player.Hooks.BattleUpdate(Time.deltaTime);  // 戦闘中の継続効果
        CheckBattleResult();

        if (_state == CombatState.GameOver) {
            _gameOverTimer += Time.deltaTime;
            if (_gameOverTimer >= _freezeDelay) Time.timeScale = 0f;
        }
    }

    public void SetBattleModifiers(List<BattleModifierBase> modifiers) {
        _activeModifiers.Clear();
        _activeModifiers.AddRange(modifiers);
    }

    private void RegisterCombatCharacters() {
        // プレイヤーを表示イベントの監視対象に登録
        _battlePresentation.Register(_player);

        // 敵を表示イベントの監視対象に登録
        foreach (var enemy in _enemies) {
            _battlePresentation.Register(enemy);
        }
    }

    public void BeginBattle(Character player, List<Character> enemies, BattleType battleType) {

        // 前戦闘の OnDead イベント購読を解除
        if (_enemies != null) {
            foreach (var enemy in _enemies) {
                if (enemy != null) enemy.OnDead -= HandleEnemyDead;
            }
        }
        if (_player != null) _player.OnDead -= HandlePlayerDead;

        _player = player;
        _enemies = new List<Character>(enemies);
        _battleType = battleType;
        _state = CombatState.Playing;

        _currentTargetIndex = 0;
        _player.SetCombatSystem(this);
        _player.OnDead += HandlePlayerDead;
        RefreshBattleContext();
        RegisterCombatCharacters();

        // 新規敵が OnDead イベントを購読し直す
        foreach (var enemy in _enemies) {
            enemy.Initialize();
            enemy.OnDead += HandleEnemyDead;
        }

        // ハプニング効果の開始処理（以前は Start() で最初の1回しか呼ばれていなかった）
        if (_battleType == BattleType.Normal) {
            foreach (var modifier in _activeModifiers) modifier.OnBattleStart(_battleContext);
        }

        _player.Hooks.BattleStart(_battleContext);  // 戦闘開始時の効果
    }

    private void RefreshBattleContext() {
        _battleContext = new BattleContext {
            CombatSystem = this,
            Player = _player,
            Enemies = _enemies
        };
    }

    private void HandlePlayerDead(Character player) {
        Debug.Log("Player Dead");
    }

    private void HandleEnemyDead(Character enemy) {
        Debug.Log($"{enemy.name} defeated");
        ValidateTarget();
    }

    // 撃破時にターゲットを自動に切り替える
    // _currentTargetIndex は「生存している敵」の中での番号
    private void ValidateTarget() {
        if (_player == null) return;

        int aliveCount = GetEnemies(_player).Count;
        if (aliveCount == 0) {
            _currentTargetIndex = 0;
            return;
        }

        _currentTargetIndex %= aliveCount;
    }

    public void RequestAttack() {
        if (!CanAttack(_player)) return;
        RequestPlayerAttack();
    }

    private bool CanAttack(Character actor) {
        if (_state != CombatState.Playing) return false;
        return actor.CanAct();
    }

    private AttackContext CreateContext(Character attacker, SkillData skill) {
        ElementType element = attacker.CurrentElement;

        return AttackContextFactory.CreateNormalAttack(
            attacker,
            GetTargets(attacker, skill.targetType),
            skill,
            element,
            _resolver.Get(element)
        );
    }

    private void RequestPlayerAttack() {
        var ctx = CreateContext(_player, _player.CurrentSkill);

        if (ctx.Element != ElementType.None) {
            if (!_player.TryConsumeMP(ctx.Skill.MPCost)) {
                Debug.Log("Change to None Element due to lacking MP");
                ctx.Element = ElementType.None;
                ctx.StatusEffect = null;  // 無属性になったので状態異常も付与しない
            } else {
                ctx.IsMagical = true;  // MP を消費した攻撃は魔法攻撃力を参照
            }
        }
        _player.TriggerAttack(ctx);
    }

    public void RequestBonusAttack(int chain) {
        BonusAttackData bonus = _player.GetBonusAttack(chain);
        if (bonus == null) return;
        List<Character> targets = GetTargets(_player, bonus.targetType);
        if (targets.Count == 0) return;

        var ctx = AttackContextFactory.CreateBonusAttack(_player, targets, bonus);
        _player.TriggerAttack(ctx);
        _player.Hooks.BonusAttack();  // ノーミスボーナス発動時の効果
    }

    // タイプミスを効果側に通知し、無効化・文章切り替えなどの結果を返す
    public TypoContext NotifyTypo() {
        if (_player == null || _state != CombatState.Playing) return new TypoContext();
        return _player.Hooks.Typo();
    }

    // 1文打ち切ったことを効果側に通知
    public void NotifyWordCompleted() {
        if (_player == null || _state != CombatState.Playing) return;
        _player.Hooks.WordCompleted();
    }

    public void AddSpecialCharge(int amount) {
        if (_player == null || _state != CombatState.Playing) return;
        _player.AddSpecialCharge(amount);
    }

    public void RequestChargedSpecialAttack() {
        if (_player == null || _state != CombatState.Playing) return;

        int level = _player.SpecialLevel;
        if (level <= 0) return;

        RequestSpecialAttack(level);
        _player.ClearSpecialCharge();
    }

    public void RequestSpecialAttack(int level) {
        if (_player == null || _state != CombatState.Playing) return;

        SpecialAttackData special = _player.GetSpecialAttack(level);
        if (special == null) return;
        List<Character> targets = GetTargets(_player, special.targetType);
        if (targets.Count == 0) return;

        var ctx = AttackContextFactory.CreateSpecialAttack(_player, targets, special);
        _player.TriggerAttack(ctx);
    }

    private AttackContext CreateEnemyContext(Character attacker, SkillData skill, EnemyData data) {
        ElementType element = EnemyElementResolver.Resolve(data);

        return AttackContextFactory.CreateNormalAttack(
            attacker,
            GetTargets(attacker, skill.targetType),
            skill,
            element,
            _resolver.Get(element)
        );
    }

    public void RequestEnemyAttack(Character enemy, SkillData skill, EnemyData data) {
        if (!CanAttack(enemy)) return;

        var ctx = CreateEnemyContext(enemy, skill, data);
        enemy.TriggerAttack(ctx);
    }

    public List<Character> GetEnemies(Character requester) {
        List<Character> targets;
    
        if (requester == _player) targets = _enemies;
        else targets = new List<Character>{ _player };  // 仮でプレイヤーだけ、分身など味方 NPC が出てきた時は変更

        return targets.Where(x => x != null && !x.IsDead).ToList();
    }

    public void MoveTargetLeft() {
        if (_player == null) return;
        var alive = GetEnemies(_player);
        if (alive.Count == 0) return;

        _currentTargetIndex = (_currentTargetIndex - 1 + alive.Count) % alive.Count;
        Debug.Log($"Target: {alive[_currentTargetIndex].name}");
    }

    public void MoveTargetRight() {
        if (_player == null) return;
        var alive = GetEnemies(_player);
        if (alive.Count == 0) return;

        _currentTargetIndex = (_currentTargetIndex + 1) % alive.Count;
        Debug.Log($"Target: {alive[_currentTargetIndex].name}");
    }

    private List<Character> GetTargets(Character attacker, SkillTargetType targetType) {
        var enemies = GetEnemies(attacker);
        if (enemies.Count == 0 || targetType != SkillTargetType.Single) return enemies;

        // 選択中ターゲットはプレイヤー用。敵の単体攻撃は先頭（現状はプレイヤーのみ）を狙う
        if (attacker != _player) return new List<Character> { enemies[0] };

        ValidateTarget();
        return new List<Character> { enemies[_currentTargetIndex] };
    }

    // 勝敗判定
    private void CheckBattleResult() {
        if (_state != CombatState.Playing) return;
        
        bool allEnemyDead = true;
        foreach (var enemy in _enemies) {
            if (!enemy.IsDead) {
                allEnemyDead = false;
                break;
            }
        }

        if (allEnemyDead) {
            HandleVictory();
            return;
        }

        if (_player.IsDead) HandleGameOver();
    }

    private void HandleVictory() {
        if (_state != CombatState.Playing) return;

        _state = CombatState.Victory;
        _player.OnBattleEnd();
        _player.Hooks.BattleEnd(new BattleEndContext(_battleType, true));
        NotifyModifiersBattleEnd();

        OnBattleVictory?.Invoke(_battleType);
    }

    private void HandleGameOver() {
        if (_state != CombatState.Playing) return;

        _state = CombatState.GameOver;
        _player.Hooks.BattleEnd(new BattleEndContext(_battleType, false));
        NotifyModifiersBattleEnd();

        OnBattleDefeat?.Invoke();
    }

    // ハプニング効果の終了処理（OnBattleEnd が呼ばれていなかった）
    private void NotifyModifiersBattleEnd() {
        if (_battleType != BattleType.Normal) return;
        foreach (var modifier in _activeModifiers) modifier.OnBattleEnd(_battleContext);
    }
}
