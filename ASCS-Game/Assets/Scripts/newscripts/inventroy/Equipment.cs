using UnityEngine;

// Holds the currently-equipped weapon (and could be extended for armor, etc.)
// Goes on the same GameObject as Inventory (the Player).
// Other systems read EquippedWeapon to know what's equipped.
public class Equipment : MonoBehaviour
{
    [SerializeField] private Item _equippedWeapon;
    public Item EquippedWeapon
    {
        get { return _equippedWeapon; }
        private set
        {
            _equippedWeapon = value;
            OnEquipmentChanged?.Invoke();
        }
    }

    // Fires when something gets equipped/unequipped
    public System.Action OnEquipmentChanged;

    // Equip an item. If it's not a weapon, it's rejected.
    // Returns true on success.
    public bool Equip(Item item)
    {
        if (item == null) return false;
        if (item.itemType != ItemType.Weapon)
        {
            Debug.Log("Can't equip: " + item.displayName + " is not a weapon");
            return false;
        }

        EquippedWeapon = item;
        Debug.Log("Equipped: " + item.displayName);
        return true;
    }

    public void Unequip()
    {
        if (EquippedWeapon == null) return;
        Debug.Log("Unequipped: " + EquippedWeapon.displayName);
        EquippedWeapon = null;
    }

    // Convenience: total weapon damage from the equipped weapon (0 if nothing)
    public int GetWeaponDamage()
    {
        return EquippedWeapon != null ? EquippedWeapon.weaponDamage : 0;
    }
}
