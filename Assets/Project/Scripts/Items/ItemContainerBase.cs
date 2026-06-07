using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[System.Serializable]
public abstract class ItemContainerBase {
    protected readonly List<GrowthItem> _items = new();
    protected abstract int MaxItemCount { get; }

    public IReadOnlyList<GrowthItem> Items => _items;
    public int Count => _items.Count;

    public virtual bool AddItem(GrowthItem item) {
        if (!HasSpace()) return false;

        _items.Add(item);
        TryMerge(item);

        return true;
    }

    public bool HasSpace() {
        return _items.Count < MaxItemCount;
    }

    public virtual bool RemoveItem(GrowthItem item) {
        return _items.Remove(item);
    }

    private void TryMerge(GrowthItem item) {
        int count = _items.Count(x => x == item);
        if (count < 2) return;

        Merge(item);
    }

    private void Merge(GrowthItem item) {
        if (item.NextRarityItem  == null) return;

        _items.Remove(item);
        _items.Remove(item);
        GrowthItem upgraded = item.NextRarityItem;

        Debug.Log($"Merge: {item.ItemName} => {upgraded.ItemName}");
        _items.Add(upgraded);
        TryMerge(upgraded);  // 合成後の強化アイテムも合成出来る時のために再起的に呼ぶ
    }

    public int CountByRarity(ItemRarity rarity) {
        return _items.Count(x => x.Rarity == rarity);
    }

    // 所持している特定レアリティの強化アイテム一覧を取得
    public List<GrowthItem> GetItemsByRarity(ItemRarity rarity) {
        return Items.Where(x => x.Rarity == rarity).ToList();
    }
}