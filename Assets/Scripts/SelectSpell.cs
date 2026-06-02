using UnityEngine;
using UnityEngine.UI;

public class SelectSpell : MonoBehaviour
{
    private bool weaponSelected = false;
    public Image selectedItem;
    public Sprite noImage;
    public static int weaponID;

    void Start()
    {
        UpdateSelected();
    }

    public void UpdateSelected()
    {
        switch (weaponID)
        {
            case 0:
                if (selectedItem != null)
                    selectedItem.sprite = noImage;
                break;
            case 1:
                Debug.Log("double");
                break;
            case 2:
                Debug.Log("");
                break;
            case 3:
                Debug.Log("");
                break;
            case 4:
                Debug.Log("");
                break;
            default:
                Debug.Log("Unknown weaponID: " + weaponID);
                break;
        }
    }
}
