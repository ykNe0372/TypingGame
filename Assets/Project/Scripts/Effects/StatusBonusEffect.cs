using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Effect/Status Bonus")]
public class StatusBonusEffect : Effect {
    [Serializable]
    public class StatusEntry {
        public StatusType statusType;
        public int value;
    }

    [SerializeField] private List<StatusEntry> _status = new();

    public override int GetStatusBonus(StatusType type) {
        int total = 0;

        foreach (var s in _status) {
            if (s.statusType == type) total += s.value;
        }
    
        return total;
    }
}