using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    [SerializeField] private CharacterBaseStatus _baseStatus;
    [SerializeField] private List<GrowthItem> _items = new();

    public int GetFinalStatus(StatusType type) {
        int baseValue = _baseStatus.GetStatus(type);
        int bonus = 0;
        
        foreach (var item in _items) bonus += item.GetStatusBonus(type);
        return baseValue + bonus;
    }    
}