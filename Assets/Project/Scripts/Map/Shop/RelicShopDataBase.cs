using UnityEngine;
using System.Collections.Generic;

// 販売する商品一覧
[CreateAssetMenu(menuName = "Shop/RelicDataBase")]
public class RelicShopDataBase : ScriptableObject {
    [SerializeField] private List<RelicData> _relics;

    public List<RelicData> Relics => _relics;
}