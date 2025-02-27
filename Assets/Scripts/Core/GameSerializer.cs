using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameSerializer
{
    public static string SerializePlayerTeam(SerializableTeam team)
    {
        return JsonUtility.ToJson(team);
    }

    public static SerializableTeam GetSerializablePlayerTeam()
    {
        List<SerializableOrbSpawner> orbs = new(5);
        foreach (OrbSpawner spawner in GameManager.Instance.PlayerSpawnerContainer.Spawners)
        {
            if (spawner.OwnedByPlayer)
            {
                orbs.Add(GetSerializableOrbSpawner(spawner));
            }
        }
        SerializableTeam team = new(orbs.ToArray());
        return team;
    }

    public static SerializableOrbSpawner GetSerializableOrbSpawner(OrbSpawner spawner)
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

        return new(spawner.metadata, position, startVelocity, upgrades);
    }

    public static List<string> CustomParseJSONTeamKeyList(string json)
    {
        // Gets all text enclosed in " quotes from a json file
        // JsonUtility doesn't support dictionaries and the database seems to not support basic arrays (?) so custom parsing was the remaining option
        List<string> keyList = new();
        if (string.IsNullOrWhiteSpace(json)) return keyList;
        StringBuilder builder = new();
        bool insideString = false;
        foreach (char ch in json)
        {
            if (insideString)
            {
                if (ch == '"')
                {
                    keyList.Add(builder.ToString());
                    builder.Clear();
                    insideString = false;
                }
                else builder.Append(ch);
            }
            else if (ch == '"') insideString = true;
        }
        return keyList;
    }
}

[System.Serializable]
public class SerializableOrbSpawner
{
    [SerializeField] public string type;
    [SerializeField] public Vector2 position;
    [SerializeField] public Vector2 startVelocity;
    [SerializeField] public List<string> upgrades;

    public SerializableOrbSpawner(OrbType type, Vector2 position, Vector2 startVelocity, List<string> upgrades)
    {
        this.type = GameManager.Settings.OrbTypeMap.GetIDOf(type);
        this.position = position;
        this.startVelocity = startVelocity;
        this.upgrades = upgrades;
    }
}
[System.Serializable]
public class SerializableTeam
{
    [SerializeField] public SerializableOrbSpawner[] orbs;

    public SerializableTeam(SerializableOrbSpawner[] orbs)
    {
        this.orbs = orbs;
    }
}