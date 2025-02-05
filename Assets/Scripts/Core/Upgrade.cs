using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    [TextArea] public string Description;

    public int Cost;
    public int SlotsReq;
    public int MaxInstancesPerOrb;

    public bool AddBehavior;
    public OrbBehavior BehaviorToAdd;
    public int BehaviorLevel;
    public bool UpgradeableBehavior = true;

    public bool AddStats;
    public float MaxHealthIncrease;
    public float AttackDamageIncrease;
    public float MassIncrease;

    public void ApplyToSpawner(OrbSpawner spawner)
    {
        spawner.AddUpgrade(this);
    }
    public bool CanApplyTo(OrbSpawner spawner)
    {
        return spawner.CanAddUpgrade(this);
    }
}
