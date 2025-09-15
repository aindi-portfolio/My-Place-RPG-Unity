using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the crafting system in our RPG.
/// Handles recipe discovery, crafting attempts, and player progression.
/// 
/// This works with CraftingRecipe ScriptableObjects to provide a flexible
/// crafting system that's easy to expand and modify.
/// </summary>
public class CraftingManager : MonoBehaviour
{
    [Header("Crafting Settings")]
    [Tooltip("All available crafting recipes in the game")]
    public List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();
    
    [Tooltip("Recipes the player has discovered")]
    public List<CraftingRecipe> knownRecipes = new List<CraftingRecipe>();
    
    [Tooltip("Player's current crafting level")]
    public int craftingLevel = 1;
    
    [Tooltip("Current crafting experience points")]
    public int craftingExperience = 0;
    
    [Tooltip("Experience needed for next level")]
    public int experienceToNextLevel = 100;
    
    [Header("References")]
    [Tooltip("Player's inventory for checking materials")]
    public Inventory playerInventory;
    
    [Tooltip("Currency manager for crafting costs")]
    public CurrencyManager currencyManager;
    
    // Events for UI updates
    public static System.Action<CraftingRecipe> OnRecipeDiscovered;
    public static System.Action<CraftingRecipe, bool> OnCraftingAttempted;  // recipe, success
    public static System.Action<int, int> OnCraftingLevelChanged;           // oldLevel, newLevel
    
    /// <summary>
    /// Initialize the crafting system
    /// </summary>
    void Start()
    {
        // Find references if not assigned
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();
        
        if (currencyManager == null)
            currencyManager = FindObjectOfType<CurrencyManager>();
        
        // Give player some basic starting recipes
        DiscoverStartingRecipes();
        
        Debug.Log($"Crafting Manager initialized. Level {craftingLevel}, {knownRecipes.Count} recipes known.");
    }
    
    /// <summary>
    /// Give the player some basic recipes to start with
    /// </summary>
    void DiscoverStartingRecipes()
    {
        // Discover all recipes that don't require a crafting level higher than 1
        foreach (CraftingRecipe recipe in allRecipes)
        {
            if (recipe.minimumCraftingLevel <= craftingLevel)
            {
                DiscoverRecipe(recipe, false); // Don't announce starting recipes
            }
        }
    }
    
    /// <summary>
    /// Discover a new recipe
    /// </summary>
    public void DiscoverRecipe(CraftingRecipe recipe, bool announce = true)
    {
        if (recipe == null) return;
        
        if (!knownRecipes.Contains(recipe))
        {
            knownRecipes.Add(recipe);
            
            if (announce)
            {
                Debug.Log($"Discovered new recipe: {recipe.recipeName}!");
                OnRecipeDiscovered?.Invoke(recipe);
            }
        }
    }
    
    /// <summary>
    /// Attempt to craft a recipe
    /// </summary>
    public bool AttemptCraft(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            Debug.LogWarning("Cannot craft null recipe!");
            return false;
        }
        
        // Check if player knows this recipe
        if (!knownRecipes.Contains(recipe))
        {
            Debug.Log($"You don't know how to craft {recipe.recipeName}!");
            return false;
        }
        
        // Check crafting level requirement
        if (craftingLevel < recipe.minimumCraftingLevel)
        {
            Debug.Log($"You need crafting level {recipe.minimumCraftingLevel} to craft {recipe.recipeName}!");
            return false;
        }
        
        // Check if player has required materials
        if (!recipe.CanCraft(playerInventory))
        {
            Debug.Log($"You don't have the required materials for {recipe.recipeName}!");
            Debug.Log(recipe.GetRequirementsText());
            return false;
        }
        
        // Attempt the craft (check for success/failure)
        bool craftingSuccess = recipe.AttemptCraft(craftingLevel);
        
        if (craftingSuccess)
        {
            // Success! Remove materials and give result
            ConsumeMaterials(recipe);
            GiveResult(recipe);
            GainCraftingExperience(recipe.experienceReward);
            
            Debug.Log($"Successfully crafted {recipe.resultQuantity} {recipe.resultItem.itemName}!");
        }
        else
        {
            // Failure! Still consume materials (could be changed based on game design)
            ConsumeMaterials(recipe);
            GainCraftingExperience(recipe.experienceReward / 2); // Half XP for failure
            
            Debug.Log($"Crafting failed! Materials were consumed but nothing was created.");
        }
        
