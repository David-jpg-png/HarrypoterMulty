using UnityEngine;
using UnityEngine.UI;

public class LifeBarr : MonoBehaviour
{
    public Image lifeBar;
    private PMovement playerMovement;
    public float maxHealth;

    void Start()
    {
        playerMovement = GameObject.Find("PC").GetComponent<PMovement>();
        maxHealth = playerMovement.maxHealth;
    }
    void Update()
    {
        lifeBar.fillAmount = playerMovement.health / maxHealth;
    }
}
