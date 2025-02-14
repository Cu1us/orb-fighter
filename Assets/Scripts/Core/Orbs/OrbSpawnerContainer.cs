using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbSpawnerContainer : MonoBehaviour
{
    public List<OrbSpawner> Spawners = new();
    public bool Visible { get; private set; } = true;

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
}
