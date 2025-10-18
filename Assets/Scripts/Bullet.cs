using System;
using Interfaces;
using Player;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public BulletType type;

    private GameObject source;
    private Rigidbody2D rb;
    private Vector2 direction;
    public ParticleSystem particle;
    public float speed;

    private float lifeTime = 2f;
    private float lifeTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        rb.linearVelocity = direction * speed;

        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
    
    public void Setup(Vector2 direction, GameObject source)
    {
        this.direction = direction;
        this.direction.Normalize();

        this.source = source;

        transform.right = this.direction;

        lifeTimer = lifeTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other && source)
        {
            if (!other.gameObject.CompareTag(source.tag))
            {
                var damageable = other.gameObject.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    if (other.gameObject.TryGetComponent(out PlayerController player))
                    {
                        player.SetDamageSource(transform.position);
                    }
                    damageable.Damage(damage);
                }
                var ps = Instantiate(particle, transform.position, Quaternion.identity);
                ps.Play();
                Destroy(particle.gameObject, 0.5f);
                Destroy(gameObject);
            }
        }
    }

    public enum BulletType
    {
        Default,
        Pump,
        Recoil,
        Skip
    }
}