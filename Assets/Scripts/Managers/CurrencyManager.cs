using UnityEngine;
using System;

/// <summary>
/// Manages the player's currency (money/coins) in the game.
/// This handles all money-related operations like earning, spending, and checking balances.
/// 
/// Uses events to notify other systems (like UI) when currency changes.
/// This keeps the currency logic separate from the display logic.
/// </summary>
public class CurrencyManager : MonoBehaviour
{
    [Header("Currency Settings")]
    [Tooltip("Starting amount of currency when game begins")]
    public int startingCurrency = 100;
    
    [Tooltip("Name of the currency (like 'Gold', 'Coins', 'Credits')")]
    public string currencyName = "Gold";
    
    [Tooltip("Should currency changes be logged to console?")]
    public bool debugMode = false;
    
    // Current amount of currency the player has
    private int currentCurrency;
    
    // Events to notify other systems when currency changes
    public static event Action<int, int> OnCurrencyChanged;  // oldAmount, newAmount
    public static event Action<int> OnCurrencyGained;        // amount gained
    public static event Action<int> OnCurrencySpent;         // amount spent
    
    /// <summary>
    /// Public property to safely access current currency amount
    /// ReadOnly from outside - can only be modified through this class's methods
    /// </summary>
    public int CurrentCurrency => currentCurrency;
    
    /// <summary>
    /// Initialize currency when the game starts
    /// </summary>
    void Start()
    {
        SetCurrency(startingCurrency);
        
        if (debugMode)
            Debug.Log($"Currency Manager initialized with {currentCurrency} {currencyName}");
    }
    
    /// <summary>
    /// Adds currency to the player's total
    /// </summary>
    public void AddCurrency(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"Trying to add {amount} currency. Amount must be positive!");
            return;
        }
        
        int oldAmount = currentCurrency;
        currentCurrency += amount;
        
        if (debugMode)
            Debug.Log($"Gained {amount} {currencyName}. Total: {currentCurrency}");
        
        // Fire events
        OnCurrencyGained?.Invoke(amount);
        OnCurrencyChanged?.Invoke(oldAmount, currentCurrency);
    }
    
    /// <summary>
    /// Attempts to spend currency
    /// Returns true if successful, false if not enough currency
    /// </summary>
    public bool SpendCurrency(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning($"Trying to spend {amount} currency. Amount must be positive!");
            return false;
        }
        
        if (currentCurrency < amount)
        {
            if (debugMode)
                Debug.Log($"Not enough {currencyName}! Have {currentCurrency}, need {amount}");
            return false;
        }
        
        int oldAmount = currentCurrency;
        currentCurrency -= amount;
        
        if (debugMode)
            Debug.Log($"Spent {amount} {currencyName}. Remaining: {currentCurrency}");
        
        // Fire events
        OnCurrencySpent?.Invoke(amount);
        OnCurrencyChanged?.Invoke(oldAmount, currentCurrency);
        
        return true;
    }
    
    /// <summary>
    /// Checks if the player has enough currency for a purchase
    /// </summary>
    public bool HasEnoughCurrency(int amount)
    {
        return currentCurrency >= amount;
    }
    
    /// <summary>
    /// Sets currency to a specific amount
    /// Useful for loading saved games or admin commands
    /// </summary>
    public void SetCurrency(int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning("Currency cannot be negative! Setting to 0.");
            amount = 0;
        }
        
        int oldAmount = currentCurrency;
        currentCurrency = amount;
        
        if (debugMode)
            Debug.Log($"Currency set to {currentCurrency} {currencyName}");
        
        OnCurrencyChanged?.Invoke(oldAmount, currentCurrency);
    }
    
    /// <summary>
    /// Gets formatted currency string for display
    /// Examples: "150 Gold", "1,250 Coins"
    /// </summary>
    public string GetFormattedCurrency()
    {
        return $"{currentCurrency:N0} {currencyName}";
    }
    
    /// <summary>
    /// Resets currency to starting amount
    /// Useful when starting a new game
    /// </summary>
    public void ResetCurrency()
    {
        SetCurrency(startingCurrency);
        
        if (debugMode)
            Debug.Log($"Currency reset to starting amount: {startingCurrency} {currencyName}");
    }
    
    /// <summary>
    /// For debugging: adds a large amount of currency
    /// </summary>
    [ContextMenu("Add 1000 Currency (Debug)")]
    public void AddDebugCurrency()
    {
        AddCurrency(1000);
    }
    
    /// <summary>
    /// Save currency data (you would implement this with your save system)
    /// </summary>
    public CurrencySaveData GetSaveData()
    {
        return new CurrencySaveData
        {
            amount = currentCurrency,
            currencyType = currencyName
        };
    }
    
    /// <summary>
    /// Load currency data (you would implement this with your save system)
    /// </summary>
    public void LoadSaveData(CurrencySaveData saveData)
    {
        currencyName = saveData.currencyType;
        SetCurrency(saveData.amount);
    }
}

/// <summary>
/// Data structure for saving/loading currency information
/// [System.Serializable] allows Unity to save this data
/// </summary>
[System.Serializable]
public class CurrencySaveData
{
    public int amount;
    public string currencyType;
}