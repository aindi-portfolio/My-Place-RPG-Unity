using UnityEngine;

/// <summary>
/// Base class for all items in the game.
/// ScriptableObject allows us to create item data assets in the Unity editor
/// that can be reused across the project without being tied to GameObjects.
/// 
/// This is perfect for RPG items because:
/// - Items can exist as data without being in the scene
/// - Easy to create new items using Unity's inspector
/// - Memory efficient (shared data, not duplicated per instance)
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "My Place RPG/Items/Base Item")]
public class Item : ScriptableObject
{
    [Header("Basic Information")]
    [Tooltip("The name that will be displayed to the player")]
    public string itemName = "New Item";
    
    [Tooltip("Description shown in tooltips and item details")]
    [TextArea(2, 4)]
    public string description = "A mysterious item.";
    
    [Tooltip("Icon shown in inventory and UI")]
    public Sprite icon;
    
    [Header("Item Properties")]
    [Tooltip("How much this item is worth when selling")]
    public int value = 1;
    
    [Tooltip("Can this item be stacked in inventory?")]
    public bool isStackable = true;
    
    [Tooltip("Maximum number that can be in one stack")]
    public int maxStackSize = 99;
    
    [Tooltip("What type of item this is (affects where it can be used)")]
    public ItemType itemType = ItemType.Resource;
    
    [Header("Rarity")]
    [Tooltip("How rare this item is (affects UI colors and sell prices)")]
    public ItemRarity rarity = ItemRarity.Common;
    
    /// <summary>
    /// Gets the display name for UI purposes
    /// Virtual allows child classes to override this behavior
    /// </summary>
    public virtual string GetDisplayName()
    {
        return itemName;
    }
    
    /// <summary>
    /// Gets the full description including any dynamic information
    /// Virtual allows child classes to add extra info
    /// </summary>
    public virtual string GetDescription()
    {
        return description;
    }
    
    /// <summary>
    /// Gets the sell value, potentially modified by rarity
    /// </summary>
    public virtual int GetSellValue()
    {
        // Multiply base value by rarity multiplier
        float multiplier = GetRarityMultiplier();
        return Mathf.RoundToInt(value * multiplier);
    }
    
    /// <summary>
    /// Helper method to get the rarity multiplier for calculations
    /// </summary>
    private float GetRarityMultiplier()
    {
        switch (rarity)
        {
            case ItemRarity.Common: return 1.0f;
            case ItemRarity.Uncommon: return 1.5f;
            case ItemRarity.Rare: return 2.0f;
            case ItemRarity.Epic: return 3.0f;
            case ItemRarity.Legendary: return 5.0f;
            default: return 1.0f;
        }
    }
}

/// <summary>
/// Enum defining the different types of items in our game
/// This helps organize items and determine where they can be used
/// </summary>
public enum ItemType
{
    Resource,    // Basic materials like wood, stone, ore
    Tool,        // Items used for gathering like pickaxe, axe
    Consumable,  // Items that are used up like potions, food
    Equipment,   // Gear that can be equipped like armor, weapons
    QuestItem,   // Special items for quests
    Crafted      // Items created through crafting
}

/// <summary>
/// Enum for item rarity levels
/// Affects item colors, sell prices, and drop rates
/// </summary>
public enum ItemRarity
{
    Common,      // White - basic items
    Uncommon,    // Green - slightly better
    Rare,        // Blue - good items
    Epic,        // Purple - very good items
    Legendary    // Orange/Gold - best items
}