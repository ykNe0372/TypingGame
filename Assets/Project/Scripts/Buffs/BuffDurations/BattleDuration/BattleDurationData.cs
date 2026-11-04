using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Duration/Battle")]
public class BattleDurationData : BuffDurationData {
    [SerializeField] private int _battleCount;

    public override BuffDuration Create() {
        return new BattleDuration(_battleCount);
    }
}