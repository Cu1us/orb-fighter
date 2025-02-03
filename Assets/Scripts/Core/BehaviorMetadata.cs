using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Behavior Metadata")]
public class BehaviorMetadata : ScriptableObject
{
    [Header("Visuals")]
    [TextArea] public string Description;
    public Sprite Icon;
    [Header("VFX")]
    public OrbVFX VisualEffectPrefab;
    public bool ApplyVFXToSpawner;
}
