using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AttackTarget : InteractableBody
{
    public int initialHealth;
    public int health;

    public virtual void GetDamage(int amount)
    {
        this.health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    public abstract void Die();
}
