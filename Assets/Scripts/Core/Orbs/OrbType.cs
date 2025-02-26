using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Type")]
public class OrbType : ScriptableObject
{
    public Orb OrbPrefab;
    public string Name;
    public string Description;
    public int Cost;
    public Texture2D OrbIcon;
    public Sprite UIIcon;
    public int MaxSlots;
    public float StartingHealth;
    public float StartingAttackDamage;
    public OrbBehavior[] StartingBehaviors;
}
