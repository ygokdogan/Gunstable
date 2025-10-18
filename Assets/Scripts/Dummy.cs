using Interfaces;
using UnityEngine;

public class Dummy : MonoBehaviour, IDamageable
{
    public int health;
    
    public void Damage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
