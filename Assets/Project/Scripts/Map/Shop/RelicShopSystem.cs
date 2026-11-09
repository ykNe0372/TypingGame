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
    private RewardSelectionUI _selectionUI;
    private RelicShopState _state;
    private readonly List<RelicShopOffer> _offers = new();
    private bool _rerolled;
    private int _selectedOfferIndex;

    public IReadOnlyList<RelicShopOffer> RelicOffers => _offers;
    public GameState ShopState => GameState.RelicShop;

    public void EnterRelicShop(Character player, RewardSelectionUI selectionUI) {
        _player = player;
        _selectionUI = selectionUI;
        _rerolled = false;
        _state = RelicShopState.SelectingRelic;
        GenerateOffers(3);
        ShowRelicOffers();
    }

    public void ExitShop() {
        _selectionUI.OnSelected -= HandleSelection;
        _selectionUI.Close();

        Debug.Log("Relic Shop Exit");
        GameStateManager.Instance.ChangeState(GameState.MapSelect);
    }

    public void GenerateOffers(int count) {
        _offers.Clear();
        var relics = ShopUtility.PickRandomOffers(_dataBase.Relics, count, _player.Relics);
        foreach (var relic in relics) _offers.Add(new RelicShopOffer(relic));
    }

    private void ShowRelicOffers() {
        _selectionUI.OnSelected -= HandleSelection;
        _selectionUI.Close();

        List<RelicData> relics = new();
        foreach (var offer in _offers) relics.Add(offer.Relic);
        _selectionUI.OpenRelics(relics);
        _selectionUI.SetEscapeHintVisible(true);
        _selectionUI.OnSelected += HandleSelection;
    }

    private void HandleSelection(int index) {
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
        _selectionUI.OpenOptions(
            new[] { "HPで支払う", "MPで支払う" },
            new[] {
                $"最大HPの{_costPercent}%を消費（HPは1以上残ります）。",
                $"最大MPの{_costPercent}%を消費します。"
            });
        _selectionUI.SetEscapeHintVisible(true);
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
        if (!CanPurchase(offerIndex, type)) return false;

        ExecutePurchase(offerIndex, type);
        return true;
    }

    private bool CanPurchase(int offerIndex, RelicCostType type) {
        if (offerIndex < 0 || offerIndex >= _offers.Count) return false;

        return type switch {
            RelicCostType.HP => _player.GetCurrentHPRatio() > (_costPercent / 100f),
            RelicCostType.MP => _player.GetCurrentMPRatio() >= (_costPercent / 100f),
            _ => false
        };
    }

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
        if (_state != RelicShopState.SelectingRelic) return false;
        if (_rerolled) {
            Debug.Log("Already Rerolled");
            return false;
        }

        _rerolled = true;
        GenerateOffers(3);
        ShowRelicOffers();
        Debug.Log("Shop Rerolled");
        
        return true;
    }
}