using System.Collections.Generic;
using UnityEngine;

public static class EnemyElementResolver {
    public static ElementType Resolve(EnemyData data) {
        ElementType result = data.ElementSettings.Mode switch {
            EnemyElementMode.None => ElementType.None,
            EnemyElementMode.Fixed => ResolveFixed(data),
            EnemyElementMode.Random => ResolveRandom(data),
            _ => ElementType.None,
        };

        Debug.Log($"{data.name} used element: {result}");
        return result;
    }

    private static ElementType ResolveFixed(EnemyData data) {
        if (data.ElementSettings.Elements.Count == 0) return ElementType.None;
        
        return data.ElementSettings.Elements[0].Element;
    }

    private static ElementType ResolveRandom(EnemyData data) {
        if (Random.value > data.ElementSettings.AttackChance/100f) return ElementType.None;

        return PickWeightElement(data.ElementSettings.Elements);
    }

    private static ElementType PickWeightElement(List<ElementWeight> elements) {
        float total = 0f;
        foreach (var e in elements) total += e.Weight;

        float random = Random.Range(0, total);
        foreach (var e in elements) {
            random -= e.Weight;
            if (random <= 0) return e.Element;
        }

        return ElementType.None;
    }
}