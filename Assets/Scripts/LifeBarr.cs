using UnityEngine;
using UnityEngine.UI;

public class LifeBarr : MonoBehaviour
{
    public Image lifeBar;
    private PMovement playerMovement;
    public float maxHealth;

    void Start()
    {
        playerMovement = Object.FindObjectOfType<PMovement>();
        if (playerMovement != null)
        {
            maxHealth = playerMovement.maxHealth;
        }
        else
        {
            Debug.LogError("LifeBarr: No PMovement component found in the scene!");
        }
    }
    void Update()
    {
        if (playerMovement != null && lifeBar != null && maxHealth > 0f)
        {
            lifeBar.fillAmount = playerMovement.health / maxHealth;
        }
    }
}
