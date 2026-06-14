using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Growth Item")]
public class GrowthItem : ScriptableObject {
    [Header("UI")]
    [SerializeField] private string _itemName;
    [SerializeField][TextArea] private string _description;
    [SerializeField] private Sprite _icon;
    [SerializeField] private ItemRarity _rarity;

    [SerializeField, Header("Effects")] private List<Effect> _effects = new();
    [SerializeField, Header("Merge")] private GrowthItem _nextRarityItem;

    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public ItemRarity Rarity => _rarity;
    public GrowthItem NextRarityItem => _nextRarityItem;

    public List<Effect> GetEffects() {
        return _effects;
    }
}