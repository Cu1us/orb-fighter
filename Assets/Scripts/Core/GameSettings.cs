using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Game configuration")]
    public LayerMask LayersThatBlockOrbPlacement;
    public Color PlayerOrbColor;
    public Color EnemyOrbColor;
    public int StartingCurrency;


    [Header("Default prefabs")]
    public OrbSpawner DefaultOrbSpawnerPrefab;

    [Header("Asset maps")]
    public UpgradeAssetMap UpgradeMap;
    public OrbTypeAssetMap OrbTypeMap;

    [Header("Offline teams")]
    public OfflineTeamList[] OfflineTeamLists;

    public SerializableTeam GetOfflineTeam(int round) => OfflineTeamLists[Mathf.Clamp(round, 0, OfflineTeamLists.Length - 1)].GetRandomTeam();
}
