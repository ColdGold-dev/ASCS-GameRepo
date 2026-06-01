using UnityEngine;

// Defines an item type. Create instances in the Project window via
// Right-click -> Create -> Inventory -> Item.
// Each item asset is a TEMPLATE - many slots can hold "Health Potion",
// they all reference the same Item asset.
[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item", order = 0)]
public class Item : ScriptableObject
{
    [Header("Display")]
    public string displayName = "New Item";
    [TextArea] public string description;
    public Sprite icon;

    [Header("Stacking")]
    [Tooltip("Can multiple of this item stack in one slot?")]
    public bool stackable = true;

    [Tooltip("Max stack size if stackable. Ignored if not stackable.")]
    public int maxStack = 99;

    [Header("Type")]
    public ItemType itemType = ItemType.General;

    [Header("Usable (for potions etc.)")]
    [Tooltip("If true, the player can 'use' this item from the inventory (e.g. a potion).")]
    public bool isUsable = false;

    [Tooltip("Health restored when used. Only matters if isUsable=true.")]
    public int healAmount = 0;

    [Header("Weapon (only used if itemType is Weapon)")]
    [Tooltip("Damage this weapon adds when equipped")]
    public int weaponDamage = 0;
}

public enum ItemType
{
    General,    // potions, materials, anything
    Weapon,     // equippable in weapon slot
    Key         // quest / unlock items
}
