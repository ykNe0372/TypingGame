public interface IShop {
    GameState ShopState { get; }
    bool Purchase(int offerIndex);
    bool Reroll();
    void ExitShop();
}