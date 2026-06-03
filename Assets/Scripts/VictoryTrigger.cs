using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance != null)
            {
                if (GameManager.Instance.voldemortDefeated)
                {
                    Debug.Log("VictoryTrigger: Player grabbed the central vision and Voldemort is defeated! Triggering victory.");
                    GameManager.Instance.TriggerVictory();
                }
                else
                {
                    Debug.Log("VictoryTrigger: Player reached the vision, but Voldemort is still alive. Defeat Voldemort first!");
                    // We can display a warning in the console, and later link it to UI if needed
                }
            }
        }
    }
}
