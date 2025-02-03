using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrade")]
public class Upgrade : ScriptableObject
{
    public Sprite Icon;
    public string Name;
    [TextArea] public string Description;

    public bool AddBehavior;
    public OrbBehavior BehaviorToAdd;
    public int BehaviorLevel;
    public bool UpgradeableBehavior = true;
    public int MaxBehaviorLevel;
    public bool CancelIfBehaviorCannotBeAdded = true;

    public bool AddStats;
    public float MaxHealthIncrease;
    public float AttackDamageIncrease;
    public float MassIncrease;

    public void ApplyToSpawner(OrbSpawner spawner)
    {
        spawner.ApplyUpgrade(this);
    }
    public bool CanApplyTo(OrbSpawner spawner)
    {
        if (AddBehavior && CancelIfBehaviorCannotBeAdded && spawner.Behaviors.ContainsKey(BehaviorToAdd))
        {
            int existingLevel = spawner.Behaviors[BehaviorToAdd].level;
            if (BehaviorLevel < existingLevel || (BehaviorLevel == existingLevel && !UpgradeableBehavior))
            {
                return false;
            }
        }
        return true;
    }
}
