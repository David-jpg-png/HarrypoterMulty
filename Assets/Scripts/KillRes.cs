using UnityEngine;

public class KillRes : MonoBehaviour
{
    public string playerTag = "Player";

    public string playerObjectName = "PC";

    private void OnTriggerEnter(Collider other)
    {
        TryKillPlayer(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryKillPlayer(collision.gameObject);
    }

    private void TryKillPlayer(GameObject other)
    {
        if (!other.CompareTag(playerTag) && other.name != playerObjectName)
            return;

        PMovement player = other.GetComponent<PMovement>();
        if (player == null)
        {
            GameObject playerObject = GameObject.Find(playerObjectName);
            if (playerObject != null)
                player = playerObject.GetComponent<PMovement>();
        }

        if (player != null)
        {
            player.Kill();
        }
    }
}
