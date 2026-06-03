using UnityEngine;

public class SpellProjectile : MonoBehaviour
{
    public float damage = 25f;
    public float lifeTime = 3f;

    private void Start()
    {
        // Destroy the projectile after lifeTime seconds to prevent memory leaks
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore trigger zones (like checkpoints, victory zones, etc.) and the player
        if (other.isTrigger || other.CompareTag("Player"))
        {
            return;
        }

        // Try to find the EControl component on the hit object or its parent
        EControl enemy = other.GetComponent<EControl>();
        if (enemy == null)
        {
            enemy = other.GetComponentInParent<EControl>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Destroy the projectile on impact
        Destroy(gameObject);
    }
}
