using System.Collections;
using UnityEngine;

public class EControl : MonoBehaviour
{
    public GameObject[] waypoints;
    public float enemspeed = 2f;
    public float touchDamage = 10f;
    int waypointIndex = 0;
    private Coroutine damageRoutine;

    void Update()
    {
        MoveEnem();
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
}
