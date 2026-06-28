using UnityEngine;
using System.Collections.Generic;

public class RelicShopSystem : MonoBehaviour, IShop {
    private enum RelicShopState {
        SelectingRelic,
        SelectingCost
    }

    [SerializeField] private RelicShopDataBase _dataBase;
    [SerializeField] private float _costPercent;

    private Character _player;
    private RelicShopState _state;
    private readonly List<RelicShopOffer> _offers = new();
    private bool _rerolled;
    private int _selectedOfferIndex;

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

    public void OnDigitPressed(int index) {
        switch (_state) {
            case RelicShopState.SelectingRelic:
                SelectRelic(index);
                break;
            case RelicShopState.SelectingCost:
                SelectCost(index);
                break;
        }
    }

    private void SelectRelic(int index) {
        if (index < 0 || index >= _offers.Count) return;

        _selectedOfferIndex = index;
        _state = RelicShopState.SelectingCost;

        Debug.Log("支払い方法を選択 | 1: HP / 2: MP");
    }

    private void SelectCost(int index) {
        switch (index) {
            case 0:
                Purchase(_selectedOfferIndex, RelicCostType.HP);
                break;
            case 1:
                Purchase(_selectedOfferIndex, RelicCostType.MP);
                break;
            default:
                return;
        }
    }

    private bool Purchase(int offerIndex, RelicCostType type) {
        if (!CanPurchase(offerIndex)) return false;

        ExecutePurchase(offerIndex, type);
        return true;
    }

    private bool CanPurchase(int offerIndex) {
        if (offerIndex < 0 || offerIndex >= _offers.Count) return false;

        float consumeHPPercent = _player.GetCurrentHPRatio();
        float consumeMPPercent = _player.GetCurrentMPRatio();

        // HP は購入で 0 になると変、MP は購入で 0 になっても変じゃない
        return consumeHPPercent > (_costPercent / 100f) || consumeMPPercent >= (_costPercent / 100f);
    }

    // UI 実装前の仮実装（自動選択、本実装では選択式にする）
    // 多分 ExecutePurchase(Character player, ShopOffer offer, List<GrowthItem> materials) とかになる
    private void ExecutePurchase(int offerIndex, RelicCostType type) {
        RelicShopOffer offer = _offers[offerIndex];
        
        switch (type) {
            case RelicCostType.HP:
                _player.TakePercentDamage(_costPercent);
                break;
            case RelicCostType.MP:
                _player.ConsumePercentMP(_costPercent);
                break;
        }

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