using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Status Bonus")]
public class StatusBonusEffect : Effect {
    [Serializable]
    public class StatusEntry {
        public StatusType statusType;
        public float value;
    }

    [SerializeField] private List<StatusEntry> _status = new();

    // 強化アイテムによるステータス変更を反映
    public override float GetStatusBonus(StatusType type) {
        float total = 0;

        foreach (var s in _status) {
            if (s.statusType == type) total += s.value;
        }
    
        return total;
    }
}