using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatSystem : MonoBehaviour {
    [SerializeField] private Character _player;
    [SerializeField] private List<Character> _enemies;
    // [SerializeField] private bool _isPaused;
    [SerializeField] private float _freezeDelay = 2f;
    [SerializeField] private CombatState _state = CombatState.Playing;
    [SerializeField] private StatusEffectResolver _resolver;

    private int _currentTargetIndex = 0;
    private float _gameOverTimer;

    public CombatState State => _state;

    private void Start() {
        _player.OnDead += HandlePlayerDead;
        foreach (var enemy in _enemies) enemy.OnDead += HandleEnemyDead;
    }

    private void Update() {
        CheckBattleResult();

        if (_state == CombatState.GameOver) {
            _gameOverTimer += Time.deltaTime;
            if (_gameOverTimer >= _freezeDelay) Time.timeScale = 0f;
        }
    }

    private void HandlePlayerDead(Character player) {
        Debug.Log("Player Dead");
        // TODO: 死亡アニメーション・死亡エフェクトの再生・UI演出開始など
    }

    private void HandleEnemyDead(Character enemy) {
        Debug.Log($"{enemy.name} defeated");
        ValidateTarget();

        // TODO: 死亡アニメーション・死亡エフェクトの再生など
    }

    // 撃破時にターゲットを自動に切り替える
    private void ValidateTarget() {
        _enemies.RemoveAll(x => x.IsDead);
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
            Targets = GetTargets(attacker, attacker.CurrentSkill),
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
        if (bonus == null) return;

        var targets = GetBonusTargets(bonus);
        _player.TriggerBonusAttack(targets, bonus);
    }

    public void RequestEnemyAttack(Character enemy) {
        if (!CanAttack(enemy)) return;

        var ctx = CreateContext(enemy);
        enemy.TriggerAttack(ctx);
    }

    public List<Character> GetEnemies(Character requester) {
        if (requester == _player) return _enemies;
        else return new List<Character> { _player };  // 仮でプレイヤーだけ、分身など味方 NPC が出てきた時は変更
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

    private List<Character> GetTargets(Character attacker, SkillData skill) {
        var enemies = GetEnemies(attacker);

        return skill.targetType switch {
            SkillTargetType.Single => new List<Character> { enemies[_currentTargetIndex] },  // 仮で先頭の敵に飛ぶようにする
            SkillTargetType.All => enemies,
            _ => enemies,
        };
    }

    // 仮、後で↑と統合するかも
    private List<Character> GetBonusTargets(BonusAttackData bonus) {
        return bonus.targetType switch {
            SkillTargetType.Single => new List<Character> { _enemies[_currentTargetIndex] },
            SkillTargetType.All => _enemies,
            _ => _enemies,
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
        Debug.Log("Victory");

        // TODO: 勝利演出・リザルトUI・報酬処理など
    }

    private void HandleGameOver() {
        if (_state != CombatState.Playing) return;

        _state = CombatState.GameOver;
        Debug.Log("Game Over");

        // TODO: ゲームオーバーUI・BGM停止など
    }
}