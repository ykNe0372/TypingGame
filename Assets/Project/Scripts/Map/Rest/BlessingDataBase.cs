using UnityEngine;
using System.Collections.Generic;

// 恩恵一覧
[CreateAssetMenu(menuName = "Rest/BlessingDataBase")]
public class BlessingDataBase : ScriptableObject {
    [SerializeField] private List<BuffData> _items;

    public List<BuffData> Items => _items;
}