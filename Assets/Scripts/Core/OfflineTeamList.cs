using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Offline Team List")]
public class OfflineTeamList : ScriptableObject
{
    public SerializableTeam[] Teams;

    public SerializableTeam GetRandomTeam()
    {
        return Teams[Random.Range(0, Teams.Length)];
    }
}