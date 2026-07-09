using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private float _freezeDelay = 2f;
    [SerializeField] private CombatState _state = CombatState.Playing;
    [SerializeField] private StatusEffectResolver _resolver;

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

    private void Start() {
        if (_battleType == BattleType.Normal) {
            foreach (var modifier in _activeModifiers) modifier.OnBattleStart(_battleContext);
        }

        _player.OnDead += HandlePlayerDead;
        foreach (var enemy in _enemies) enemy.OnDead += HandleEnemyDead;
    }

    private void Update() {
        if (_battleType == BattleType.Normal) {
            foreach (var modifier in _activeModifiers) modifier.OnUpdate(_battleContext, Time.deltaTime);
        }

        _player.TickBuffs(Time.deltaTime);
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

    public void BeginBattle(Character player, List<Character> enemies, BattleType battleType) {

        // 前戦闘の OnDead イベント購読を解除
        if (_enemies != null) {
            foreach (var enemy in _enemies) {
                if (enemy != null) enemy.OnDead -= HandleEnemyDead;
            }
        }

        _player = player;
        _enemies = new List<Character>(enemies);
        _battleType = battleType;
        _state = CombatState.Playing;

        RefreshBattleContext();

        // 新規敵が OnDead イベントを購読し直す
        foreach (var enemy in _enemies) {
            enemy.Initialize();
            enemy.OnDead += HandleEnemyDead;
        }
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
    private void ValidateTarget() {
        _enemies.Where(x => x != null && !x.IsDead);
        if (_enemies.Count == 0) return;

        _currentTargetIndex %= _enemies.Count;
    }

    public void RequestAttack() {
        if (!CanAttack(_player)) return;
        RequestPlayerAttack();
    }

    private bool CanAttack(Character actor) {
        if (_state != CombatState.Playing) return false;
        return actor.CanAct();
    }

    private AttackContext CreateContext(Character attacker) {
        var ctx = new AttackContext {
            Attacker = attacker,
            Targets = GetTargets(attacker, attacker.CurrentSkill.targetType),
            Element = attacker.CurrentElement,
            Skill = attacker.CurrentSkill,
            StatusEffect = _resolver.Get(attacker.CurrentElement)
        };
        ctx.AttackInstances.Add(new AttackInstance {
            PowerMultiplier = 1f
        });

        return ctx;
    }

    private void RequestPlayerAttack() {
        var ctx = CreateContext(_player);

        if (ctx.Element != ElementType.None) {
            if (!_player.TryConsumeMP(ctx.Skill.MPCost)) {
                Debug.Log("Change to None Element due to lacking MP");
                ctx.Element = ElementType.None;
            }
        }
        _player.TriggerAttack(ctx);
    }

    public void RequestBonusAttack(int chain) {
        var bonus = _player.GetBonusAttack(chain);
        var targets = GetTargets(_player, bonus.targetType);
        if (bonus == null || targets.Count == 0) return;

        _player.TriggerBonusAttack(targets, bonus);
    }

    public void RequestSpecialAttack(int level) {
        var special = _player.GetSpecialAttack(level);
        var targets = GetTargets(_player, special.targetType);
        if (special == null || targets.Count == 0) return;

        _player.TriggerSpecialAttack(targets, special);
    }

    public void RequestEnemyAttack(Character enemy) {
        if (!CanAttack(enemy)) return;

        var ctx = CreateContext(enemy);
        enemy.TriggerAttack(ctx);
    }

    public List<Character> GetEnemies(Character requester) {
        List<Character> targets;
    
        if (requester == _player) targets = _enemies;
        else targets = new List<Character>{ _player };  // 仮でプレイヤーだけ、分身など味方 NPC が出てきた時は変更

        return targets.Where(x => x != null && !x.IsDead).ToList();
    }

    public void MoveTargetLeft() {
        if (_enemies.Count == 0) return;

        _currentTargetIndex = (_currentTargetIndex - 1 + _enemies.Count) % _enemies.Count;
        Debug.Log($"Target: {_enemies[_currentTargetIndex].name}");
    }

    public void MoveTargetRight() {
        if (_enemies.Count == 0) return;

        _currentTargetIndex = (_currentTargetIndex + 1) % _enemies.Count;
        Debug.Log($"Target: {_enemies[_currentTargetIndex].name}");
    }

    private List<Character> GetTargets(Character attacker, SkillTargetType targetType) {
        var enemies = GetEnemies(attacker);

        ValidateTarget();
        return targetType switch {
            SkillTargetType.Single => new List<Character> { enemies[_currentTargetIndex] },
            SkillTargetType.All => enemies,
            _ => enemies,
        };
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

        OnBattleVictory?.Invoke(_battleType);
    }

    private void HandleGameOver() {
        if (_state != CombatState.Playing) return;

        _state = CombatState.GameOver;

        OnBattleDefeat?.Invoke();
    }
}