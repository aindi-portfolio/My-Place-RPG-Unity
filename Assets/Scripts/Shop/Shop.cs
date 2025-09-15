using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Manages the shop system where players can buy and sell items.
/// This component should be attached to shop NPCs or shop objects in the world.
/// 
/// Features:
/// - Buy items with currency
/// - Sell items for currency
/// - Different shop types (general store, blacksmith, etc.)
/// - Stock management
/// - Price adjustments based on shop type
/// </summary>
public class Shop : MonoBehaviour
{
    [Header("Shop Information")]
    [Tooltip("Name of this shop shown to the player")]
    public string shopName = "General Store";
    
    [Tooltip("What type of shop this is (affects what items are available)")]
    public ShopType shopType = ShopType.General;
    
    [Tooltip("Greeting message when player opens shop")]
    public string shopGreeting = "Welcome to my shop!";
    
    [Header("Shop Settings")]
    [Tooltip("Multiplier for buying prices (1.0 = normal, 1.5 = 50% more expensive)")]
    [Range(0.1f, 5.0f)]
    public float buyPriceMultiplier = 1.2f;
    
    [Tooltip("Multiplier for selling prices (1.0 = full value, 0.5 = half value)")]
    [Range(0.1f, 1.0f)]
    public float sellPriceMultiplier = 0.7f;
    
    [Tooltip("Items this shop sells and their stock")]
    public List<ShopItem> shopInventory = new List<ShopItem>();
    
    [Tooltip("What item types this shop will buy from the player")]
    public List<ItemType> acceptedItemTypes = new List<ItemType>();
    
    // Events for shop interactions
    public static event Action<Item, int, int> OnItemSold;      // item, quantity, price
    public static event Action<Item, int, int> OnItemBought;    // item, quantity, price
    public static event Action<Shop> OnShopOpened;
    public static event Action<Shop> OnShopClosed;
    
    private bool isShopOpen = false;
    
    /// <summary>
    /// Opens the shop interface
    /// </summary>
    public void OpenShop()
    {
        if (isShopOpen) return;
        
        isShopOpen = true;
        Debug.Log($"Opening {shopName}: {shopGreeting}");
        
        OnShopOpened?.Invoke(this);
    }
    
    /// <summary>
    /// Closes the shop interface
    /// </summary>
    public void CloseShop()
    {
        if (!isShopOpen) return;
        
        isShopOpen = false;
        Debug.Log($"Closing {shopName}");
        
        OnShopClosed?.Invoke(this);
    }
    
    /// <summary>
    /// Attempts to buy an item from the shop
    /// Returns true if purchase was successful
    /// </summary>
    public bool BuyItem(Item item, int quantity, Inventory playerInventory, CurrencyManager currencyManager)
    {
        if (!isShopOpen)
        {
            Debug.LogWarning("Shop is not open!");
            return false;
        }
        
        // Find the item in shop inventory
        ShopItem shopItem = shopInventory.Find(si => si.item == item);
        if (shopItem == null)
        {
            Debug.Log($"{shopName} doesn't sell {item.itemName}");
            return false;
        }
        
        // Check if shop has enough stock
        if (shopItem.currentStock < quantity)
        {
            Debug.Log($"{shopName} doesn't have enough {item.itemName}. Available: {shopItem.currentStock}");
            return false;
        }
        
        // Calculate total cost
        int itemPrice = GetBuyPrice(item);
        int totalCost = itemPrice * quantity;
        
        // Check if player has enough money
        if (!currencyManager.HasEnoughCurrency(totalCost))
        {
            Debug.Log($"Not enough money! Need {totalCost}, have {currencyManager.CurrentCurrency}");
            return false;
        }
        
        // Check if player has inventory space
        int leftover = playerInventory.AddItem(item, quantity);
        if (leftover > 0)
        {
            Debug.Log($"Not enough inventory space! {leftover} items couldn't be added");
            // Could handle partial purchases here
            return false;
        }
        
        // Complete the purchase
        currencyManager.SpendCurrency(totalCost);
        shopItem.currentStock -= quantity;
        
        Debug.Log($"Bought {quantity} {item.itemName} for {totalCost} coins");
        
        OnItemBought?.Invoke(item, quantity, totalCost);
        return true;
    }
    
    /// <summary>
    /// Attempts to sell an item to the shop
    /// Returns true if sale was successful
    /// </summary>
    public bool SellItem(Item item, int quantity, Inventory playerInventory, CurrencyManager currencyManager)
    {
        if (!isShopOpen)
        {
            Debug.LogWarning("Shop is not open!");
            return false;
        }
        
        // Check if shop accepts this item type
        if (!acceptedItemTypes.Contains(item.itemType))
        {
            Debug.Log($"{shopName} doesn't buy {item.itemType} items");
            return false;
        }
        
        // Check if player has the item
        if (!playerInventory.HasItem(item, quantity))
        {
            Debug.Log($"Don't have enough {item.itemName} to sell");
            return false;
        }
        
        // Calculate sell value
        int itemPrice = GetSellPrice(item);
        int totalValue = itemPrice * quantity;
        
        // Complete the sale
        if (playerInventory.RemoveItem(item, quantity))
        {
            currencyManager.AddCurrency(totalValue);
            
            Debug.Log($"Sold {quantity} {item.itemName} for {totalValue} coins");
            
            OnItemSold?.Invoke(item, quantity, totalValue);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Gets the price to buy an item from this shop
    /// </summary>
    public int GetBuyPrice(Item item)
    {
        int basePrice = item.GetSellValue();
        return Mathf.RoundToInt(basePrice * buyPriceMultiplier);
    }
    
    /// <summary>
    /// Gets the price this shop will pay for an item
    /// </summary>
    public int GetSellPrice(Item item)
    {
        int basePrice = item.GetSellValue();
        return Mathf.RoundToInt(basePrice * sellPriceMultiplier);
    }
    
    /// <summary>
    /// Restocks the shop (could be called periodically or when shop resets)
    /// </summary>
    public void RestockShop()
    {
        foreach (ShopItem shopItem in shopInventory)
        {
            shopItem.currentStock = shopItem.maxStock;
        }
        
        Debug.Log($"{shopName} has been restocked!");
    }
    
    /// <summary>
    /// Gets available items for purchase
    /// </summary>
    public List<ShopItem> GetAvailableItems()
    {
        List<ShopItem> available = new List<ShopItem>();
        
        foreach (ShopItem shopItem in shopInventory)
        {
            if (shopItem.currentStock > 0)
            {
                available.Add(shopItem);
            }
        }
        
        return available;
    }
}

/// <summary>
/// Represents an item in a shop's inventory
/// </summary>
[System.Serializable]
public class ShopItem
{
    [Tooltip("The item being sold")]
    public Item item;
    
    [Tooltip("Current number available for purchase")]
    public int currentStock;
    
    [Tooltip("Maximum stock when shop is restocked")]
    public int maxStock;
    
    [Tooltip("Custom price multiplier for this specific item")]
    public float priceMultiplier = 1.0f;
    
    public ShopItem(Item shopItem, int stock)
    {
        item = shopItem;
        currentStock = stock;
        maxStock = stock;
    }
}

/// <summary>
/// Different types of shops with different specialties
/// </summary>
public enum ShopType
{
    General,      // Buys and sells everything
    Blacksmith,   // Specializes in tools and weapons
    Alchemist,    // Specializes in potions and magical items
    Merchant,     // Good prices, wide selection
    Pawnshop,     // Buys everything but lower prices
    Specialty     // Focuses on specific item types
}