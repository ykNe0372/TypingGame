using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/StatusEffect/Resolver")]
public class StatusEffectResolver : ScriptableObject {
    [Serializable]
    public struct Entry {
        public ElementType element;
        public StatusEffectData effect;
    }

    [SerializeField] private List<Entry> _entries;

    private Dictionary<ElementType, StatusEffectData> _map;

    private void OnEnable() {
        _map = new();
        foreach (var e in _entries) _map[e.element] = e.effect;
    }

    public StatusEffectData Get(ElementType element) {
        return _map.TryGetValue(element, out var effect) ? effect : null;
    }
}