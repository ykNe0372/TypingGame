using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour {
    [SerializeField] private ShopDataBase _dataBase;
    [SerializeField] private int _commonRate;
    [SerializeField] private int _rareRate;

    private List<ShopOffer> _offers = new();
    private bool _rerolled;
    private bool _hasPurchased;

    public IReadOnlyList<ShopOffer> Offers => _offers;

    public void EnterShop(MapNode node) {
        _rerolled = false;
        _hasPurchased = false;
        GenerateOffers(5);

        Debug.Log("xxx--- SHOP ---xxx");
        foreach (var offer in _offers) Debug.Log($"{offer.Item.ItemName} [{offer.Item.Rarity}]");
    }

    public void ExitShop(Character player) {
        if (!_hasPurchased) {
            GrowthItem item = GetServiceItem();  // 所持上限に達していた時用に抽選と取得は分ける
            if (item != null) {
                player.AddItem(item);
                Debug.Log($"Service Item: {item.ItemName}");
            }
        }

        Debug.Log("Shop Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    public void GenerateOffers(int count) {
        _offers.Clear();
        List<GrowthItem> candidates = new(_dataBase.Items);

        for (int i=0; i<count; ++i) {
            if (candidates.Count == 0) break;

            int index = Random.Range(0, candidates.Count);
            GrowthItem item = candidates[index];
            _offers.Add(new ShopOffer(item));
            candidates.RemoveAt(index);
        }
    }

    // UI 実装前の仮実装（自動選択、本実装では選択式にする）
    // 多分 Purchase(Character player, ShopOffer offer, List<GrowthItem> materials) とかになる
    public bool Purchase(Character player, int offerIndex) {
        if (offerIndex < 0 || offerIndex >= _offers.Count) return false;

        ShopOffer offer = _offers[offerIndex];
        if (!CanPurchase(player, offer.Item)) return false;
        if (offer.IsSoldOut) {
            Debug.Log("Already SoldOut");
            return false;
        }

        List<GrowthItem> materials = AutoSelectMaterials(player, offer.Item);
        if (materials.Count == 0) return false;

        player.AddItem(offer.Item);
        if (materials != null) ConsumeMaterials(player, materials);
        offer.IsSoldOut = true;
        _hasPurchased = true;

        return true;
    }

    private bool CanPurchase(Character player, GrowthItem item) {
        ItemRarity rarity = item.Rarity;
        ItemRarity? lowerRarity = RarityUtility.GetLowerRarity(rarity);
        
        bool same = player.CountItemsByRarity(rarity) >= 1;
        bool lower = lowerRarity.HasValue && player.CountItemsByRarity(lowerRarity.Value) >= 2;

        Debug.Log($"same: {player.CountItemsByRarity(rarity)}" +
            $"lower: {(lowerRarity.HasValue ? player.CountItemsByRarity(lowerRarity.Value) : 0)}");

        return same || lower;
    }

    private List<GrowthItem> AutoSelectMaterials(Character player, GrowthItem targetItem) {
        ItemRarity rarity = targetItem.Rarity;
        ItemRarity? lowerRarity = RarityUtility.GetLowerRarity(rarity);
        List<GrowthItem> lowerItems = new();
        
        // 下位レア2個（優先）
        if (lowerRarity.HasValue) {
            foreach (var item in player.Items) {
                if (item.Rarity == lowerRarity) lowerItems.Add(item);
                if (lowerItems.Count >= 2) return new List<GrowthItem>{ lowerItems[0], lowerItems[1] };
            }
        }

        // 同レア1個
        foreach (var item in player.Items) {
            if (item.Rarity == rarity) return new List<GrowthItem>{ item };
        }

        return new List<GrowthItem>();
    }

    private void ConsumeMaterials(Character player, List<GrowthItem> materials) {
        foreach (var item in materials) player.RemoveItem(item);
    }

    public bool Reroll() {
        if (_rerolled) {
            Debug.Log("Already Rerolled");
            return false;
        }

        _rerolled = true;
        GenerateOffers(5);
        Debug.Log("Shop Rerolled");
        foreach (var offer in _offers) Debug.Log($"{offer.Item.ItemName} [{offer.Item.Rarity}]");
        
        return true;
    }

    private GrowthItem GetServiceItem() {
        ItemRarity rarity = GetServiceRarity();
        List<GrowthItem> candidates = new();

        foreach (var item in _dataBase.Items) {
            if (item.Rarity == rarity) candidates.Add(item);
        }
        if (candidates.Count == 0) return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    private ItemRarity GetServiceRarity() {
        int roll = Random.Range(0, 100);
        
        if (roll < _commonRate) return ItemRarity.Common;
        if (roll < _rareRate) return ItemRarity.Rare;
        return ItemRarity.Epic;   // Legendary はサービスでは出さない方が感覚的に自然
    }
}