using UnityEngine;
using System.Collections.Generic;

// 販売する商品一覧
[CreateAssetMenu(menuName = "Shop/ShopDataBase")]
public class ShopDataBase : ScriptableObject {
    [SerializeField] private List<GrowthItem> _items;

    public List<GrowthItem> Items => _items;
}