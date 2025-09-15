using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a crafting recipe in the game.
/// ScriptableObject so recipes can be created as assets in the Unity editor.
/// 
/// This defines what items are needed to craft something and what the result is.
/// Perfect for a crafting-focused RPG where players combine resources to make new items.
/// </summary>
[CreateAssetMenu(fileName = "New Recipe", menuName = "My Place RPG/Crafting/Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [Header("Recipe Information")]
    [Tooltip("The name of this recipe shown to the player")]
    public string recipeName = "New Recipe";
    
    [Tooltip("Description of what this recipe creates")]
    [TextArea(2, 3)]
    public string description = "A crafting recipe.";
    
    [Header("Requirements")]
    [Tooltip("What items and how many are needed to craft this")]
    public List<ItemRequirement> requiredItems = new List<ItemRequirement>();
    
    [Tooltip("What crafting station is needed? Leave empty if can craft anywhere.")]
    public CraftingStationType requiredStation = CraftingStationType.None;
    
    [Tooltip("Minimum crafting level required to use this recipe")]
    public int minimumCraftingLevel = 1;
    
    [Tooltip("How long this recipe takes to craft (in seconds)")]
    public float craftingTime = 3.0f;
    
    [Header("Results")]
    [Tooltip("What item is created when crafting succeeds")]
    public Item resultItem;
    
    [Tooltip("How many of the result item are created")]
    public int resultQuantity = 1;
    
    [Tooltip("Experience points gained for crafting this recipe")]
    public int experienceReward = 10;
    
    [Header("Success Rate")]
    [Tooltip("Base chance of success (0-100%). Higher level reduces failure chance.")]
    [Range(0f, 100f)]
    public float baseSuccessRate = 100f;
    
    /// <summary>
    /// Checks if the player has all required items in their inventory
    /// </summary>
    public bool CanCraft(Inventory playerInventory)
    {
        // Check each required item
        foreach (ItemRequirement requirement in requiredItems)
        {
            if (!playerInventory.HasItem(requirement.item, requirement.quantity))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Calculates success rate based on player's crafting level
    /// Higher level = better success rate
    /// </summary>
    public float CalculateSuccessRate(int playerCraftingLevel)
    {
        if (playerCraftingLevel < minimumCraftingLevel)
        {
            return 0f; // Can't even attempt if level too low
        }
        
        // Base success rate plus bonus for higher level
        float levelBonus = (playerCraftingLevel - minimumCraftingLevel) * 2f;
        float finalRate = Mathf.Min(100f, baseSuccessRate + levelBonus);
        
        return finalRate;
    }
    
    /// <summary>
    /// Attempts to craft this recipe
    /// Returns true if successful, false if failed
    /// </summary>
    public bool AttemptCraft(int playerCraftingLevel)
    {
        float successRate = CalculateSuccessRate(playerCraftingLevel);
        float roll = Random.Range(0f, 100f);
        
        return roll <= successRate;
    }
    
    /// <summary>
    /// Gets a user-friendly string listing all required items
    /// </summary>
    public string GetRequirementsText()
    {
        if (requiredItems.Count == 0)
        {
            return "No materials required";
        }
        
        string requirements = "Required materials:\n";
        foreach (ItemRequirement requirement in requiredItems)
        {
            requirements += $"• {requirement.item.itemName} x{requirement.quantity}\n";
        }
        
        return requirements.TrimEnd('\n');
    }
}

/// <summary>
/// Represents a single item requirement for a recipe
/// This is serializable so it shows up nicely in the Unity inspector
/// </summary>
[System.Serializable]
public class ItemRequirement
{
    [Tooltip("The item that is required")]
    public Item item;
    
    [Tooltip("How many of this item are needed")]
    public int quantity = 1;
    
    /// <summary>
    /// Constructor for easy creation in code
    /// </summary>
    public ItemRequirement(Item requiredItem, int requiredQuantity)
    {
        item = requiredItem;
        quantity = requiredQuantity;
    }
}

/// <summary>
/// Types of crafting stations in the game
/// Different recipes might require different stations
/// </summary>
public enum CraftingStationType
{
    None,           // Can craft anywhere (by hand)
    Workbench,      // Basic crafting table
    Forge,          // For metalworking
    Alchemy,        // For potions and magical items
    Kitchen,        // For cooking food
    Loom,           // For making cloth items
    Anvil,          // For advanced metalworking
    Enchanter       // For magical enhancements
}