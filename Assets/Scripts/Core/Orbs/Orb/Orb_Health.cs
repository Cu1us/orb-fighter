using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Orb health logic
public partial class Orb
{
    public float Health;
    public float MaxHealth;
    public float AttackDamage;

    public void Damage(float damage, DamageCause cause = DamageCause.UNKNOWN, Orb damager = null)
    {
        DamageInfo damageInfo = new(damage, cause, damager);
        Health -= damage;
        onTakeDamage?.Invoke(damageInfo);
        DisplayHealthFractionOnMaterial();
        if (Health <= 0)
        {
            Health = 0;
            Die(damageInfo);
        }
    }

    public void DisplayHealthFractionOnMaterial()
    {
        renderer.material.SetFloat("_HealthFraction", Mathf.Clamp01((Health - 0.75f) / (MaxHealth - 0.75f)));
    }

    public void Die(DamageInfo damageInfo)
    {
        onDeath?.Invoke(damageInfo);
        Remove();
    }

    public void MeleeHitOrb(Orb toDamage)
    {
        toDamage.Damage(AttackDamage, DamageCause.MELEE, this);
    }
}
