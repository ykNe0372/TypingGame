using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(menuName = "Data/Growth Item")]
public class GrowthItem : ScriptableObject {
    [Header("UI")]
    [SerializeField] private string _itemName;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _icon;

    [SerializeField, Header("Effects")] private List<Effect> _effects = new();

    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;

    public List<Effect> GetEffects() {
        return _effects;
    }
}