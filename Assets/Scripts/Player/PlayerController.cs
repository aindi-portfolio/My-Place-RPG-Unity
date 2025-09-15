using UnityEngine;

/// <summary>
/// Controls the player character in our RPG game.
/// Handles movement, interaction with objects, and basic player actions.
/// 
/// This is a simple 2D controller that's perfect for a beginner RPG.
/// It uses Unity's built-in physics system for movement and collision detection.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]  // Automatically adds Rigidbody2D component
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("How fast the player moves")]
    public float moveSpeed = 5f;
    
    [Tooltip("Should movement feel smooth or snappy?")]
    public bool smoothMovement = true;
    
    [Tooltip("How quickly player stops when not moving (only used if smoothMovement is true)")]
    public float movementSmoothing = 0.1f;
    
    [Header("Interaction Settings")]
    [Tooltip("How close player needs to be to interact with objects")]
    public float interactionRange = 2f;
    
    [Tooltip("What layers contain interactable objects")]
    public LayerMask interactableLayerMask = 1;
    
    [Header("References")]
    [Tooltip("Visual representation of the player (sprite, model, etc.)")]
    public GameObject playerVisual;
    
    // Components
    private Rigidbody2D rb2d;           // For physics movement
    private Vector2 movement;           // Current movement input
    private Vector2 smoothedMovement;   // Smoothed movement for smooth controls
    
    // Interaction
    private IInteractable currentInteractable;  // What the player can currently interact with
    
    /// <summary>
    /// Get components and initialize
    /// </summary>
    void Start()
    {
        // Get the Rigidbody2D component
        rb2d = GetComponent<Rigidbody2D>();
        
        // Set up physics for top-down movement
        rb2d.gravityScale = 0f;  // No gravity in top-down view
        rb2d.freezeRotation = true;  // Don't let physics rotate the player
        
        Debug.Log("Player initialized and ready!");
    }
    
    /// <summary>
    /// Handle input every frame
    /// Update() is called once per frame and is perfect for input handling
    /// </summary>
    void Update()
    {
        HandleMovementInput();
        HandleInteractionInput();
        CheckForInteractables();
    }
    
    /// <summary>
    /// Handle physics movement
    /// FixedUpdate() is called at fixed intervals and is perfect for physics
    /// </summary>
    void FixedUpdate()
    {
        MovePlayer();
    }
    
    /// <summary>
    /// Get movement input from player
    /// </summary>
    void HandleMovementInput()
    {
        // Get input from arrow keys or WASD
        float horizontal = Input.GetAxisRaw("Horizontal");  // A/D or Left/Right arrows
        float vertical = Input.GetAxisRaw("Vertical");      // W/S or Up/Down arrows
        
        // Create movement vector
        movement = new Vector2(horizontal, vertical);
        
        // Normalize diagonal movement so player doesn't move faster diagonally
        // Without this, moving diagonally would be 1.4x faster than moving straight
        if (movement.magnitude > 1f)
        {
            movement = movement.normalized;
        }
    }
    
    /// <summary>
    /// Handle interaction input
    /// </summary>
    void HandleInteractionInput()
    {
        // Check if player pressed interaction key (Space or E)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }
    
    /// <summary>
    /// Move the player using physics
    /// </summary>
    void MovePlayer()
    {
        Vector2 targetVelocity = movement * moveSpeed;
        
        if (smoothMovement)
        {
            // Smooth movement - feels more polished
            smoothedMovement = Vector2.Lerp(smoothedMovement, targetVelocity, movementSmoothing);
            rb2d.velocity = smoothedMovement;
        }
        else
        {
            // Direct movement - more responsive
            rb2d.velocity = targetVelocity;
        }
        
        // Flip player visual based on movement direction (optional)
        if (playerVisual != null && movement.x != 0)
        {
            // Flip the visual to face movement direction
            Vector3 scale = playerVisual.transform.localScale;
            scale.x = movement.x > 0 ? 1 : -1;  // Face right if moving right, left if moving left
            playerVisual.transform.localScale = scale;
        }
    }
    
    /// <summary>
    /// Check for nearby interactable objects
    /// </summary>
    void CheckForInteractables()
    {
        // Find all colliders within interaction range
        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(transform.position, interactionRange, interactableLayerMask);
        
        IInteractable closestInteractable = null;
        float closestDistance = float.MaxValue;
        
        // Find the closest interactable object
        foreach (Collider2D obj in nearbyObjects)
        {
            IInteractable interactable = obj.GetComponent<IInteractable>();
            if (interactable != null)
            {
                float distance = Vector2.Distance(transform.position, obj.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestInteractable = interactable;
                }
            }
        }
        
        // Update current interactable
        if (currentInteractable != closestInteractable)
        {
            // Stop highlighting old interactable
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerExit();
            }
            
            // Start highlighting new interactable
            currentInteractable = closestInteractable;
            if (currentInteractable != null)
            {
                currentInteractable.OnPlayerEnter();
            }
        }
    }
    
    /// <summary>
    /// Try to interact with the current interactable object
    /// </summary>
    void TryInteract()
    {
        if (currentInteractable != null)
        {
            Debug.Log($"Interacting with {currentInteractable}");
            currentInteractable.Interact(this);
        }
        else
        {
            Debug.Log("Nothing to interact with nearby");
        }
    }
    
    /// <summary>
    /// Get the player's current position
    /// Useful for other scripts that need to know where the player is
    /// </summary>
    public Vector2 GetPosition()
    {
        return transform.position;
    }
    
    /// <summary>
    /// Get the direction the player is facing
    /// Useful for placing objects, determining interaction direction, etc.
    /// </summary>
    public Vector2 GetFacingDirection()
    {
        if (movement.magnitude > 0.1f)
        {
            return movement.normalized;
        }
        
        // If not moving, face right by default
        return Vector2.right;
    }
    
    /// <summary>
    /// Draw interaction range in the Scene view for debugging
    /// Only visible when this object is selected in the editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // Draw interaction range as a wire circle
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCircle(transform.position, interactionRange);
        
        // Draw movement direction
        if (movement.magnitude > 0.1f)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, movement.normalized * 1.5f);
        }
    }
}

/// <summary>
/// Interface for objects that the player can interact with.
/// Any object that implements this interface can be interacted with by the player.
/// 
/// This is a great example of polymorphism - different objects can have different
/// interaction behaviors, but they all use the same interface.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Called when the player interacts with this object
    /// </summary>
    void Interact(PlayerController player);
    
    /// <summary>
    /// Called when the player enters interaction range
    /// Use this to highlight the object or show interaction prompts
    /// </summary>
    void OnPlayerEnter();
    
    /// <summary>
    /// Called when the player leaves interaction range
    /// Use this to remove highlights or hide interaction prompts
    /// </summary>
    void OnPlayerExit();
}