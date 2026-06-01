using UnityEngine;
using UnityEngine.UI;

// Visual representation of one inventory slot.
// Shows an item icon (no count since items don't stack).
public class InventorySlotUI : MonoBehaviour
{
    [Header("References")]
    public Image iconImage;

    public void SetSlot(Item item, int count)
    {
        if (item == null || count <= 0)
        {
            iconImage.enabled = false;
            return;
        }
        iconImage.enabled = true;
        iconImage.sprite = item.icon;
    }
}