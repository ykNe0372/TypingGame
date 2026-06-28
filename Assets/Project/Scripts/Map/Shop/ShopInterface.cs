public interface IShop {
    GameState ShopState { get; }
    // bool Purchase(int offerIndex);
    void OnDigitPressed(int index);
    bool Reroll();
    void ExitShop();
}