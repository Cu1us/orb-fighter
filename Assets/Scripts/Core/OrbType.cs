using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Type")]
public class OrbType : ScriptableObject
{
    public Orb OrbPrefab;
    public int MaxSlots;
    public float StartingHealth;
    public float StartingAttackDamage;
    public OrbBehavior StartingBehavior;
}
