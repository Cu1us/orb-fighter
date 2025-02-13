using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSerializer
{
    public static string SerializePlayerTeam()
    {
        SerializableTeam team = GetSerializablePlayerTeam();
        return JsonUtility.ToJson(team);
    }
    public static SerializableTeam GetSerializablePlayerTeam()
    {
        List<SerializableOrbData> orbs = new(5);
        foreach (OrbSpawner spawner in GameManager.Instance.SpawnerContainer.Spawners)
        {
            if (spawner.OwnedByPlayer)
            {
                orbs.Add(GetSerializableOrbSpawner(spawner));
            }
        }
        SerializableTeam team = new(orbs.ToArray());
        return team;
    }
    public static SerializableOrbData GetSerializableOrbSpawner(OrbSpawner spawner)
    {
        Vector2 position = spawner.transform.position;
        Vector2 startVelocity = spawner.StartVelocityDir * spawner.StartVelocityMagnitude;
        List<string> upgrades = new();

        foreach (Upgrade upgrade in spawner.Upgrades)
        {
            if (GameManager.TryGetIDFromUpgrade(upgrade, out string id))
            {
                upgrades.Add(id);
            }
            else
            {
                Debug.LogWarning($"Failed to get an ID from upgrade '{upgrade.name}'. Is it not defined in the asset map?");
            }
        }

        return new(position, startVelocity, upgrades.ToArray());
    }
}

[System.Serializable]
public struct SerializableOrbData
{
    [SerializeField] public Vector2 position;
    [SerializeField] public Vector2 startVelocity;
    [SerializeField] public string[] upgrades;

    public SerializableOrbData(Vector2 position, Vector2 startVelocity, string[] upgrades)
    {
        this.position = position;
        this.startVelocity = startVelocity;
        this.upgrades = upgrades;
    }
}
[System.Serializable]
public struct SerializableTeam
{
    [SerializeField] public SerializableOrbData[] orbs;

    public SerializableTeam(SerializableOrbData[] orbs)
    {
        this.orbs = orbs;
    }
}