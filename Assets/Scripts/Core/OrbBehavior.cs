using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbBehavior : ScriptableObject
{
    [NonSerialized] public Orb orb;
    [NonSerialized] public int level;
    [NonSerialized] public bool enabled = true;

    [field: SerializeField] public BehaviorMetadata Metadata { get; private set; }

    public void Initialize(Orb parent)
    {
        orb = parent;
        Setup();
    }

    protected abstract void Setup();

    public virtual string GetDescription()
    {
        return Metadata == null ? "No description" : Metadata.Description;
    }
}

public struct BehaviorParameters
{
    public int level;
    public BehaviorParameters(int level)
    {
        this.level = level;
    }
}