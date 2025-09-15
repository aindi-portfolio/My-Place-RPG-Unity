using UnityEngine;

/// <summary>
/// Helper script to set up a basic game scene for testing.
/// This creates the essential objects you need to test the RPG systems.
/// 
/// To use this:
/// 1. Create an empty GameObject in your scene
/// 2. Add this script to it
/// 3. Press the "Set Up Basic Scene" button in the Inspector
/// 4. Or call SetUpBasicScene() from another script
/// 
/// This is perfect for beginners who want to quickly test the systems
/// without having to manually set up all the objects.
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [Header("Setup Options")]
    [Tooltip("Should we create a player character?")]
    public bool createPlayer = true;
    
    [Tooltip("Should we create the game manager?")]
    public bool createGameManager = true;
    
    [Tooltip("Should we create basic UI?")]
    public bool createBasicUI = true;
    
    [Tooltip("Should we create some example resource nodes?")]
    public bool createResourceNodes = true;
    
    [Tooltip("How many example resource nodes to create")]
    public int resourceNodeCount = 5;
    
    /// <summary>
    /// Set up a basic scene for testing the RPG systems
    /// You can call this from the Inspector or from code
    /// </summary>
    [ContextMenu("Set Up Basic Scene")]
    public void SetUpBasicScene()
    {
        Debug.Log("Setting up basic RPG scene...");
        
        if (createGameManager)
        {
            CreateGameManager();
        }
        
        if (createPlayer)
        {
            CreatePlayer();
        }
        
        if (createBasicUI)
        {
            CreateBasicUI();
        }
        
        if (createResourceNodes)
        {
            CreateExampleResourceNodes();
        }
        
        Debug.Log("Basic scene setup complete! Press Play to test.");
    }
    
    /// <summary>
    /// Create the game manager with all required components
    /// </summary>
    void CreateGameManager()
    {
        // Check if GameManager already exists
        if (FindObjectOfType<GameManager>() != null)
        {
            Debug.Log("GameManager already exists, skipping creation.");
            return;
        }
        
        // Create GameManager object
        GameObject gameManagerObj = new GameObject("GameManager");
        gameManagerObj.AddComponent<GameManager>();
        gameManagerObj.AddComponent<Inventory>();
        gameManagerObj.AddComponent<CurrencyManager>();
        gameManagerObj.AddComponent<CraftingManager>();
        
        Debug.Log("GameManager created with Inventory, Currency, and Crafting systems.");
    }
    
    /// <summary>
    /// Create a basic player character
    /// </summary>
    void CreatePlayer()
    {
        // Check if player already exists
        if (FindObjectOfType<PlayerController>() != null)
        {
            Debug.Log("Player already exists, skipping creation.");
            return;
        }
        
        // Create player object
        GameObject playerObj = new GameObject("Player");
        playerObj.transform.position = Vector3.zero;
        
        // Add required components
        playerObj.AddComponent<PlayerController>();
        playerObj.AddComponent<Rigidbody2D>();
        playerObj.AddComponent<CircleCollider2D>();
        
        // Set up physics
        Rigidbody2D rb = playerObj.GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        
        // Create visual child object
        GameObject playerVisual = new GameObject("PlayerVisual");
        playerVisual.transform.SetParent(playerObj.transform);
        playerVisual.transform.localPosition = Vector3.zero;
        
        // Add a simple visual (you can replace this with a sprite later)
        SpriteRenderer sr = playerVisual.AddComponent<SpriteRenderer>();
        
        // Create a simple colored square sprite
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.blue);
        texture.Apply();
        
        Sprite playerSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
        sr.sprite = playerSprite;
        
        // Connect visual to player controller
        PlayerController pc = playerObj.GetComponent<PlayerController>();
        pc.playerVisual = playerVisual;
        
        Debug.Log("Player created at origin with basic blue square visual.");
    }
    
    /// <summary>
    /// Create basic UI for currency and inventory display
    /// </summary>
    void CreateBasicUI()
    {
        // Check if Canvas already exists
        if (FindObjectOfType<Canvas>() != null)
        {
            Debug.Log("Canvas already exists, skipping UI creation.");
            return;
        }
        
        // Create Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        // Create EventSystem if it doesn't exist
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
        
        // Create UI Manager
        GameObject uiManagerObj = new GameObject("UIManager");
        uiManagerObj.transform.SetParent(canvasObj.transform);
        SimpleUIManager uiManager = uiManagerObj.AddComponent<SimpleUIManager>();
        
        // Create currency text
        GameObject currencyTextObj = CreateUIText("Currency Text", canvasObj.transform);
        RectTransform currencyRect = currencyTextObj.GetComponent<RectTransform>();
        currencyRect.anchorMin = new Vector2(1, 1);
        currencyRect.anchorMax = new Vector2(1, 1);
        currencyRect.anchoredPosition = new Vector2(-10, -10);
        currencyRect.sizeDelta = new Vector2(200, 30);
        
        // Create inventory text
        GameObject inventoryTextObj = CreateUIText("Inventory Text", canvasObj.transform);
        RectTransform inventoryRect = inventoryTextObj.GetComponent<RectTransform>();
        inventoryRect.anchorMin = new Vector2(0, 1);
        inventoryRect.anchorMax = new Vector2(0, 1);
        inventoryRect.anchoredPosition = new Vector2(10, -10);
        inventoryRect.sizeDelta = new Vector2(300, 150);
        
        // Create interaction prompt
        GameObject promptTextObj = CreateUIText("Interaction Prompt", canvasObj.transform);
        RectTransform promptRect = promptTextObj.GetComponent<RectTransform>();
        promptRect.anchorMin = new Vector2(0.5f, 0.5f);
        promptRect.anchorMax = new Vector2(0.5f, 0.5f);
        promptRect.anchoredPosition = new Vector2(0, 100);
        promptRect.sizeDelta = new Vector2(200, 30);
        promptTextObj.SetActive(false);
        
        // Connect to UI Manager
        uiManager.currencyText = currencyTextObj.GetComponent<TMPro.TextMeshProUGUI>();
        uiManager.inventoryText = inventoryTextObj.GetComponent<TMPro.TextMeshProUGUI>();
        uiManager.interactionPrompt = promptTextObj.GetComponent<TMPro.TextMeshProUGUI>();
        
        Debug.Log("Basic UI created with currency, inventory, and interaction displays.");
    }
    
    /// <summary>
    /// Helper method to create UI text objects
    /// </summary>
    GameObject CreateUIText(string name, Transform parent)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        TMPro.TextMeshProUGUI text = textObj.AddComponent<TMPro.TextMeshProUGUI>();
        text.text = name;
        text.fontSize = 16;
        text.color = Color.white;
        
        return textObj;
    }
    
    /// <summary>
    /// Create some example resource nodes for testing
    /// </summary>
    void CreateExampleResourceNodes()
    {
        for (int i = 0; i < resourceNodeCount; i++)
        {
            // Create random position around the origin
            Vector3 position = new Vector3(
                Random.Range(-8f, 8f),
                Random.Range(-4f, 4f),
                0
            );
            
            // Create resource node object
            GameObject nodeObj = new GameObject($"Resource Node {i + 1}");
            nodeObj.transform.position = position;
            
            // Add components
            nodeObj.AddComponent<ResourceNode>();
            CircleCollider2D collider = nodeObj.AddComponent<CircleCollider2D>();
            collider.isTrigger = true;
            collider.radius = 1.5f;
            
            // Create visual
            GameObject visual = new GameObject("Visual");
            visual.transform.SetParent(nodeObj.transform);
            visual.transform.localPosition = Vector3.zero;
            
            SpriteRenderer sr = visual.AddComponent<SpriteRenderer>();
            
            // Create different colored squares for different "resource types"
            Color[] colors = { Color.green, Color.gray, Color.yellow, Color.red, Color.magenta };
            Color nodeColor = colors[i % colors.Length];
            
            Texture2D texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, nodeColor);
            texture.Apply();
            
            Sprite nodeSprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 100);
            sr.sprite = nodeSprite;
            
            // Set layer for interaction
            nodeObj.layer = LayerMask.NameToLayer("Default");
        }
        
        Debug.Log($"Created {resourceNodeCount} example resource nodes. Note: You'll need to create ResourceItem assets and assign them to the nodes!");
    }
}