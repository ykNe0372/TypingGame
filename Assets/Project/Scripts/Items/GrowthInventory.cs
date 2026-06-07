using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public class ItemInventory {
    private const int MaxItemCount = 30;
    private readonly List<GrowthItem> _items = new();

    public IReadOnlyList<GrowthItem> Items => _items;
    public int Count => _items.Count;

    public bool AddItem(GrowthItem item) {
        if (!CanAddItem()) return false;
    
        _items.Add(item);
        TryMerge(item);
        
        return true;
    }

    private void TryMerge(GrowthItem item) {
        int count = _items.Count(x => x == item);
        if (count < 2) return;
        Merge(item);
    }

    private void Merge(GrowthItem item) {
        if (item.NextRarityItem == null) return;

        _items.Remove(item);
        _items.Remove(item);
        GrowthItem upgraded = item.NextRarityItem;

        Debug.Log($"Merge: {item.ItemName} -> {upgraded.ItemName}");
        _items.Add(upgraded);
        TryMerge(upgraded);   // 合成後の強化アイテムも合成出来る時のために再起的に呼ぶ
    }

    public bool RemoveItem(GrowthItem item) {
        return _items.Remove(item);
    }

    public int CountByRarity(ItemRarity rarity) {
        int count = 0;
        foreach (var item in _items) {
            if (item.Rarity == rarity) ++count;
        }

        return count;
    }

    // 所持している特定レアリティの強化アイテム一覧を取得
    public List<GrowthItem> GetItemsByRarity(ItemRarity rarity) {
        List<GrowthItem> result = new();
        foreach (var item in _items) {
            if (item.Rarity == rarity) result.Add(item);
        }

        return result;
    }

    public bool CanAddItem() {
        return _items.Count < MaxItemCount;
    }
}