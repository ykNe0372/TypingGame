using UnityEngine;
using System.Collections.Generic;

public class BossAreaSystem : MonoBehaviour {
    [SerializeField] private CombatSystem _combatSystem;

    private Character _player;
    private List<Character> _boss;
    private BossAreaState _state;

    public BossAreaState State => _state;

    private void Awake() {
        _combatSystem.OnBattleVictory += HandleBattleVictory;
        _combatSystem.OnBattleDefeat += HandleBattleDefeat;
    }

    private void OnDestroy() {
        _combatSystem.OnBattleVictory -= HandleBattleVictory;
        _combatSystem.OnBattleDefeat -= HandleBattleDefeat;
    }

    public void EnterBossArea(Character player, List<Character> boss) {
        _player = player;
        _boss = boss;
        EnterPreOrganize();
    }

    // ボス戦前の整理エリア
    private void EnterPreOrganize() {
        _state = BossAreaState.PreOrganize;
        Debug.Log("[BossArea] Enter: PreOrganize");

        EnterBattle();
    }

    private void EnterBattle() {
        _state = BossAreaState.Battle;
        Debug.Log("[BossArea] Enter: Battle");

        _combatSystem.BeginBattle(_player, _boss, BattleType.Boss);
    }

    // ボス戦後の整理エリア
    private void EnterPostOrganize() {
        _state = BossAreaState.PostOrganize;
        Debug.Log("[BossArea] Enter: PostOrganize");

        EnterSelectNextFloor();
    }

    private void EnterSelectNextFloor() {
        _state = BossAreaState.SelectNextFloor;
        Debug.Log("[BossArea] Enter: SelectNextFloor");

        RunManager.Instance.GenerateNextSection();
    }

    private void Complete() {
        _state = BossAreaState.Complete;
        Debug.Log("[BossArea] Enter: Complete");
    }

    private void HandleBattleVictory(BattleType battleType) {
        if (battleType != BattleType.Boss) return;

        Debug.Log("[Boss] Victory");
        EnterPostOrganize();

        // TODO: 勝利演出・リザルトUIなど
    }

    private void HandleBattleDefeat() {
        Debug.Log("[Boss] GAME OVER");
        Complete();
        // TODO: ゲームオーバーUI・BGM停止など
    }
}