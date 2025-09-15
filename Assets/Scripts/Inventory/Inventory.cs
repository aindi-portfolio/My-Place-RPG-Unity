using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages the player's inventory system.
/// This is the heart of item management in our RPG.
/// 
/// Key features:
/// - Stores items and their quantities
/// - Handles item stacking
/// - Provides methods to add, remove, and check items
/// - Fires events when inventory changes (so UI can update)
/// 
/// This class uses Events to communicate with the UI, which is a clean way
/// to keep the inventory logic separate from the display logic.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [Tooltip("Maximum number of different item slots")]
    public int maxSlots = 30;
    
    [Tooltip("Should inventory changes be logged to console? (Helpful for debugging)")]
    public bool debugMode = false;
    
    // Dictionary stores items and their quantities
    // Dictionary is perfect because it's fast to look up items by their reference
    private Dictionary<Item, int> items = new Dictionary<Item, int>();
    
    // Events allow other scripts (like UI) to react when inventory changes
    // Think of events like a notification system - when something happens here,
    // other scripts that are "listening" get notified automatically
    public static event Action<Item, int> OnItemAdded;     // Fired when items are added
    public static event Action<Item, int> OnItemRemoved;   // Fired when items are removed
    public static event Action OnInventoryChanged;        // Fired when anything changes
    
    /// <summary>
    /// Gets the current number of unique items in inventory
    /// </summary>
    public int CurrentSlotCount => items.Count;
    
    /// <summary>
    /// Gets read-only access to all items in inventory
    /// ReadOnlyDictionary prevents other scripts from directly modifying our inventory
    /// </summary>
    public IReadOnlyDictionary<Item, int> Items => items;
    
    /// <summary>
    /// Adds items to the inventory
    /// Returns the number of items that couldn't be added (due to space limits)
    /// </summary>
    public int AddItem(Item item, int quantity = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Trying to add null item to inventory!");
            return quantity; // Return all as "couldn't add"
        }
        
        if (quantity <= 0)
        {
            Debug.LogWarning($"Trying to add {quantity} of {item.itemName}. Quantity must be positive!");
            return quantity;
        }
        
        int remainingToAdd = quantity;
        
        // If item already exists in inventory
        if (items.ContainsKey(item))
        {
            if (item.isStackable)
            {
                // Calculate how many we can add to existing stack
                int currentAmount = items[item];
                int spaceInStack = item.maxStackSize - currentAmount;
                int toAddToStack = Mathf.Min(remainingToAdd, spaceInStack);
                
                items[item] = currentAmount + toAddToStack;
                remainingToAdd -= toAddToStack;
                
                if (debugMode)
                    Debug.Log($"Added {toAddToStack} {item.itemName} to existing stack. Now have {items[item]}");
            }
            else
            {
                // Non-stackable items need separate slots
                // For now, we don't add more if it already exists
                // In a more complex system, you might want multiple slots for non-stackable items
                Debug.LogWarning($"{item.itemName} is not stackable and already exists in inventory!");
                return quantity;
            }
        }
        else
        {
            // Item doesn't exist yet - check if we have space for a new slot
            if (CurrentSlotCount >= maxSlots)
            {
                Debug.Log("Inventory is full! Cannot add new items.");
                return quantity; // Return all as "couldn't add"
            }
            
            // Add new item
            int toAddToNewSlot = item.isStackable ? 
                Mathf.Min(remainingToAdd, item.maxStackSize) : 
                1;
            
            items[item] = toAddToNewSlot;
            remainingToAdd -= toAddToNewSlot;
            
            if (debugMode)
                Debug.Log($"Added {toAddToNewSlot} {item.itemName} as new item. {remainingToAdd} couldn't be added.");
        }
        
        // Fire events to notify other systems (like UI)
        int actuallyAdded = quantity - remainingToAdd;
        if (actuallyAdded > 0)
        {
            OnItemAdded?.Invoke(item, actuallyAdded);
            OnInventoryChanged?.Invoke();
        }
        
        return remainingToAdd; // Return how many couldn't be added
    }
    
    /// <summary>
    /// Removes items from inventory
    /// Returns true if the full amount was removed, false if there wasn't enough
    /// </summary>
    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            Debug.LogWarning("Invalid item or quantity for removal!");
            return false;
        }
        
        if (!items.ContainsKey(item))
        {
            Debug.LogWarning($"Trying to remove {item.itemName} but it's not in inventory!");
            return false;
        }
        
        int currentAmount = items[item];
        if (currentAmount < quantity)
        {
            Debug.LogWarning($"Not enough {item.itemName}! Have {currentAmount}, trying to remove {quantity}");
            return false;
        }
        
        // Remove the items
        int newAmount = currentAmount - quantity;
        if (newAmount <= 0)
        {
            // Remove item completely if quantity reaches 0
            items.Remove(item);
            if (debugMode)
                Debug.Log($"Removed all {item.itemName} from inventory");
        }
        else
        {
            items[item] = newAmount;
            if (debugMode)
                Debug.Log($"Removed {quantity} {item.itemName}. {newAmount} remaining");
        }
        
        // Fire events
        OnItemRemoved?.Invoke(item, quantity);
        OnInventoryChanged?.Invoke();
        
        return true;
    }
    
    /// <summary>
    /// Checks if inventory contains at least the specified amount of an item
    /// </summary>
    public bool HasItem(Item item, int quantity = 1)
    {
        if (item == null) return false;
        
        return items.ContainsKey(item) && items[item] >= quantity;
    }
    
    /// <summary>
    /// Gets the current quantity of a specific item
    /// Returns 0 if item is not in inventory
    /// </summary>
    public int GetItemCount(Item item)
    {
        if (item == null) return 0;
        
        return items.ContainsKey(item) ? items[item] : 0;
    }
    
    /// <summary>
    /// Clears all items from inventory
    /// Useful for testing or when starting a new game
    /// </summary>
    public void ClearInventory()
    {
        items.Clear();
        OnInventoryChanged?.Invoke();
        
        if (debugMode)
            Debug.Log("Inventory cleared!");
    }
    
    /// <summary>
    /// Gets a formatted string showing all items in inventory
    /// Useful for debugging or simple text-based displays
    /// </summary>
    public string GetInventoryDisplayText()
    {
        if (items.Count == 0)
        {
            return "Inventory is empty";
        }
        
        string display = $"Inventory ({CurrentSlotCount}/{maxSlots} slots):\n";
        foreach (var kvp in items)
        {
            Item item = kvp.Key;
            int quantity = kvp.Value;
            display += $"• {item.itemName} x{quantity}\n";
        }
        
        return display.TrimEnd('\n');
    }
}