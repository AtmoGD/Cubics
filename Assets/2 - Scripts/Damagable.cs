using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damagable
{
    void TakeDamage(float damage);
    void Die(Vector2 knockback, float delay, bool addMana, bool addScore, bool removeFromList);
}
