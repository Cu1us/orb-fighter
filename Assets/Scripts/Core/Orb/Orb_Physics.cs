using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Orb physics
public partial class Orb
{
    public Vector2 StartVelocity;
    public float maxSpeed;

    public void Takeoff()
    {
        GameManager.Instance.onTakeoffAllOrbs -= Takeoff;
        rigidbody.simulated = true;
        ApplyStartVelocity();
        onTakeoff?.Invoke();
    }

    void ApplyStartVelocity()
    {
        rigidbody.velocity = Vector2.zero;
        rigidbody.AddForce(StartVelocity, ForceMode2D.Impulse);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        onAnyCollision?.Invoke(collision);
        if (collision.gameObject.CompareTag("Orb") && collision.gameObject.TryGetComponent(out Orb otherOrb))
        {
            ContactPoint2D contact = collision.GetContact(0);
            OrbCollisionInfo orbCollisionInfo = new(otherOrb, contact.normal, contact.point, collision.relativeVelocity);
            onOrbCollision?.Invoke(orbCollisionInfo);
            if (IsEnemy(otherOrb))
            {
                MeleeHitOrb(otherOrb);
            }
        }
    }

    public void Launch(Vector2 launchVector, float maxSpeedCap = 1f)
    {
        float effectiveMaxSpeed = maxSpeed * maxSpeedCap;
        if (rigidbody.velocity.sqrMagnitude > effectiveMaxSpeed * effectiveMaxSpeed)
        {
            return;
        }
        Vector2 newVelocity = rigidbody.velocity + launchVector;
        if (newVelocity.sqrMagnitude > effectiveMaxSpeed * effectiveMaxSpeed)
        {
            newVelocity = newVelocity.normalized * effectiveMaxSpeed;
        }
        rigidbody.velocity = newVelocity;
    }
}
