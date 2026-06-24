using UnityEngine;
using System.Collections.Generic;

public class RelicShopSystem : MonoBehaviour, IShop {
    [SerializeField] private RelicShopDataBase _dataBase;

    private Character _player;
    private readonly List<RelicShopOffer> _offers = new();
    private bool _rerolled;

    public IReadOnlyList<RelicShopOffer> RelicOffers => _offers;
    public GameState ShopState => GameState.RelicShop;

    public void EnterRelicShop(Character player) {
        _player = player;
        _rerolled = false;
        GenerateOffers(3);

        Debug.Log("xxx--- RELIC SHOP ---xxx");
        foreach (var offer in _offers) Debug.Log($"{offer.Relic.RelicName}");
    }

    public void ExitShop() {
        Debug.Log("Relic Shop Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    public void GenerateOffers(int count) {
        _offers.Clear();
        var relics = ShopUtility.PickRandomOffers(_dataBase.Relics, count);
        foreach (var relic in relics) _offers.Add(new RelicShopOffer(relic));
    }

    public bool Purchase(int offerIndex) {
        if (!CanPurchase(offerIndex)) return false;

        ExecutePurchase(offerIndex);
        return true;
    }

    // UI 実装前の仮実装（自動選択、本実装では選択式にする）
    // 多分 ExecutePurchase(Character player, ShopOffer offer, List<GrowthItem> materials) とかになる
    private bool CanPurchase(int offerIndex) {
        if (offerIndex < 0 || offerIndex >= _offers.Count) return false;
        // HP or MPが消費%以上あるか（購入可能かを確認）

        return true;
    }

    private void ExecutePurchase(int offerIndex) {
        RelicShopOffer offer = _offers[offerIndex];
        // 実際に消費する処理
        _player.EquipRelic(offer.Relic);

        ExitShop();
    }

    public bool Reroll() {
        if (_rerolled) {
            Debug.Log("Already Rerolled");
            return false;
        }

        _rerolled = true;
        GenerateOffers(3);
        Debug.Log("Shop Rerolled");
        foreach (var offer in _offers) Debug.Log($"{offer.Relic.RelicName}");
        
        return true;
    }
}