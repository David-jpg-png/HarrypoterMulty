using System.Collections;
using UnityEngine;

public class EControl : MonoBehaviour
{
    public GameObject[] waypoints;
    public float enemspeed = 2f;
    public float touchDamage = 10f;
    int waypointIndex = 0;
    private Coroutine damageRoutine;

    [Header("Health & Defeat")]
    public bool isVoldemort = false;
    public float health = 50f;

    void Start()
    {
        if (gameObject.name.Contains("Voldemort"))
        {
            isVoldemort = true;
            health = 200f; // Give Voldemort more health
        }
    }

    void Update()
    {
        MoveEnem();

        #if UNITY_EDITOR
        // Debug key for testing: press K to defeat Voldemort instantly in editor
        if (isVoldemort && Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(9999f);
        }
        #endif
    }

    void MoveEnem()
    {
        if (Vector3.Distance(transform.position, waypoints[waypointIndex].transform.position) < 0.1f)
        {
            waypointIndex++;
            if (waypointIndex >= waypoints.Length)
            {
                waypointIndex = 0;
            }
        }

        transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointIndex].transform.position, enemspeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PMovement player = other.GetComponent<PMovement>();
            if (player != null && damageRoutine == null)
            {
                damageRoutine = StartCoroutine(DealDamage(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine);
                damageRoutine = null;
            }
        }
    }

    private IEnumerator DealDamage(PMovement player)
    {
        while (true)
        {
            Vector3 dir3 = (player.transform.position - transform.position).normalized;
            Vector2 dir2 = new Vector2(dir3.x, dir3.z);
            player.TakeDamage(dir2, touchDamage);
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isVoldemort && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterVoldemortDefeated();
        }
        
        Debug.Log($"{gameObject.name} has been defeated!");
        Destroy(gameObject);
    }
}
