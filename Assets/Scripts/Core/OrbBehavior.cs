using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OrbBehavior : ScriptableObject
{
    [NonSerialized] public Orb orb;
    [NonSerialized] public int level;
    [NonSerialized] public bool enabled = true;

    [field: SerializeField] public OrbInfo Info { get; private set; }

    public void Initialize(Orb parent)
    {
        orb = parent;
        Setup();
    }

    protected abstract void Setup();

    public virtual string GetDescription()
    {
        return Info == null ? "No description" : Info.Description;
    }
}
