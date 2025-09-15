# My Place RPG - Unity Development Guide for Beginners

Welcome to your Unity RPG development journey! This guide will help you understand how to work with the project structure we've created and make progress on your "My Place" RPG game.

## 📋 Project Overview

Your RPG focuses on three core mechanics:
- **Gathering**: Collect resources from the world
- **Crafting**: Combine resources to create new items
- **Selling**: Trade items for currency to progress

## 🗂️ Project Structure Explained

```
Assets/
├── Scripts/
│   ├── Core/           # Basic game systems (ResourceNode, etc.)
│   ├── Items/          # Item definitions (Item.cs, ResourceItem.cs)
│   ├── Inventory/      # Inventory management system
│   ├── Crafting/       # Crafting recipes and logic
│   ├── Shop/           # Trading and selling systems
│   ├── Player/         # Player controller and movement
│   ├── Managers/       # Game-wide managers (GameManager, CurrencyManager)
│   ├── UI/             # User interface scripts (you'll add these)
│   └── Data/           # ScriptableObject asset files
├── Prefabs/            # Reusable game objects
├── Scenes/             # Your game levels/areas
├── Materials/          # Visual materials and shaders
├── Textures/           # Images and sprites
└── Audio/              # Sound effects and music
```

## 🚀 Step-by-Step Development Plan

### Phase 1: Set Up Your Scene (Week 1-2)
**Goal**: Create a basic playable scene with a player character

1. **Create Your First Scene**
   - Open Unity
   - Create a new scene: `File > New Scene`
   - Save it as "MainGame" in `Assets/Scenes/`

2. **Set Up the Player**
   - Create an empty GameObject, name it "Player"
   - Add the `PlayerController` script to it
   - Add a `Rigidbody2D` component (the script requires this)
   - Add a `CircleCollider2D` component for collision detection
   - Create a child GameObject called "PlayerVisual"
   - Add a `SpriteRenderer` to PlayerVisual (add any sprite for now)

3. **Set Up the Camera**
   - Select the Main Camera
   - Set Position to (0, 0, -10)
   - Add a simple camera follow script (you can find tutorials online)

4. **Test Movement**
   - Press Play and use WASD or arrow keys to move
   - The player should move around smoothly

### Phase 2: Create Resource Gathering (Week 3-4)
**Goal**: Add resource nodes that players can interact with

1. **Create Resource Items**
   - Right-click in `Assets/Scripts/Data/`
   - Choose `Create > My Place RPG > Items > Resource`
   - Create items like "Wood", "Stone", "Iron Ore"
   - Set their properties (names, descriptions, values)

2. **Create Resource Nodes**
   - Create empty GameObjects for trees, rocks, etc.
   - Add `ResourceNode` script to each
   - Add `CircleCollider2D` (make sure "Is Trigger" is checked)
   - Set the Layer to "Interactable" (create this layer if needed)
   - Assign your resource items to the nodes
   - Add simple sprites to represent them

3. **Set Up the Game Manager**
   - Create an empty GameObject called "GameManager"
   - Add the `GameManager` script
   - Add the `Inventory` script to the same object
   - Add the `CurrencyManager` script to the same object

4. **Test Gathering**
   - Play the game and walk near resource nodes
   - Press Space or E to gather resources
   - Check the Console for feedback messages

### Phase 3: Add Basic UI (Week 5-6)
**Goal**: Create simple UI to show inventory and currency

1. **Create UI Canvas**
   - Right-click in Hierarchy > UI > Canvas
   - This creates Canvas, EventSystem automatically

2. **Add Currency Display**
   - Create UI > Text (TextMeshPro) as child of Canvas
   - Position it in top-right corner
   - Create a script to update this text when currency changes

3. **Add Simple Inventory Display**
   - Create UI > Panel as child of Canvas
   - Add UI > Text children to show item counts
   - Create a script to update inventory display

4. **Add Interaction Prompts**
   - Create UI > Text for "Press E to interact"
   - Show/hide when near interactable objects

### Phase 4: Implement Crafting (Week 7-8)
**Goal**: Create recipes and crafting interface

1. **Create Crafting Recipes**
   - Right-click in `Assets/Scripts/Data/`
   - Choose `Create > My Place RPG > Crafting > Recipe`
   - Create simple recipes like:
     - 2 Wood → 1 Wooden Plank
     - 3 Stone → 1 Stone Block
     - 1 Wood + 1 Stone → 1 Basic Tool

