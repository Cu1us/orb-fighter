using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Asset Map")]
public class AssetMap : ScriptableObject
{
    [SerializeField] bool Preload;
    [SerializeField] UpgradeMap[] Maps;
    [field: NonSerialized] public bool Loaded { get; private set; } = false;

    // There is apparently no better way to make a bi-directional dictionary than using two dictionaries
    [NonSerialized] public Dictionary<string, Upgrade> IDToUpgradeMap;
    [NonSerialized] public Dictionary<Upgrade, string> UpgradeToIDMap;


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
        foreach (UpgradeMap upgradeMap in Maps)
        {
            bool success = IDToUpgradeMap.TryAdd(upgradeMap.Key, upgradeMap.Value) & UpgradeToIDMap.TryAdd(upgradeMap.Value, upgradeMap.Key);
            if (!success)
            {
                Debug.LogWarning($"Duplicate upgrade map found when setting up upgrade '{upgradeMap.Value.name}': Duplicate ID or Upgrade in the list.");
            }
        }
        Loaded = true;
    }

    public Upgrade GetUpgrade(string id)
    {
        if (!Loaded) SetupMap();
        return IDToUpgradeMap[id];
    }
    public bool TryGetUpgrade(string id, out Upgrade upgrade)
    {
        if (!Loaded) SetupMap();
        return IDToUpgradeMap.TryGetValue(id, out upgrade);
    }
    public string GetID(Upgrade upgrade)
    {
        if (!Loaded) SetupMap();
        return UpgradeToIDMap[upgrade];
    }
    public bool TryGetID(Upgrade upgrade, out string id)
    {
        if (!Loaded) SetupMap();
        return UpgradeToIDMap.TryGetValue(upgrade, out id);
    }
}

[Serializable]
public record UpgradeMap { [SerializeField] public string Key; [SerializeField] public Upgrade Value; }