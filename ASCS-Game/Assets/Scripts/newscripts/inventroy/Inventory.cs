using System.Collections.Generic;
using UnityEngine;

// Holds the player's inventory. Pure data and logic - no UI, no input.
// The UI listens to OnInventoryChanged to refresh.
public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public int count;

        public bool IsEmpty => item == null || count <= 0;

        public void Clear()
        {
            item = null;
            count = 0;
        }
    }

    [Header("Setup")]
    [Tooltip("How many slots in the grid")]
    public int slotCount = 20;

    public List<Slot> slots = new List<Slot>();

    // Fires whenever something changes (add, remove, swap)
    public System.Action OnInventoryChanged;

    private void Awake()
    {
        // Initialize slots list if not pre-populated
        while (slots.Count < slotCount) slots.Add(new Slot());
    }

    // Tries to add an item. Returns the amount that DIDN'T fit (0 means everything fit).
    public int AddItem(Item item, int amount = 1)
    {
        if (item == null || amount <= 0) return amount;

        int remaining = amount;

        // First pass: top up existing stacks of the same item
        if (item.stackable)
        {
            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                if (slots[i].item == item && slots[i].count < item.maxStack)
                {
                    int canFit = item.maxStack - slots[i].count;
                    int toAdd = Mathf.Min(canFit, remaining);
                    slots[i].count += toAdd;
                    remaining -= toAdd;
                }
            }
        }

        // Second pass: drop the rest into empty slots
        for (int i = 0; i < slots.Count && remaining > 0; i++)
        {
            if (slots[i].IsEmpty)
            {
                int toAdd = item.stackable ? Mathf.Min(item.maxStack, remaining) : 1;
                slots[i].item = item;
                slots[i].count = toAdd;
                remaining -= toAdd;
            }
        }

        if (remaining < amount) OnInventoryChanged?.Invoke();
        return remaining;
    }

    // Remove count from a specific slot. Returns true if removal succeeded.
    public bool RemoveFromSlot(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;
        Slot s = slots[slotIndex];
        if (s.IsEmpty || s.count < amount) return false;

        s.count -= amount;
        if (s.count <= 0) s.Clear();
        OnInventoryChanged?.Invoke();
        return true;
    }

    // Swap the contents of two slots (used for drag-drop reorder)
    public void SwapSlots(int a, int b)
    {
        if (a < 0 || b < 0 || a >= slots.Count || b >= slots.Count || a == b) return;
        Slot temp = new Slot { item = slots[a].item, count = slots[a].count };
        slots[a].item = slots[b].item;
        slots[a].count = slots[b].count;
        slots[b].item = temp.item;
        slots[b].count = temp.count;
        OnInventoryChanged?.Invoke();
    }

    // How many of a given item across all slots
    public int CountOf(Item item)
    {
        if (item == null) return 0;
        int total = 0;
        foreach (Slot s in slots) if (s.item == item) total += s.count;
        return total;
    }

    public bool Has(Item item, int amount = 1) => CountOf(item) >= amount;
}
