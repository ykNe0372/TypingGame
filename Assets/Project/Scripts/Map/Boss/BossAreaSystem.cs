using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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

    private void EnterPostOrganize() {
        _state = BossAreaState.PostOrganize;
        Debug.Log("[BossArea] Enter: PostOrganize");

        EnterSelectNextFloor();
    }

    private void EnterSelectNextFloor() {
        _state = BossAreaState.SelectNextFloor;
        Debug.Log("[BossArea] Enter: SelectNextFloor");

        RunManager.Instance.GenerateNextSection();

        var nodes = RunManager.Instance.GetStartNodes();

        Debug.Log("xxx--- Next Floor ---xxx");
        for (int i=0; i<nodes.Count; ++i) Debug.Log($"{i+1}: {nodes[i].Type}");
    }

    public void SelectNextFloor(int index) {
        var nodes = RunManager.Instance.GetStartNodes();
        if (index < 0 || index >= nodes.Count) return;

        Debug.Log("xxx--- Next Floor ---xxx");
        for (int i=0; i<nodes.Count; ++i) Debug.Log($"{i+1}: {nodes[i].Type}");

        RunManager.Instance.EnterNextFloor(nodes[index]);
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