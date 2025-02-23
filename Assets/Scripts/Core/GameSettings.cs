using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public LayerMask LayersThatBlockOrbPlacement;
    public OrbSpawner DefaultOrbSpawnerPrefab;
    public AssetMap UpgradeIDMap;

    public OfflineTeamList[] OfflineTeamLists;

    public SerializableTeam GetOfflineTeam(int round) => OfflineTeamLists[Mathf.Clamp(round, 0, OfflineTeamLists.Length - 1)].GetRandomTeam();
}
