using UnityEngine;

/// <summary>
/// Example of a resource node that players can gather materials from.
/// This could be a tree, rock, ore node, etc.
/// 
/// Implements IInteractable so the player can interact with it.
/// This is a great template for creating different types of gathering nodes.
/// </summary>
public class ResourceNode : MonoBehaviour, IInteractable
{
    [Header("Resource Information")]
    [Tooltip("What resource this node gives when gathered")]
    public ResourceItem resourceItem;
    
    [Tooltip("How many times this node can be gathered before it's depleted")]
    public int maxHarvestCount = 3;
    
    [Tooltip("Current number of times this has been harvested")]
    public int currentHarvestCount = 0;
    
    [Header("Visual Feedback")]
    [Tooltip("Object to show when player is near (interaction prompt)")]
    public GameObject interactionPrompt;
    
    [Tooltip("Visual representation that changes based on depletion")]
    public SpriteRenderer nodeRenderer;
    
    [Tooltip("Sprites for different depletion states")]
    public Sprite[] depletionStages;
    
    private bool isPlayerNear = false;
    
    /// <summary>
    /// Initialize the node
    /// </summary>
    void Start()
    {
        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Set initial visual state
        UpdateVisuals();
    }
    
    /// <summary>
    /// Called when player interacts with this node
    /// </summary>
    public void Interact(PlayerController player)
    {
        if (resourceItem == null)
        {
            Debug.LogWarning($"ResourceNode {name} has no resource item assigned!");
            return;
        }
        
        // Check if node is depleted
        if (currentHarvestCount >= maxHarvestCount)
        {
            Debug.Log($"This {resourceItem.itemName} source is depleted!");
            return;
        }
        
        // Try to gather the resource
        AttemptGather();
    }
    
    /// <summary>
    /// Attempt to gather resources from this node
    /// </summary>
    void AttemptGather()
    {
        // For now, assume player always has the right tool
        // In a more complex system, you'd check the player's equipped tool
        int toolLevel = 1;  // Basic tool level
        
        // Check if the resource can be gathered (tool requirements, etc.)
        if (!resourceItem.CanGatherWith(ToolType.None, toolLevel))
        {
            Debug.Log($"You need a {resourceItem.requiredTool} to gather {resourceItem.itemName}!");
            return;
        }
        
        // Calculate how much to gather
        int amountGathered = resourceItem.CalculateGatherAmount(toolLevel);
        
        // Add to player's inventory
        GameManager gameManager = GameManager.Instance;
        if (gameManager != null && gameManager.AddItemToPlayer(resourceItem, amountGathered))
        {
            // Successfully gathered!
            Debug.Log($"Gathered {amountGathered} {resourceItem.itemName}!");
            
            // Increase harvest count
            currentHarvestCount++;
            UpdateVisuals();
            
            // Check if node is now depleted
            if (currentHarvestCount >= maxHarvestCount)
            {
                Debug.Log($"{resourceItem.itemName} source is now depleted.");
                OnNodeDepleted();
            }
        }
        else
        {
            Debug.Log("Inventory is full! Cannot gather more items.");
        }
    }
    
    /// <summary>
    /// Called when the node becomes depleted
    /// </summary>
    void OnNodeDepleted()
    {
        // Could disable interaction, change appearance, start respawn timer, etc.
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        // Maybe start a respawn timer
        Invoke(nameof(RespawnNode), 30f);  // Respawn after 30 seconds
    }
    
    /// <summary>
    /// Respawn this node after it's been depleted
    /// </summary>
    void RespawnNode()
    {
        currentHarvestCount = 0;
        UpdateVisuals();
        Debug.Log($"{resourceItem.itemName} source has respawned!");
    }
    
    /// <summary>
    /// Update the visual appearance based on depletion state
    /// </summary>
    void UpdateVisuals()
    {
        if (nodeRenderer == null || depletionStages.Length == 0) return;
        
        // Calculate which sprite to use based on depletion
        float depletionPercent = (float)currentHarvestCount / maxHarvestCount;
        int spriteIndex = Mathf.FloorToInt(depletionPercent * depletionStages.Length);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, depletionStages.Length - 1);
        
        nodeRenderer.sprite = depletionStages[spriteIndex];
    }
    
    /// <summary>
    /// Called when player enters interaction range
    /// </summary>
    public void OnPlayerEnter()
    {
        isPlayerNear = true;
        
        // Show interaction prompt if node isn't depleted
        if (currentHarvestCount < maxHarvestCount && interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }
        
        // Could add highlighting, sound effects, etc.
        Debug.Log($"Can gather {resourceItem?.itemName ?? "resources"} here");
    }
    
    /// <summary>
    /// Called when player leaves interaction range
    /// </summary>
    public void OnPlayerExit()
    {
        isPlayerNear = false;
        
        // Hide interaction prompt
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
    }
    
    /// <summary>
    /// Debug info for the inspector
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Show depletion status
        Gizmos.color = currentHarvestCount >= maxHarvestCount ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        // Show harvest count
        if (Application.isPlaying)
        {
#if UNITY_EDITOR
            UnityEditor.Handles.Label(transform.position + Vector3.up, 
                $"{currentHarvestCount}/{maxHarvestCount}");
#endif
        }
    }
}