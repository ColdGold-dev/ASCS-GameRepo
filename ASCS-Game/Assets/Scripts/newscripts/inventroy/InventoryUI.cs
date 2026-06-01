using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Manages the visible inventory panel.
// IMPORTANT: This script should be on the Canvas itself (which stays active),
// NOT on the Panel (which gets toggled off). Otherwise pressing Tab the second
// time would never be detected because the script's GameObject was disabled.
public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Inventory data source (usually on the Player)")]
    public Inventory inventory;

    [Tooltip("The panel GameObject to show/hide when toggled")]
    public GameObject panel;

    [Tooltip("The transform that holds the slot UIs (has a Grid Layout Group)")]
    public Transform slotsParent;

    [Tooltip("Prefab for one slot UI")]
    public GameObject slotPrefab;

    [Header("Input")]
    public Key toggleKey = Key.Tab;

    private List<InventorySlotUI> slotUIs = new List<InventorySlotUI>();

    private void Start()
    {
        BuildSlots();
        if (inventory != null)
        {
            inventory.OnInventoryChanged += Refresh;
        }
        Refresh();
        if (panel != null) panel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.OnInventoryChanged -= Refresh;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null) return;
        if (Keyboard.current[toggleKey].wasPressedThisFrame)
        {
            Debug.Log("TAB pressed - toggling inventory");
            panel.SetActive(!panel.activeSelf);
        }
    }

    private void BuildSlots()
    {
        if (slotsParent == null || slotPrefab == null || inventory == null) return;

        foreach (Transform child in slotsParent)
        {
            Destroy(child.gameObject);
        }
        slotUIs.Clear();

        for (int i = 0; i < inventory.slots.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, slotsParent);
            InventorySlotUI ui = go.GetComponent<InventorySlotUI>();
            if (ui != null) slotUIs.Add(ui);
        }
    }

    public void Refresh()
    {
        if (inventory == null) return;
        for (int i = 0; i < slotUIs.Count && i < inventory.slots.Count; i++)
        {
            slotUIs[i].SetSlot(inventory.slots[i].item, inventory.slots[i].count);
        }
    }
}