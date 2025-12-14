using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class GameMath
{
    public static int GetBiasedInt(int min, int max, float bias)
    {
        if (min >= max) return min;

        bias = Mathf.Clamp01(bias);
        float r = Random.value;
        
        float exp = Mathf.Lerp(4f, 0.25f, bias);
        float t = Mathf.Pow(r, exp);
        return Mathf.RoundToInt(Mathf.Lerp(min, max, t));
    }
    
    public static T GetWeightedPrefab<T>(IList<(T item, float weight)> items)
    {
        float total = 0f;
        for (int i = 0; i < items.Count; i++)
            total += Mathf.Max(0f, items[i].weight);

        if (total <= 0f) throw new Exception("No positive weights.");

        float roll = Random.value * total;
        float acc = 0f;

        for (int i = 0; i < items.Count; i++)
        {
            acc += Mathf.Max(0f, items[i].weight);
            if (roll <= acc) return items[i].item;
        }

        return items[^1].item; // fallback
    }

}
