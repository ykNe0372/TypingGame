using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialFlowController : MonoBehaviour {
    private const string ReturnCommand = "START";
    private const string CompletionReading = "START と入力してゲーム開始";

    private enum TutorialStep {
        TypingAttack,
        ChangeSkill,
        ChangeElement,
        ExperienceStatusEffect,
        UseSpecial,
        DefeatEnemy,
        ChooseReward,
        Complete
    }

    [SerializeField] private Character _player;
    [SerializeField] private TutorialEnemyAttackController _enemy;
    [SerializeField] private CombatSystem _combatSystem;
    [SerializeField] private RewardSystem _rewardSystem;
    [SerializeField] private TMP_Text _instructionText;

    private TutorialStep _step;
    private int _initialSkillIndex;
    private bool _isSubscribed;
    private bool _isCompleteInputSubscribed;
    private bool _isLoading;
    private int _returnCommandProgress;
    private Keyboard _keyboard;
    private TypingManager _typingManager;

    private void Awake() {
        RunManager runManager = FindFirstObjectByType<RunManager>();
        if (runManager != null) runManager.enabled = false;
    }

    private void Start() {
        _typingManager = FindFirstObjectByType<TypingManager>();

        if (!ValidateReferences()) {
            enabled = false;
            return;
        }

        if (!_enemy.Initialize(_combatSystem)) {
            enabled = false;
            return;
        }
        SubscribeToEvents();
        _initialSkillIndex = _player.CurrentSkillIndex;
        SetStep(TutorialStep.TypingAttack);
        if (GameStateManager.Instance != null) GameStateManager.Instance.ChangeState(GameState.Battle);
        _combatSystem.BeginBattle(_player, new List<Character> { _enemy.Character }, BattleType.Normal);
    }

    private void OnDisable() {
        if (_isSubscribed) UnsubscribeFromEvents();
        if (_isCompleteInputSubscribed) {
            if (_keyboard != null) _keyboard.onTextInput -= HandleCompleteInput;
            _isCompleteInputSubscribed = false;
            _keyboard = null;
        }
    }

    private bool ValidateReferences() {
        if (_player == null || _enemy == null || _combatSystem == null ||
            _rewardSystem == null || _instructionText == null) {
            Debug.LogError("Assign all tutorial flow references in the Inspector.", this);
            return false;
        }

        if (_enemy.Character == null) {
            Debug.LogError("The tutorial enemy character must be assigned.", this);
            return false;
        }

        if (_player.Skills.Count < 2) {
            Debug.LogError("The tutorial player needs at least two skills to demonstrate skill switching.", this);
            return false;
        }

        return true;
    }

    private void SubscribeToEvents() {
        _player.OnSkillChanged += HandleSkillChanged;
        _player.OnElementChanged += HandleElementChanged;
        _enemy.Character.OnStatusEffectApplied += HandleStatusEffectApplied;
        _combatSystem.OnPlayerWordCompleted += HandleWordCompleted;
        _combatSystem.OnPlayerSpecialAttackExecuted += HandleSpecialAttackExecuted;
        _combatSystem.OnBattleVictory += HandleBattleVictory;
        _rewardSystem.OnRewardFinished += HandleRewardFinished;
        _isSubscribed = true;
    }

    private void UnsubscribeFromEvents() {
        _player.OnSkillChanged -= HandleSkillChanged;
        _player.OnElementChanged -= HandleElementChanged;
        _enemy.Character.OnStatusEffectApplied -= HandleStatusEffectApplied;
        _combatSystem.OnPlayerWordCompleted -= HandleWordCompleted;
        _combatSystem.OnPlayerSpecialAttackExecuted -= HandleSpecialAttackExecuted;
        _combatSystem.OnBattleVictory -= HandleBattleVictory;
        _rewardSystem.OnRewardFinished -= HandleRewardFinished;
        _isSubscribed = false;
    }

    private void HandleWordCompleted() {
        if (_step == TutorialStep.TypingAttack) {
            SetStep(TutorialStep.ChangeSkill);
        }
    }

    private void HandleSkillChanged(int skillIndex) {
        if (_step == TutorialStep.ChangeSkill && skillIndex != _initialSkillIndex) {
            SetStep(TutorialStep.ChangeElement);
        }
    }

    private void HandleElementChanged(ElementType _) {
        if (_step != TutorialStep.ChangeElement) return;

        SetStep(TutorialStep.ExperienceStatusEffect);
    }

    private void HandleStatusEffectApplied(StatusEffectType _) {
        if (_step != TutorialStep.ExperienceStatusEffect) return;

        _combatSystem.AddSpecialCharge(100);
        SetStep(TutorialStep.UseSpecial);
    }

    private void HandleSpecialAttackExecuted(int _) {
        if (_step == TutorialStep.UseSpecial) SetStep(TutorialStep.DefeatEnemy);
    }

    private void HandleBattleVictory(BattleType _) {
        if (_step == TutorialStep.Complete || _step == TutorialStep.ChooseReward) return;

        if (_step != TutorialStep.DefeatEnemy) {
            Debug.LogWarning("The tutorial enemy was defeated before all tutorial steps were completed.", this);
        }

        SetStep(TutorialStep.ChooseReward);
        _rewardSystem.ShowReward(_player);
    }

    private void HandleRewardFinished() {
        if (_step != TutorialStep.ChooseReward) return;

        SetStep(TutorialStep.Complete);
        _returnCommandProgress = 0;
        if (_typingManager == null) {
            Debug.LogWarning("TypingManager is not present; the completion prompt cannot be updated.", this);
        }
        UpdateCompletionPrompt();

        _keyboard = Keyboard.current;
        if (_keyboard == null) {
            Debug.LogError("A keyboard is required to return to the title from the tutorial.", this);
            return;
        }

        _keyboard.onTextInput += HandleCompleteInput;
        _isCompleteInputSubscribed = true;
    }

    private void HandleCompleteInput(char input) {
        if (_isLoading || _step != TutorialStep.Complete) return;

        char normalized = char.ToUpperInvariant(TypingInputNormalizer.Normalize(input));
        if (!char.IsLetter(normalized)) return;

        if (_returnCommandProgress < ReturnCommand.Length &&
            normalized == ReturnCommand[_returnCommandProgress]) {
            ++_returnCommandProgress;
        } else {
            _returnCommandProgress = normalized == ReturnCommand[0] ? 1 : 0;
        }

        UpdateCompletionPrompt();
        if (_returnCommandProgress == ReturnCommand.Length) TutorialToTyping();
    }

    private void UpdateCompletionPrompt() {
        if (_typingManager == null) return;

        _typingManager.ShowCommandPrompt(
            ReturnCommand,
            CompletionReading,
            ReturnCommand,
            _returnCommandProgress
        );
    }

    private void TutorialToTyping() {
        if (!Application.CanStreamedLevelBeLoaded(SceneNames.Typing)) {
            Debug.LogError($"Scene '{SceneNames.Typing}' is not included in the build settings.", this);
            return;
        }

        _isLoading = true;
        SceneManager.LoadScene(SceneNames.Typing);
    }

    private void SetStep(TutorialStep step) {
        _step = step;
        _enemy.Character.SetInvulnerable(step != TutorialStep.DefeatEnemy);
        _instructionText.text = step switch {
            TutorialStep.TypingAttack => "タイピングして敵を攻撃しましょう。",
            TutorialStep.ChangeSkill => "数字キーで別の技を選ぶことができます。",
            TutorialStep.ChangeElement => "選択中の技を再度選択すると、属性を変更できます。",
            TutorialStep.ExperienceStatusEffect => "属性攻撃はMPを消費しますが、確率で状態異常を付与します。\n実際に攻撃して、状態異常を発動させてみましょう。",
            TutorialStep.UseSpecial => "100字打つ毎に、必殺技が1段階溜まります（最大3段階）。\nEnterキーで必殺技を発動してみましょう。",
            TutorialStep.DefeatEnemy => "基本操作は以上です。\nそれでは、敵を倒してみましょう。",
            TutorialStep.ChooseReward => "戦闘後は強化アイテムを1つ貰えます。(矢印:移動 Space:選択)",
            TutorialStep.Complete => "チュートリアル完了！\nSTART と入力してタイトルに戻ります。",
            _ => string.Empty
        };
    }
}
