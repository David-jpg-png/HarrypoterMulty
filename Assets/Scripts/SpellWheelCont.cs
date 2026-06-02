using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpellWheelCont : MonoBehaviour
{
    public int id;
    private Animator anim;
    public string itemName;
    public TextMeshProUGUI itemText;
    public Image selectedItem;
    private bool selected = false;
    public Sprite icon;

    void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if (selected)
        {
            selectedItem.sprite = icon;
            itemText.text = itemName;
        }
    }

    public void Select()
    {
        selected = true;
        SelectSpell.weaponID = id;
    }

    public void Deselect()
    {
        selected = false;
        SelectSpell.weaponID = 0;
    }

    public void HoverEnter()
    {
        if (anim != null) anim.SetBool("Hover", true);
        if (itemText != null) itemText.text = itemName;
    }

    public void HoverExit()
    {
        if (anim != null) anim.SetBool("Hover", false);
        if (itemText != null) itemText.text = "";
    }
}
