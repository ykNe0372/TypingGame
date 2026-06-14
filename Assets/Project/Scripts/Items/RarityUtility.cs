public static class RarityUtility {
    public static ItemRarity? GetLowerRarity(ItemRarity rarity) {
        return rarity switch {
            ItemRarity.Legendary => ItemRarity.Epic,
            ItemRarity.Epic => ItemRarity.Rare,
            ItemRarity.Rare => ItemRarity.Common,
            ItemRarity.Common => null,
            _ => null
        };
    }

    public static ItemRarity? GetHigherRarity(ItemRarity rarity) {
        return rarity switch {
            ItemRarity.Common => ItemRarity.Rare,
            ItemRarity.Rare => ItemRarity.Epic,
            ItemRarity.Epic => ItemRarity.Legendary,
            ItemRarity.Legendary => null,
            _ => null
        };
    }
}