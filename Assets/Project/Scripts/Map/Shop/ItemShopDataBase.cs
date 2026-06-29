using UnityEngine;
using System.Collections.Generic;

// 販売する商品一覧
[CreateAssetMenu(menuName = "Shop/ShopDataBase")]
public class ItemShopDataBase : ScriptableObject {
    [SerializeField] private List<GrowthItem> _items;

    public List<GrowthItem> Items => _items;
}