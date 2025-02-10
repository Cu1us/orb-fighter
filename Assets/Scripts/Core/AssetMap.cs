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
            bool success = UpgradeMap.TryAdd(upgradeMap.Key, upgradeMap.Value);
            if (!success)
            {
                Debug.LogWarning($"Duplicate upgrade map found when setting up upgrade '{upgradeMap.Value.name}': Another upgrade already has the ID '{upgradeMap.Key}'");
            }
        }
        Loaded = true;
    }
    public Upgrade Get(string id)
    {
        if (!Loaded) SetupMap();
        return UpgradeMap[id];
    }
    public bool TryGet(string id, out Upgrade upgrade)
    {
        if (!Loaded) SetupMap();
        return UpgradeMap.TryGetValue(id, out upgrade);
    }

    public Dictionary<string, Upgrade> UpgradeMap;
}

[System.Serializable]
public record UpgradeMap(string Key, Upgrade Value);

// https://developercommunity.visualstudio.com/t/error-cs0518-predefined-type-systemruntimecompiler/1244809
// Issue with compiling down to .NET 4. from a higher version (?); this class needs to be manually defined. Apparently a bug with Visual Studio.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}