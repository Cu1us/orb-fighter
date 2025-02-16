using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public LayerMask LayersThatBlockOrbPlacement;
    public OrbSpawner DefaultOrbSpawnerPrefab;
    public AssetMap UpgradeIDMap;
}
