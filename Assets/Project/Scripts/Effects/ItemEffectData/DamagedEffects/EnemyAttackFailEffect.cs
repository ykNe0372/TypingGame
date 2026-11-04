using UnityEngine;

// 敵が稀に攻撃の発動を失敗する（自分を対象にした攻撃が確率で不発になる）
[CreateAssetMenu(menuName = "Data/Effect/Damaged/Enemy Attack Fail")]
public class EnemyAttackFailEffect : Effect, IIncomingAttackCancelHook {
    [SerializeField, Tooltip("不発確率（%）")] private float _failChance = 8f;

    public bool ShouldCancelIncomingAttack(Character owner, AttackContext ctx) {
        if (ctx.Attacker == owner) return false;
        if (!EffectUtility.Roll(_failChance)) return false;

        Debug.Log($"[EnemyAttackFail] {ctx.Attacker.name} の攻撃が失敗");
        return true;
    }
}
