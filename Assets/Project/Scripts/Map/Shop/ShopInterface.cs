public interface IShop {
    GameState ShopState { get; }
    bool Reroll();
    void ExitShop();
}