using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple UI manager to display currency and basic inventory information.
/// This is a beginner-friendly UI system that updates automatically when game state changes.
/// 
/// Attach this to a UI Canvas or UI Manager object in your scene.
/// Make sure to assign the UI Text components in the Inspector.
/// </summary>
public class SimpleUIManager : MonoBehaviour
{
    [Header("Currency Display")]
    [Tooltip("Text component to show current currency amount")]
    public TextMeshProUGUI currencyText;
    
    [Header("Inventory Display")]
    [Tooltip("Text component to show inventory summary")]
    public TextMeshProUGUI inventoryText;
    
    [Tooltip("How often to update the inventory display (in seconds)")]
    public float inventoryUpdateRate = 1.0f;
    
    [Header("Interaction Prompts")]
    [Tooltip("Text shown when player can interact with something")]
    public TextMeshProUGUI interactionPrompt;
    
    [Tooltip("Text to show for interaction prompt")]
    public string promptText = "Press E to interact";
    
    // References to game systems
    private CurrencyManager currencyManager;
    private Inventory playerInventory;
    
    // For updating inventory display
    private float lastInventoryUpdate;
    
    /// <summary>
    /// Initialize the UI system
    /// </summary>
    void Start()
    {
        // Find game systems
        currencyManager = FindObjectOfType<CurrencyManager>();
        playerInventory = FindObjectOfType<Inventory>();
        
        // Subscribe to currency changes for real-time updates
        CurrencyManager.OnCurrencyChanged += UpdateCurrencyDisplay;
        
        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);
        
        // Initial display updates
        UpdateCurrencyDisplay(0, 0);
        UpdateInventoryDisplay();
        
        Debug.Log("Simple UI Manager initialized");
    }
    
    /// <summary>
    /// Update UI elements that need regular updates
    /// </summary>
    void Update()
    {
        // Update inventory display periodically
        if (Time.time - lastInventoryUpdate >= inventoryUpdateRate)
        {
            UpdateInventoryDisplay();
            lastInventoryUpdate = Time.time;
        }
    }
    
    /// <summary>
    /// Update the currency display when currency changes
    /// This method is called automatically thanks to the event subscription
    /// </summary>
    void UpdateCurrencyDisplay(int oldAmount, int newAmount)
    {
        if (currencyText != null && currencyManager != null)
        {
            currencyText.text = currencyManager.GetFormattedCurrency();
        }
    }
    
    /// <summary>
    /// Update the inventory display to show current items
    /// </summary>
    void UpdateInventoryDisplay()
    {
        if (inventoryText == null || playerInventory == null) return;
        
        // Get basic inventory info
        string displayText = $"Inventory ({playerInventory.CurrentSlotCount}/{playerInventory.maxSlots})";
        
        // Add items if inventory isn't empty
        if (playerInventory.CurrentSlotCount > 0)
        {
            displayText += "\\n";
            int itemsShown = 0;
            
            // Show first few items (to avoid UI overflow)
            foreach (var kvp in playerInventory.Items)
            {
                if (itemsShown >= 5) // Only show first 5 items
                {
                    int remaining = playerInventory.CurrentSlotCount - itemsShown;
                    displayText += $"... and {remaining} more";
                    break;
                }
                
                displayText += $"{kvp.Key.itemName}: {kvp.Value}\\n";
                itemsShown++;
            }
        }
        else
        {
            displayText += "\\nEmpty";
        }
        
        inventoryText.text = displayText;
    }
    
    /// <summary>
    /// Show interaction prompt
    /// Call this when player enters interaction range of an object
    /// </summary>
    public void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.text = promptText;
            interactionPrompt.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Hide interaction prompt
    /// Call this when player leaves interaction range
    /// </summary>
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Show a temporary message to the player
    /// Useful for feedback like "Item added!" or "Not enough materials!"
    /// </summary>
    public void ShowMessage(string message, float duration = 3f)
    {
        // For now, just log to console
        // In a full implementation, you'd show this in a message panel
        Debug.Log($"UI Message: {message}");
        
        // You could implement a message popup here
        // Create a message panel that appears and fades out after 'duration' seconds
    }
    
    /// <summary>
    /// Clean up event subscriptions when destroyed
    /// This prevents memory leaks and errors
    /// </summary>
    void OnDestroy()
    {
        CurrencyManager.OnCurrencyChanged -= UpdateCurrencyDisplay;
    }
}

/// <summary>
/// Simple extension of ResourceNode that integrates with the UI system
/// This shows how to connect your game objects with the UI
/// </summary>
public class UIAwareResourceNode : ResourceNode
{
    private SimpleUIManager uiManager;
    
    /// <summary>
    /// Find the UI manager when starting
    /// </summary>
    new void Start()
    {
        base.Start(); // Call the parent Start method
        uiManager = FindObjectOfType<SimpleUIManager>();
    }
    
    /// <summary>
    /// Show interaction prompt when player enters range
    /// </summary>
    public override void OnPlayerEnter()
    {
        base.OnPlayerEnter(); // Call the parent method
        
        if (uiManager != null)
        {
            uiManager.ShowInteractionPrompt();
        }
    }
    
    /// <summary>
    /// Hide interaction prompt when player exits range
    /// </summary>
    public override void OnPlayerExit()
    {
        base.OnPlayerExit(); // Call the parent method
        
        if (uiManager != null)
        {
            uiManager.HideInteractionPrompt();
        }
    }
}