using UnityEngine;

public class ItemCol : MonoBehaviour
{
    public float healAmount = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HealPotion"))
        {
            // Only pick up if this GameObject has a PMovement and isn't at max health
            PMovement pm = GetComponent<PMovement>();
            if (pm != null)
            {
                if (pm.health < pm.maxHealth)
                {
                    pm.health += healAmount;
                    pm.health = Mathf.Clamp(pm.health, 0, pm.maxHealth);
                    Destroy(other.gameObject);
                }
                // else: at max health, do not pick up
            }
            else
            {
                // Fallback: if no PMovement found, still destroy the potion
                Destroy(other.gameObject);
            }
        }
    }
}
