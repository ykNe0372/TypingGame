using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Relic")]
public class RelicData : ScriptableObject {
    [Header("UI")]
    [SerializeField] private string _relicName;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _icon;

    [SerializeField, Header("Effects")] private List<RelicEffect> _effects = new();


    public string RelicName => _relicName;
    public string Description => _description;
    public Sprite Icon => _icon;

    public List<RelicEffect> GetEffects() {
        return _effects;
    }
}