        OnCraftingAttempted?.Invoke(recipe, craftingSuccess);
        return craftingSuccess;
    }
    
    /// <summary>
    /// Remove required materials from inventory
    /// </summary>
    void ConsumeMaterials(CraftingRecipe recipe)
    {
        foreach (ItemRequirement requirement in recipe.requiredItems)
        {
            playerInventory.RemoveItem(requirement.item, requirement.quantity);
        }
    }
    
    /// <summary>
    /// Give the crafted item to the player
    /// </summary>
    void GiveResult(CraftingRecipe recipe)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddItemToPlayer(recipe.resultItem, recipe.resultQuantity);
        }
        else
        {
            playerInventory.AddItem(recipe.resultItem, recipe.resultQuantity);
        }
    }
    
    /// <summary>
    /// Add crafting experience and check for level up
    /// </summary>
    void GainCraftingExperience(int experience)
    {
        craftingExperience += experience;
        Debug.Log($"Gained {experience} crafting experience! ({craftingExperience}/{experienceToNextLevel})");
        
        // Check for level up
        CheckForLevelUp();
    }
    
    /// <summary>
    /// Check if player should level up and handle the level up
    /// </summary>
    void CheckForLevelUp()
    {
        while (craftingExperience >= experienceToNextLevel)
        {
            // Level up!
            int oldLevel = craftingLevel;
            craftingLevel++;
            craftingExperience -= experienceToNextLevel;
            
            // Increase XP requirement for next level
            experienceToNextLevel = Mathf.RoundToInt(experienceToNextLevel * 1.2f);
            
            Debug.Log($"Crafting level up! Now level {craftingLevel}");
            OnCraftingLevelChanged?.Invoke(oldLevel, craftingLevel);
            
            // Discover new recipes unlocked by level up
            DiscoverRecipesForLevel(craftingLevel);
        }
    }
    
    /// <summary>
    /// Discover recipes that are now available at this level
    /// </summary>
    void DiscoverRecipesForLevel(int level)
    {
        foreach (CraftingRecipe recipe in allRecipes)
        {
            if (recipe.minimumCraftingLevel == level && !knownRecipes.Contains(recipe))
            {
                DiscoverRecipe(recipe, true);
            }
        }
    }
    
    /// <summary>
    /// Get all recipes the player can currently craft
    /// </summary>
    public List<CraftingRecipe> GetCraftableRecipes()
    {
        List<CraftingRecipe> craftable = new List<CraftingRecipe>();
        
        foreach (CraftingRecipe recipe in knownRecipes)
        {
            if (craftingLevel >= recipe.minimumCraftingLevel && recipe.CanCraft(playerInventory))
            {
                craftable.Add(recipe);
            }
        }
        
        return craftable;
    }
    
    /// <summary>
    /// Get all known recipes regardless of whether they can be crafted right now
    /// </summary>
    public List<CraftingRecipe> GetKnownRecipes()
    {
        return new List<CraftingRecipe>(knownRecipes);
    }
    
    /// <summary>
    /// Check if a specific recipe can be crafted right now
    /// </summary>
    public bool CanCraftRecipe(CraftingRecipe recipe)
    {
        if (recipe == null) return false;
        
        return knownRecipes.Contains(recipe) && 
               craftingLevel >= recipe.minimumCraftingLevel && 
               recipe.CanCraft(playerInventory);
    }
    
    /// <summary>
    /// Get crafting progress information
    /// </summary>
    public string GetCraftingProgressText()
    {
        return $"Crafting Level: {craftingLevel}\n" +
               $"Experience: {craftingExperience}/{experienceToNextLevel}\n" +
               $"Known Recipes: {knownRecipes.Count}/{allRecipes.Count}";
    }
    
    /// <summary>
    /// Debug method to add experience
    /// </summary>
    [ContextMenu("Add 50 XP (Debug)")]
    public void AddDebugExperience()
    {
        GainCraftingExperience(50);
    }
}