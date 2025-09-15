# My Place RPG - Unity Game Project

Welcome to "My Place" - a beginner-friendly RPG focused on gathering, crafting, and selling!

## 🎮 Game Overview

**My Place** is a 2D top-down RPG where players:
- **Gather** resources from the world (wood, stone, ores)
- **Craft** items by combining materials with recipes
- **Sell** items to merchants for currency
- **Progress** by unlocking new recipes and areas

## 🏗️ Project Structure

This project uses an industry-standard Unity architecture designed for beginners:

### Core Systems
- **Item System**: ScriptableObject-based items with inheritance (Item → ResourceItem)
- **Inventory Management**: Event-driven inventory with stacking and capacity limits
- **Crafting System**: Recipe-based crafting with success rates and progression
- **Shop System**: Buy/sell mechanics with different shop types
- **Currency Management**: Centralized money system with events
- **Player Controller**: 2D movement with interaction system

### Architecture Patterns
- **Manager Pattern**: GameManager coordinates all systems
- **Singleton Pattern**: Global access to key managers
- **Event System**: Loose coupling between systems
- **Interface Design**: IInteractable for consistent object interaction
- **Component-Based**: Modular systems that can be easily extended

## 📁 File Organization

```
Assets/
├── Scripts/
│   ├── Core/           # ResourceNode and base game objects
│   ├── Items/          # Item definitions and types
│   ├── Inventory/      # Inventory management system
│   ├── Crafting/       # Recipe system and crafting logic
│   ├── Shop/           # Trading and merchant systems
│   ├── Player/         # Player movement and interaction
│   ├── Managers/       # Game-wide managers (Game, Currency)
│   ├── UI/             # User interface components
│   └── Data/           # ScriptableObject assets (items, recipes)
├── Prefabs/            # Reusable GameObjects
├── Scenes/             # Game levels and areas
├── Materials/          # Visual materials and shaders
├── Textures/           # Sprites and images
└── Audio/              # Sound effects and music
```

## 🚀 Getting Started

### For Beginners
1. **Read the Development Guide**: See `DEVELOPMENT_GUIDE.md` for step-by-step instructions
2. **Start with Phase 1**: Set up a basic scene with player movement
3. **Follow the weekly plan**: Each phase builds on the previous one
4. **Test frequently**: Play your game after each change

### For Experienced Developers
1. **Review the architecture**: Check out the Manager and Event systems
2. **Examine ScriptableObjects**: See how items and recipes are structured
3. **Understand the interaction system**: IInteractable interface design
4. **Explore the UI integration**: Event-driven UI updates

## 🛠️ Key Features

### Inventory System
- **Dynamic stacking**: Items stack up to their maximum stack size
- **Capacity management**: Limited slots with visual feedback
- **Event notifications**: UI updates automatically when inventory changes
- **Type safety**: Strongly-typed item system prevents errors

### Crafting System
- **Recipe discovery**: Players learn recipes through gameplay
- **Success rates**: Crafting can fail based on skill level
- **Progression system**: Crafting levels unlock new recipes
- **Flexible requirements**: Recipes can require tools, stations, or skill levels

### Shop System
- **Multiple shop types**: Different merchants specialize in different items
- **Dynamic pricing**: Buy/sell prices can be adjusted per shop
- **Stock management**: Shops have limited inventory that can restock
- **Category filtering**: Shops only accept certain item types

## 📖 Learning Resources

### Included Documentation
- **DEVELOPMENT_GUIDE.md**: Complete beginner's roadmap
- **Inline code comments**: Every script heavily commented for learning
- **Architecture explanations**: Comments explain design decisions

### Recommended Learning
- Unity Learn Platform (free tutorials)
- C# programming fundamentals
- Game design patterns
- Unity's official documentation

## 🎯 Beginner-Friendly Features

### Extensive Comments
Every script includes:
- **Class-level explanations**: What the script does and why
- **Method documentation**: What each function accomplishes
- **Inline comments**: Complex logic explained step-by-step
- **Unity-specific notes**: Explains Unity concepts like MonoBehaviour, Events, etc.

### Error Prevention
- **Null checks**: Prevents common NullReferenceExceptions
- **Defensive programming**: Validates inputs and handles edge cases
- **Debug logging**: Helpful console messages for testing
- **Inspector tooltips**: Every public field explains its purpose

### Modular Design
- **Easy to extend**: Add new item types, shops, or crafting stations
- **Loosely coupled**: Systems work independently
- **Event-driven**: Clean separation between logic and UI
- **Component-based**: Mix and match functionality

## 🔧 Customization Examples

Want to expand the game? Try these modifications:

### New Item Types
```csharp
[CreateAssetMenu(fileName = "New Tool", menuName = "My Place RPG/Items/Tool")]
public class ToolItem : Item
{
    public ToolType toolType;
    public int toolLevel;
    // Add tool-specific functionality
}
```

### Custom Crafting Stations
```csharp
public class CraftingStation : MonoBehaviour, IInteractable
{
    public CraftingStationType stationType;
    // Implement IInteractable methods
}
```

### New Shop Types
```csharp
public enum ShopType
{
    General, Blacksmith, Alchemist, YourNewShopType
}
```

## 🐛 Common Issues and Solutions

### Scene Setup
- Make sure GameManager has Inventory and CurrencyManager components
- Player needs Rigidbody2D and Collider2D components
- Resource nodes need Colliders with "Is Trigger" checked

### Script References
- Assign ScriptableObject items in the Inspector
- Check that UI components are connected properly
- Verify layer masks for interactions

### Performance
- Inventory UI updates are throttled (configurable rate)
- Resource nodes use efficient collision detection
- Events prevent unnecessary Update() calls

## 📝 Development Status

### ✅ Implemented Core Systems
- [x] Item system with inheritance
- [x] Inventory management with events
- [x] Resource gathering mechanics
- [x] Crafting system with recipes
- [x] Shop/trading system
- [x] Currency management
- [x] Player movement and interaction
- [x] Basic UI framework

### 🚧 Ready for Implementation
- [ ] Scene setup and prefab creation
- [ ] UI visual design and layout
- [ ] Art assets (sprites, animations)
- [ ] Audio implementation
- [ ] Save/load system
- [ ] Multiple game areas
- [ ] Advanced gameplay features

## 🤝 Contributing

This project is designed as a learning resource. Feel free to:
- Fork and experiment with modifications
- Submit issues if you find bugs or unclear documentation
- Share improvements or additional learning resources
- Ask questions about Unity or C# concepts

## 📄 License

This project is created for educational purposes. Use it to learn, modify it for your own projects, and share your knowledge with other beginners!

---

**Happy game development!** 🎮✨

Remember: Every expert was once a beginner. Take your time, experiment, and don't be afraid to make mistakes - that's how you learn!
