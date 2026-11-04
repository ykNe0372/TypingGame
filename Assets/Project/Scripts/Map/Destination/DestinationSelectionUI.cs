// DestinationSelectionUI.cs
using UnityEngine;
using System.Collections.Generic;

public class DestinationSelectionUI : MonoBehaviour {
    [SerializeField] private DestinationOption _optionPrefab;
    [SerializeField] private Transform _optionRoot;

    private readonly List<DestinationOption> _options = new();

    // 選択可能なノード一覧を受け取り、行き先候補を表示する
    public void Show(IReadOnlyList<MapNode> nodes) {
        ClearOptions();
        gameObject.SetActive(true);

        Vector2 startPosition = Vector2.zero;

        for (int i = 0; i < nodes.Count; i++) {
            var option = Instantiate(_optionPrefab, _optionRoot);

            RectTransform rectTransform = option.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = startPosition + new Vector2(0f, -80f * i);

            string displayName = GetDisplayName(nodes[i].Type);
            option.Initialize(i + 1, displayName);
            _options.Add(option);
        }
    }

    // MapTypeをUI表示用の文字列に変換する
    private string GetDisplayName(MapType type) {
        return type switch {
            MapType.Battle => "BATTLE",
            MapType.ItemShop => "ITEM SHOP",
            MapType.RelicShop => "RELIC SHOP",
            MapType.Rest => "REST",
            MapType.Boss => "BOSS",
            _ => type.ToString()
        };
    }

    // 行き先候補を削除してパネルを閉じる
    public void Hide() {
        ClearOptions();
        gameObject.SetActive(false);
    }

    // 生成済みの候補をすべて削除する
    private void ClearOptions() {
        foreach (var option in _options) {
            if (option != null) {
                Destroy(option.gameObject);
            }
        }

        _options.Clear();
    }
}