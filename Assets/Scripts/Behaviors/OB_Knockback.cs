using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Orb Behavior/Knockback")]
public class OB_Knockback : OrbBehavior
{
    [SerializeField] float knockbackForce;

    protected override void Setup()
    {
        orb.onOrbCollision += CollideWithOrb;
    }

    void CollideWithOrb(OrbCollisionInfo collision)
    {
        if (!enabled) return;
        Vector2 knockback = collision.normal * knockbackForce * Mathf.Pow(level + 1, 2);
        collision.otherOrb.Launch(-knockback);
        orb.Launch(knockback);
    }
}