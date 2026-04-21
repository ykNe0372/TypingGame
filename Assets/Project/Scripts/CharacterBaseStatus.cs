using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEntry {
    public StatusType statusType;
    public int value;
}

[CreateAssetMenu(menuName = "Data/Character/BaseStatus")]
public class CharacterBaseStatus : ScriptableObject {
    [SerializeField] private List<StatusEntry> _status = new(); // Inspector 編集用
    
    private Dictionary<StatusType, int> _statusDictionary;

    private void Init() {
        _statusDictionary = new Dictionary<StatusType, int>();
        foreach (var entry in _status) _statusDictionary[entry.statusType] = entry.value;
    }

    public int GetStatus(StatusType type) {
        if (_statusDictionary == null) Init();
        return _statusDictionary.TryGetValue(type, out var value) ? value : 0;
    }
}