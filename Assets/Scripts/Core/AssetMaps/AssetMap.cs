using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetMap<T> : ScriptableObject where T : ScriptableObject
{
    [SerializeField] bool Preload;
    [SerializeField] Map[] Maps;
    [field: NonSerialized] public bool Loaded { get; private set; } = false;

    // There is apparently no better way to make a bi-directional dictionary than using two dictionaries
    [NonSerialized] public Dictionary<string, T> FromIDMap = new();
    [NonSerialized] public Dictionary<T, string> ToIDMap = new();


    [RuntimeInitializeOnLoadMethod]
    void Initialize()
    {
        Loaded = false;
        if (Preload)
        {
            SetupMap();
        }
    }

    void SetupMap()
    {
        Debug.Log($"Setting up Asset Map '{name}'");
        foreach (Map map in Maps)
        {
            bool success = FromIDMap.TryAdd(map.Key, map.Value) & ToIDMap.TryAdd(map.Value, map.Key);
            if (!success)
            {
                Debug.LogWarning($"Duplicate map found in map {name} when mapping '{map.Value.name}': Duplicate ID or asset in the list.");
            }
        }
        Loaded = true;
    }

    public T GetFromID(string id)
    {
        if (!Loaded) SetupMap();
        return FromIDMap[id];
    }
    public bool TryGetFromID(string id, out T asset)
    {
        if (!Loaded) SetupMap();
        return FromIDMap.TryGetValue(id, out asset);
    }
    public string GetIDOf(T asset)
    {
        if (!Loaded) SetupMap();
        return ToIDMap[asset];
    }
    public bool TryGetIDOf(T asset, out string id)
    {
        if (!Loaded) SetupMap();
        return ToIDMap.TryGetValue(asset, out id);
    }

    [Serializable]
    public record Map { [SerializeField] public string Key; [SerializeField] public T Value; }
}