2. **Set Up Crafting Manager**
   - Add `CraftingManager` script to your GameManager object
   - Assign your recipes to the allRecipes list

3. **Create Crafting UI**
   - Create a crafting panel (hidden by default)
   - Add buttons for each known recipe
   - Show requirements and results
   - Add craft buttons

4. **Test Crafting**
   - Gather materials
   - Open crafting menu (you'll need to add a key for this)
   - Try crafting different items

### Phase 5: Add Shop System (Week 9-10)
**Goal**: Create NPCs or locations where players can buy/sell

1. **Create Shop Objects**
   - Create empty GameObjects for shops
   - Add `Shop` script to each
   - Add `CircleCollider2D` with "Is Trigger" checked
   - Set up shop inventories with items to sell

2. **Create Shop UI**
   - Create buy/sell interface panels
   - Show shop items with prices
   - Show player inventory for selling
   - Add buy/sell buttons

3. **Test Trading**
   - Walk up to shops and interact
   - Buy items with currency
   - Sell gathered/crafted items

### Phase 6: Polish and Expand (Week 11+)
**Goal**: Improve the game feel and add content

1. **Add Visual Polish**
   - Create proper sprites for all items
   - Add particle effects for gathering
   - Add sound effects for actions
   - Improve UI design

2. **Add More Content**
   - More resource types
   - More complex recipes
   - Different tool requirements
   - Multiple areas/maps

3. **Add Game Progression**
   - Player levels
   - Skill trees
   - Rare resources
   - Special crafting stations

## 🛠️ Key Unity Concepts You'll Learn

### Scripts and Components
- **MonoBehaviour**: Base class for Unity scripts
- **Components**: Scripts attached to GameObjects
- **Transform**: Position, rotation, scale of objects
- **Rigidbody2D**: Physics for 2D movement
- **Collider2D**: For collision detection

### Events and Communication
- **Events**: How scripts communicate (like notifications)
- **Singleton Pattern**: GameManager.Instance access
- **Interfaces**: IInteractable for common behavior

### Data Management
- **ScriptableObjects**: Data assets (items, recipes)
- **Lists and Dictionaries**: Storing collections of data
- **Serialization**: Saving/loading game data

### UI System
- **Canvas**: Container for all UI elements
- **Text/TextMeshPro**: Displaying text
- **Buttons**: Interactive UI elements
- **Panels**: Organizing UI groups

## 🐛 Common Beginner Mistakes and Solutions

### "NullReferenceException"
**Problem**: Trying to use something that doesn't exist
**Solution**: 
- Check if references are assigned in Inspector
- Use null checks: `if (someObject != null)`

### "Object not found"
**Problem**: Script can't find another component/object
**Solution**:
- Assign references in Inspector
- Use FindObjectOfType<>() carefully
- Check object names and tags

### "Method not found" or "Doesn't exist in context"
**Problem**: Typos or missing using statements
**Solution**:
- Check spelling carefully
- Add: `using UnityEngine;` at top of scripts
- Check Unity documentation for correct method names

### UI not responding
**Problem**: Missing EventSystem or incorrect Canvas settings
**Solution**:
- Make sure EventSystem exists in scene
- Check Canvas Render Mode is "Screen Space - Overlay"

## 📚 Next Steps for Learning

1. **Unity Learn Platform**: Free Unity tutorials
2. **Brackeys YouTube**: Excellent beginner tutorials
3. **Unity Documentation**: Official reference
4. **C# Fundamentals**: Learn programming basics

## 🎯 Tips for Success

1. **Start Small**: Get one system working before adding the next
2. **Test Frequently**: Play your game after every change
3. **Read Error Messages**: The Console gives helpful information
4. **Ask for Help**: Unity community is very helpful
5. **Save Often**: Ctrl+S saves your scene and scripts
6. **Version Control**: Learn Git for backing up your work

## 🔧 Customization Ideas

Once you understand the basics, try these modifications:

- **Different Resource Types**: Rare gems, magical crystals
- **Weather Effects**: Rain affects gathering speed
- **Day/Night Cycle**: Some resources only available at certain times
- **NPCs with Quests**: "Bring me 10 wood for a reward"
- **Multiple Areas**: Forest, mines, mountains with different resources
- **Player Housing**: Build and customize a home base
- **Multiplayer**: Let friends join your world

Remember: Game development is a journey, not a destination. Every expert was once a beginner, so be patient with yourself and enjoy the learning process!

Happy coding! 🎮