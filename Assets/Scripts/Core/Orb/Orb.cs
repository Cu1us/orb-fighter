using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Core orb behavior. Other modules (partial class implementations) are split into other files in the same folder.
public partial class Orb : MonoBehaviour
{
    [Header("References")]
    public Transform VisualEffectsContainer;
    [HideInInspector] new public Rigidbody2D rigidbody;
    [HideInInspector] new public Collider2D collider;
    [HideInInspector] new public SpriteRenderer renderer;

    readonly List<OrbBehavior> ActiveBehaviors = new();

    readonly List<OrbVFX> ActiveVFX = new();

    [Header("Settings")]
    public bool OwnedByPlayer;

    #region Events
    public Action onSpawn;
    public Action onUpdate;
    public Action onFixedUpdate;
    public Action onTakeoff;
    public Action<Collision2D> onAnyCollision;
    public Action<OrbCollisionInfo> onOrbCollision;
    public Action<DamageInfo> onTakeDamage;
    public Action<DamageInfo> onDeath;
    public Action onDespawn;
    #endregion

    void Start()
    {
        onSpawn?.Invoke();
        GameManager.Instance.onTakeoffAllOrbs += Takeoff;
    }
    void Update()
    {
        onUpdate?.Invoke();
    }
    void FixedUpdate()
    {
        onFixedUpdate?.Invoke();
    }
    void OnDestroy()
    {
        onDespawn?.Invoke();
        if (GameManager.HasInstance)
        {
            GameManager.Instance.UnregisterOrb(this);
        }
        foreach (OrbBehavior behavior in ActiveBehaviors)
        {
            Destroy(behavior);
        }
    }

    public void SetColor(Color color)
    {
        renderer.color = color;
    }

    public OrbBehavior AddBehavior(OrbBehavior behaviorToClone, BehaviorOptions parameters = new())
    {
        OrbBehavior behavior = Instantiate(behaviorToClone);
        behavior.level = parameters.level;
        ActiveBehaviors.Add(behavior);
        if (behavior.Metadata != null && behavior.Metadata.VisualEffectPrefab)
        {
            Instantiate(behavior.Metadata.VisualEffectPrefab, VisualEffectsContainer);
        }
        behavior.Initialize(this);
        if (behavior.Metadata && behavior.Metadata.VisualEffectPrefab)
        {
            OrbVFX vfx = Instantiate(behavior.Metadata.VisualEffectPrefab, VisualEffectsContainer);
            ActiveVFX.Add(vfx);
        }
        return behavior;
    }

    public bool IsEnemy(Orb toCheck) => OwnedByPlayer != toCheck.OwnedByPlayer;
    public bool IsAlly(Orb toCheck) => OwnedByPlayer == toCheck.OwnedByPlayer;

    [ContextMenu("Reset private component references")]
    void Reset()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }
}

public struct OrbCollisionInfo
{
    public Orb otherOrb;
    public Vector2 normal;
    public Vector2 impactPoint;
    public Vector2 relativeVelocity;

    public OrbCollisionInfo(Orb otherOrb, Vector2 normal, Vector2 impactPoint, Vector2 relativeVelocity)
    {
        this.otherOrb = otherOrb;
        this.normal = normal;
        this.impactPoint = impactPoint;
        this.relativeVelocity = relativeVelocity;
    }
}
public struct DamageInfo
{
    float damage;
    public Orb damager;
    public DamageCause cause;

    public DamageInfo(float damage, DamageCause cause, Orb damager = null)
    {
        this.damage = damage;
        this.cause = cause;
        this.damager = damager;
    }
}
public enum DamageCause
{
    UNKNOWN = 0,
    MELEE
}