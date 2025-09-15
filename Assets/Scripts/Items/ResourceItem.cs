using UnityEngine;

/// <summary>
/// Specialized item class for resources that can be gathered from the world.
/// Inherits from Item but adds gathering-specific properties.
/// 
/// Examples: Wood from trees, Stone from rocks, Ore from mining nodes
/// </summary>
[CreateAssetMenu(fileName = "New Resource", menuName = "My Place RPG/Items/Resource")]
public class ResourceItem : Item
{
    [Header("Resource Properties")]
    [Tooltip("What tool is required to gather this resource? Leave empty if no tool needed.")]
    public ToolType requiredTool = ToolType.None;
    
    [Tooltip("Minimum tool level required to gather this resource")]
    public int minimumToolLevel = 1;
    
    [Tooltip("How long it takes to gather this resource (in seconds)")]
    public float gatherTime = 2.0f;
    
    [Tooltip("Minimum amount gathered per action")]
    public int minGatherAmount = 1;
    
    [Tooltip("Maximum amount gathered per action")]
    public int maxGatherAmount = 3;
    
    [Header("Spawn Information")]
    [Tooltip("How common this resource is in the world (0-100)")]
    [Range(0f, 100f)]
    public float spawnChance = 50f;
    
    /// <summary>
    /// Calculates how much of this resource should be gathered
    /// Takes into account tool quality, player level, etc.
    /// </summary>
    public int CalculateGatherAmount(int toolLevel = 1)
    {
        // Base random amount
        int baseAmount = Random.Range(minGatherAmount, maxGatherAmount + 1);
        
        // Bonus for better tools
        float toolBonus = 1.0f + (toolLevel * 0.1f);
        
        // Apply bonus and round down
        int finalAmount = Mathf.FloorToInt(baseAmount * toolBonus);
        
        return Mathf.Max(1, finalAmount); // Always at least 1
    }
    
    /// <summary>
    /// Checks if a tool can gather this resource
    /// </summary>
    public bool CanGatherWith(ToolType tool, int toolLevel)
    {
        // Check if correct tool type (or no tool required)
        if (requiredTool != ToolType.None && requiredTool != tool)
        {
            return false;
        }
        
        // Check tool level requirement
        return toolLevel >= minimumToolLevel;
    }
}

/// <summary>
/// Types of tools that can be used for gathering
/// </summary>
public enum ToolType
{
    None,        // No tool required (can gather by hand)
    Axe,         // For cutting trees, chopping wood
    Pickaxe,     // For mining stone and ore
    Shovel,      // For digging, gathering soil/clay
    Scythe,      // For harvesting plants and crops
    FishingRod,  // For catching fish
    Net          // For catching bugs or small creatures
}