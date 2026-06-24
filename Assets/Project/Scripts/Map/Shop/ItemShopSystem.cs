using UnityEngine;
using System.Collections.Generic;

public class ItemShopSystem : MonoBehaviour, IShop {
    [SerializeField] private ItemShopDataBase _dataBase;
    [SerializeField] private int _commonRate;
    [SerializeField] private int _rareRate;

    private Character _player;
    private readonly List<ItemShopOffer> _offers = new();
    private bool _rerolled;
    private bool _hasPurchased;

    public IReadOnlyList<ItemShopOffer> ItemOffers => _offers;
    public GameState ShopState => GameState.ItemShop;

    public void EnterShop(Character player) {
        _player = player;
        _rerolled = false;
        _hasPurchased = false;
        GenerateOffers(5);

        Debug.Log("xxx--- ITEM SHOP ---xxx");
        foreach (var offer in _offers) Debug.Log($"{offer.Item.ItemName} [{offer.Item.Rarity}]");
    }

    public void ExitShop() {
        if (!_hasPurchased) {
            GrowthItem item = GetServiceItem();  // 所持上限に達していた時用に抽選と取得は分ける
            if (item != null) _player.AddItem(item);
        }

        Debug.Log("Item Shop Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    public void GenerateOffers(int count) {
        _offers.Clear();
        var items = ShopUtility.PickRandomOffers(_dataBase.Items, count);
        foreach (var item in items) _offers.Add(new ItemShopOffer(item));
    }

    public bool Purchase(int offerIndex) {
        if (!CanPurchase(offerIndex)) return false;

        ExecutePurchase(offerIndex);
        return true;
    }

    private bool CanPurchase(int offerIndex) {
        ItemShopOffer offer = _offers[offerIndex];
        List<GrowthItem> materials = AutoSelectMaterials(offer.Item);
    
        if (offerIndex < 0 || offerIndex >= _offers.Count) return false;
        if (offer.IsSoldOut) return false;
        if (!HasMaterials(offer.Item)) return false;
        if (materials.Count == 0) return false;

        return true;
    }

    // UI 実装前の仮実装（自動選択、本実装では選択式にする）
    // 多分 ExecutePurchase(Character player, ShopOffer offer, List<GrowthItem> materials) とかになる？
    private void ExecutePurchase(int offerIndex) {
        ItemShopOffer offer = _offers[offerIndex];
        List<GrowthItem> materials = AutoSelectMaterials(offer.Item);
        if (materials != null) ConsumeMaterials(materials);
    
        _player.AddItem(offer.Item);
        offer.IsSoldOut = true;
        _hasPurchased = true;
    }

    private bool HasMaterials(GrowthItem item) {
        ItemRarity rarity = item.Rarity;
        ItemRarity? higherRarity = RarityUtility.GetHigherRarity(rarity);
        ItemRarity? lowerRarity = RarityUtility.GetLowerRarity(rarity);
        
        bool same = _player.CountItemsByRarity(rarity) >= 1;
        bool higher = higherRarity.HasValue && _player.CountItemsByRarity(higherRarity.Value) >= 1;
        bool lower = lowerRarity.HasValue && _player.CountItemsByRarity(lowerRarity.Value) >= 2;

        Debug.Log($"same: {_player.CountItemsByRarity(rarity)}, " +
            $"higher: {(higherRarity.HasValue ? _player.CountItemsByRarity(higherRarity.Value) : 0)}, " +
            $"lower: {(lowerRarity.HasValue ? _player.CountItemsByRarity(lowerRarity.Value) : 0)}");

        return same || higher || lower;
    }

    private List<GrowthItem> AutoSelectMaterials(GrowthItem targetItem) {
        ItemRarity rarity = targetItem.Rarity;
        ItemRarity? lowerRarity = RarityUtility.GetLowerRarity(rarity);
        ItemRarity? higherRarity = RarityUtility.GetHigherRarity(rarity);
        
        // 下位レア2個（優先）
        if (lowerRarity.HasValue) {
            var lowerItems = _player.GetItemsByRarity(lowerRarity.Value, 2);
            if (lowerItems.Count >= 2) return lowerItems;
        }

        // 同レア1個
        var sameItems = _player.GetItemsByRarity(rarity, 1);
        if (sameItems.Count >= 1) return sameItems;

        // 上位レア1個（仮実装）
        if (higherRarity.HasValue) {
            var higherItems = _player.GetItemsByRarity(higherRarity.Value, 1);
            if (higherItems.Count >= 1) return higherItems;
        }

        return new List<GrowthItem>();
    }

    private void ConsumeMaterials(List<GrowthItem> materials) {
        foreach (var item in materials) _player.RemoveItem(item);
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