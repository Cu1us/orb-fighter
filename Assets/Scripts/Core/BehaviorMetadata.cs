using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Behavior Metadata")]
public class BehaviorMetadata : ScriptableObject
{
    [Header("Visuals")]
    public string Name;
    [TextArea] public string Description;
    public Sprite Icon;

    [Header("Settings")]
    public int MaxLevel;
}
