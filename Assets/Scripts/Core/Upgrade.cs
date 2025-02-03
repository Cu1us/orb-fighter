using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Upgrade")]
public class Upgrade : ScriptableObject
{
    public Sprite Icon;

    public float MaxHealthIncrease;
    public float AttackDamageIncrease;
    public float MassIncrease;

    public void ApplyToSpawner(OrbSpawner spawner)
    {
        spawner.ApplyUpgradeStats(this);
    }
}
