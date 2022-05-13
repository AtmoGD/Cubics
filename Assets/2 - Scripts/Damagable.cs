using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damagable
{
    void TakeDamage(float damage);
    void Die(float delay, bool addMana);
}
