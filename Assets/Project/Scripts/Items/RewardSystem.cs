using UnityEngine;
using System.Collections.Generic;

public class RewardSystem : MonoBehaviour {
    [SerializeField] private List<GrowthItem> _itemPool = new();
    [SerializeField] private RewardSelectionUI _selectionUI;

    public void ShowReward(Character player) {
        var rewards = GetRandomItems(3);
        _selectionUI.Open(rewards, player);
    }

    private List<GrowthItem> GetRandomItems(int count) {
        var pool = new List<GrowthItem>(_itemPool);
        var result = new List<GrowthItem>();

        for (int i=0; i<count; ++i) {
            if (pool.Count <= 0) break;
            int index = Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);   // 重複防止
        }

        return result;
    }
}