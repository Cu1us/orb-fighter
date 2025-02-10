using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSerializer : MonoBehaviour
{
    public string SerializePlayerTeam()
    {

        return "";
    }
    public SerializableOrbData SerializeOrb(OrbSpawner spawner)
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

        return new SerializableOrbData(position, startVelocity, upgrades.ToArray());
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
    [SerializeField] public int round;
    [SerializeField] public SerializableOrbData orbs;
}