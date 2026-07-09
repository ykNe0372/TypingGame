using UnityEngine;

[CreateAssetMenu(menuName = "Buffs/Duration/Time")]
public class TimeDurationData : BuffDurationData {
    [SerializeField] private float _seconds;

    public override BuffDuration Create() {
        return new TimeDuration(_seconds);
    }
}