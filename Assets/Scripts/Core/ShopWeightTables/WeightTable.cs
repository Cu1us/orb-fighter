using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WeightTable<T> : ScriptableObject
{
    public Entry[] Entries;
    [NonSerialized] float TotalWeight = float.NaN;

    public T PickRandom()
    {
        if (float.IsNaN(TotalWeight))
            CalculateTotalWeight();
        float chosenWeight = Random.Range(0, TotalWeight);
        float currentWeight = 0;
        foreach (Entry entry in Entries)
        {
            currentWeight += entry.Weight;
            if (currentWeight >= chosenWeight)
            {
                return entry.Value;
            }
        }
        Debug.LogWarning($"Couldn't pick valid entry from weight map '{name}'!");
        return default;
    }

    public static T PickFromAll(IEnumerable<WeightTable<T>> combinedTables)
    {
        float combinedTotalWeight = 0;
        foreach (WeightTable<T> table in combinedTables)
        {
            if (float.IsNaN(table.TotalWeight))
                table.CalculateTotalWeight();
            combinedTotalWeight += table.TotalWeight;
        }
        float chosenWeight = Random.Range(0, combinedTotalWeight);
        float currentWeight = 0;
        foreach (WeightTable<T> table in combinedTables)
        {
            foreach (Entry entry in table.Entries)
            {
                currentWeight += entry.Weight;
                if (currentWeight >= chosenWeight)
                {
                    return entry.Value;
                }
            }
        }
        Debug.LogWarning($"Couldn't pick valid entry from collection of weight maps!");
        return default;
    }

    void CalculateTotalWeight()
    {
        float total = 0;
        foreach (Entry entry in Entries)
            total += entry.Weight;
        TotalWeight = total;
    }

    [Serializable]
    public record Entry { [SerializeField] public T Value; [SerializeField] public float Weight; }
}
