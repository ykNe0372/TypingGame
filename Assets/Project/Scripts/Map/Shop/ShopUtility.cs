using UnityEngine;
using System.Collections.Generic;

public static class ShopUtility {
    // 商品を選択
    public static List<T> PickRandomOffers<T>(List<T> source, int count, IEnumerable<T> excludeItems = null) {
        List<T> candidates = new(source);
        List<T> result = new();

        // 除外対象を抽選候補から外す
        if (excludeItems != null) {
            foreach (var item in excludeItems) {
                candidates.RemoveAll(x => EqualityComparer<T>.Default.Equals(x, item));
            }
        }

        for (int i=0; i<count; ++i) {
            if (candidates.Count == 0) break;

            int index = Random.Range(0, candidates.Count);
            result.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        return result;
    }
}