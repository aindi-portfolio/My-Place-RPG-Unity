using UnityEngine;

/// <summary>
/// Main game manager that coordinates all the systems in our RPG.
/// This is the "central hub" that connects inventory, currency, crafting, etc.
/// 
/// Uses the Singleton pattern so there's only one GameManager and it can be
/// accessed from anywhere in the code. This is perfect for a manager that
/// needs to coordinate between different systems.
/// 
/// Think of this as the "brain" of your game that keeps track of everything.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("Is the game currently paused?")]
    public bool isPaused = false;
    
    [Tooltip("Current version of the game")]
    public string gameVersion = "1.0.0";
    
    // Singleton instance - allows access from anywhere with GameManager.Instance
    public static GameManager Instance { get; private set; }
    
    [Header("Core Systems")]
    [Tooltip("Reference to the player's inventory")]
    public Inventory playerInventory;
    
    [Tooltip("Reference to the currency manager")]
    public CurrencyManager currencyManager;
    
    // Player reference - will be set when player spawns
    private PlayerController player;
    
    /// <summary>
    /// Property to safely access the player
    /// </summary>
    public PlayerController Player => player;
    
    /// <summary>
    /// Set up the singleton and initialize core systems
    /// Awake() runs before Start(), perfect for initialization
    /// </summary>
    void Awake()
    {
        // Implement singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep this object when loading new scenes
            
            Debug.Log($"GameManager initialized - Version {gameVersion}");
        }
        else
        {
            // If another GameManager already exists, destroy this one
            Debug.LogWarning("Multiple GameManagers detected! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        
        // Initialize core systems
        InitializeSystems();
    }
    
    /// <summary>
    /// Initialize all the game systems
    /// </summary>
    void InitializeSystems()
    {
        // Find core systems if not assigned in inspector
        if (playerInventory == null)
            playerInventory = FindObjectOfType<Inventory>();
        
        if (currencyManager == null)
            currencyManager = FindObjectOfType<CurrencyManager>();
        
        // Subscribe to important events
        // This allows the GameManager to react to things happening in the game
        Inventory.OnInventoryChanged += OnInventoryChanged;
        CurrencyManager.OnCurrencyChanged += OnCurrencyChanged;
        
        Debug.Log("Core systems initialized");
    }
    
    /// <summary>
    /// Called when the game starts
    /// </summary>
    void Start()
    {
        // Find the player in the scene
        player = FindObjectOfType<PlayerController>();
        
        if (player == null)
        {
            Debug.LogWarning("No PlayerController found in scene!");
        }
        else
        {
            Debug.Log("Player found and registered with GameManager");
        }
        
        // Start the game
        StartNewGame();
    }
    
    /// <summary>
    /// Start a new game session
    /// </summary>
    public void StartNewGame()
    {
        Debug.Log("Starting new game...");
        
        // Reset all systems to starting state
        if (currencyManager != null)
            currencyManager.ResetCurrency();
        
        if (playerInventory != null)
            playerInventory.ClearInventory();
        
        // Give player some starting items (optional)
        GiveStartingItems();
        
        Debug.Log("New game started!");
    }
    
    /// <summary>
    /// Give the player some starting items
    /// You can customize this based on your game design
    /// </summary>
    void GiveStartingItems()
    {
        // You would load your starting items from ScriptableObjects here
        // For now, just log that we would give starting items
        Debug.Log("Starting items would be given here (create some Item ScriptableObjects first)");
        
        // Example of how you would give starting items:
        // if (playerInventory != null && someStartingItem != null)
        // {
        //     playerInventory.AddItem(someStartingItem, 5);
        // }
    }
    
    /// <summary>
    /// Pause or unpause the game
    /// </summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        
        // Unity's Time.timeScale controls how fast time moves
        // 0 = paused, 1 = normal speed
        Time.timeScale = isPaused ? 0f : 1f;
        
        Debug.Log(isPaused ? "Game paused" : "Game unpaused");
    }
    
    /// <summary>
    /// Toggle pause state
    /// </summary>
    public void TogglePause()
    {
        SetPaused(!isPaused);
    }
    
    /// <summary>
    /// Called when inventory changes
    /// </summary>
    void OnInventoryChanged()
    {
        // React to inventory changes here
        // Maybe update UI, check for quest completion, etc.
        Debug.Log("Inventory changed - GameManager notified");
    }
    
    /// <summary>
    /// Called when currency changes
    /// </summary>
    void OnCurrencyChanged(int oldAmount, int newAmount)
    {
        // React to currency changes here
        // Maybe unlock new shops, achievements, etc.
        Debug.Log($"Currency changed from {oldAmount} to {newAmount} - GameManager notified");
    }
    
    /// <summary>
    /// Clean up when object is destroyed
    /// </summary>
    void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        Inventory.OnInventoryChanged -= OnInventoryChanged;
        CurrencyManager.OnCurrencyChanged -= OnCurrencyChanged;
    }
    
    /// <summary>
    /// Handle input for game-wide controls
    /// </summary>
    void Update()
    {
        // Check for pause input (ESC key)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        
        // Add other global input handling here
        // Like opening inventory, accessing menus, etc.
    }
    
    /// <summary>
    /// Quick access method to add items to player inventory
    /// Other scripts can call this easily: GameManager.Instance.AddItemToPlayer(item, amount)
    /// </summary>
    public bool AddItemToPlayer(Item item, int quantity = 1)
    {
        if (playerInventory == null)
        {
            Debug.LogError("No player inventory found!");
            return false;
        }
        
        int leftover = playerInventory.AddItem(item, quantity);
        return leftover == 0; // Returns true if all items were added
    }
    
    /// <summary>
    /// Quick access method to give currency to player
    /// </summary>
    public void GiveCurrencyToPlayer(int amount)
    {
        if (currencyManager == null)
        {
            Debug.LogError("No currency manager found!");
            return;
        }
        
        currencyManager.AddCurrency(amount);
    }
}