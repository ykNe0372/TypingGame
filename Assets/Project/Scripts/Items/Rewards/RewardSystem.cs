using UnityEngine;
using System.Collections.Generic;
using System;

public class RewardSystem : MonoBehaviour {
    [SerializeField, Header("報酬（強化アイテム）候補")] private List<GrowthItem> _itemPool = new();
    [SerializeField, Header("報酬（レリック）候補")] private List<RelicData> _relicPool = new();
    [SerializeField] private RewardSelectionUI _selectionUI;

    public event Action OnRewardFinished;    // 報酬受け取り完了通知

    public void ShowReward(Character player) {
        var rewards = GetRandomItems(3);
        _selectionUI.OpenItems(rewards);
        _selectionUI.OnSelected += OnItemSelected;

        // ↓【ローカル関数】という機能
        void OnItemSelected(int index) {
            _selectionUI.OnSelected -= OnItemSelected;
            player.AddItem(rewards[index]);
            _selectionUI.Close();

            OnRewardFinished?.Invoke();  // 通常戦闘時の報酬は強化アイテムだけ
        }
    }

    public void ShowBossReward(Character player) {
        var itemRewards = GetRandomItems(3);
        _selectionUI.OpenItems(itemRewards);
        _selectionUI.OnSelected += OnItemSelected;

        void OnItemSelected(int index) {
            _selectionUI.OnSelected -= OnItemSelected;
            player.AddItem(itemRewards[index]);
            _selectionUI.Close();

            ShowRelicReward(player);  // 強化アイテム報酬→レリック報酬
        }
    }

    private void ShowRelicReward(Character player) {
        var relicRewards = GetRandomRelics(3);
        _selectionUI.OpenRelics(relicRewards);
        _selectionUI.OnSelected += OnRelicSelected;

        void OnRelicSelected(int index) {
            _selectionUI.OnSelected -= OnRelicSelected;
            player.EquipRelic(relicRewards[index]);
            _selectionUI.Close();

            OnRewardFinished?.Invoke();
        }
    }

    private List<GrowthItem> GetRandomItems(int count) {
        var pool = new List<GrowthItem>(_itemPool);
        var result = new List<GrowthItem>();

        for (int i=0; i<count; ++i) {
            if (pool.Count <= 0) break;

            int index = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);   // 重複防止
        }

        return result;
    }

    private List<RelicData> GetRandomRelics(int count) {
        var pool = new List<RelicData>(_relicPool);
        var result = new List<RelicData>();

        for (int i=0; i<count; ++i) {
            if (pool.Count <= 0) break;

            int index = UnityEngine.Random.Range(0, pool.Count);
            result.Add(pool[index]);
            pool.RemoveAt(index);   // 重複防止
        }

        return result;
    }
}