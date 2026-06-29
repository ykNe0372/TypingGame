using UnityEngine;
using System.Collections.Generic;

public static class ShopUtility {
    // 商品を選択
    public static List<T> PickRandomOffers<T>(List<T> source, int count) {
        List<T> candidates = new(source);
        List<T> result = new();

        for (int i=0; i<count; ++i) {
            if (candidates.Count == 0) break;

            int index = Random.Range(0, candidates.Count);
            result.Add(candidates[index]);
            candidates.RemoveAt(index);
        }

        return result;
    }
}