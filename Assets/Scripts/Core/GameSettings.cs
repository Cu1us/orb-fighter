using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public LayerMask layersThatBlockOrbPlacement;
    public AssetMap UpgradeIDMap;
}
