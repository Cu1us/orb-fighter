using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawnerContainer : MonoBehaviour
{
    public List<OrbSpawner> Spawners = new();
    public bool Visible { get; private set; } = true;
    public Side spawnSide;
    public bool SpawnsEnemies;

    public void SpawnAll()
    {
        foreach (OrbSpawner spawner in Spawners)
        {
            spawner.Spawn();
        }
    }

    public void Hide()
    {
        foreach (OrbSpawner spawner in Spawners)
        {
            spawner.gameObject.SetActive(false);
        }
        Visible = false;
    }
    public void Show()
    {
        foreach (OrbSpawner spawner in Spawners)
        {
            spawner.gameObject.SetActive(true);
        }
        Visible = true;
    }

    public void AddSpawner(OrbSpawner toAdd)
    {
        Spawners.Add(toAdd);
        if (toAdd.gameObject.activeSelf != Visible)
        {
            toAdd.gameObject.SetActive(Visible);
        }
    }

    public void Clear()
    {
        foreach (OrbSpawner spawner in Spawners)
        {
            Destroy(spawner.gameObject);
        }
        Spawners.Clear();
    }

    public void SetupTeam(SerializableTeam team, bool clearPrevious = true)
    {
        if (clearPrevious) Clear();
        foreach (SerializableOrbSpawner orbData in team.orbs)
        {
            if (spawnSide == Side.RIGHT)
            {
                orbData.position.x = -orbData.position.x;
                orbData.startVelocity.x = -orbData.startVelocity.x;
            }
            OrbSpawner spawner = OrbSpawner.InstantiateSpawnerFromData(orbData, transform);
            spawner.OwnedByPlayer = !SpawnsEnemies;
            AddSpawner(spawner);
        }
    }

    public enum Side
    {
        LEFT,
        RIGHT
    }
}
