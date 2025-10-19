using System;
using Interfaces;
using Managers;
using Player;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public BulletType type;
    public AudioClip fireClip;

    private GameObject source;
    private Rigidbody2D rb;
    private Vector2 direction;
    public ParticleSystem particle;
    public float speed;

    public float lifeTime = 2f;
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
        direction.Normalize();
        this.source = source;

        transform.right = this.direction;

        lifeTimer = lifeTime;
        GameManager.instance.audioManager.Play("Fire");
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