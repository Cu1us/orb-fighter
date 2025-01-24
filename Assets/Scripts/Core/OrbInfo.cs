using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Info")]
public class OrbInfo : ScriptableObject
{
    [Header("Visuals")]
    public string Name;
    [TextArea] public string Description;
    public Sprite Icon;

    [Header("Settings")]
    public int MaxLevel;
}
