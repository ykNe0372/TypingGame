using System;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "Data/Character/BaseStatus")]
public class CharacterBaseStatus : ScriptableObject {
    [Serializable]
    public class StatusEntry {
        public StatusType statusType;
        public float value;
    }

    [SerializeField] private List<StatusEntry> _status = new(); // Inspector 編集用
    
    private Dictionary<StatusType, float> _statusDictionary;

    private void Init() {
        _statusDictionary = new Dictionary<StatusType, float>();
        foreach (var s in _status) _statusDictionary[s.statusType] = s.value;
    }

    public float GetStatus(StatusType type) {
        if (_statusDictionary == null) Init();
        return _statusDictionary.TryGetValue(type, out var value) ? value : 0f;
    }
}