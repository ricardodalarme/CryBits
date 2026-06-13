using CommandLine;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Slots;
using CryBits.Persistence.Stores;
using CryBits.Server.Core;
using CryBits.Server.Entities;
using System;
using System.IO;
using Attribute = CryBits.Definitions.Characters.Attribute;
using NpcDef = CryBits.Definitions.Npcs.Npc;
using NpcDropDef = CryBits.Definitions.Npcs.NpcDrop;
using ShopDef = CryBits.Definitions.Shops.Shop;
using ShopItemDef = CryBits.Definitions.Shops.ShopItem;

namespace CryBits.Server.Commands;

[Verb("seed",
    HelpText =
        "Seeds the server with starter items, NPCs, shops and a map. Skips if data already exists (use -f to overwrite).")]
internal sealed class SeedCommand : IConsoleCommand
{
    private readonly DefinitionCatalog _catalog;

    public SeedCommand() : this(DefinitionCatalog.Instance) { }
    public SeedCommand(DefinitionCatalog catalog) => _catalog = catalog;

    [Option('f', "force", HelpText = "Overwrite existing data even if it is already present.")]
    public bool Force { get; set; }

    public void Execute()
    {
        // Guard: abort if data already exists and force flag not set.
        if (!Force && (_catalog.Items.Count > 0 || _catalog.Npcs.Count > 0 || _catalog.Shops.Count > 0 || _catalog.Maps.Count > 0 ||
                       _catalog.Classes.Count > 0))
        {
            Console.WriteLine("[Seed] Data already exists. Run with -f / --force to overwrite.");
            return;
        }

        Console.WriteLine("[Seed] Seeding data...");

        // Clear any in-memory data so we start fresh.
        _catalog.Items.Clear();
        _catalog.Npcs.Clear();
        _catalog.Shops.Clear();
        _catalog.Maps.Clear();
        _catalog.Classes.Clear();

        // ── Items ────────────────────────────────────────────────────────────

        var gold = new Item
        {
            Name = "Gold",
            Description = "Common currency used throughout the world.",
            Texture = 6, // Amulet — shiny valuable
            Stackable = true,
            Rarity = Rarity.Common
        };

        var sword = new Item
        {
            Name = "Iron Sword",
            Description = "A sturdy sword forged from iron.",
            Texture = 2, // Sword
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Weapon,
            WeaponDamage = 15,
            Rarity = Rarity.Common
        };

        var armor = new Item
        {
            Name = "Leather Armor",
            Description = "Simple but reliable leather armor.",
            Texture = 3, // Armor
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Armor,
            Rarity = Rarity.Common,
            EquipAttribute =
            {
                [(byte)Attribute.Resistance] = 10
            }
        };

        var helmet = new Item
        {
            Name = "Iron Helmet",
            Description = "A sturdy iron helmet.",
            Texture = 4, // Helmet
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Helmet,
            Rarity = Rarity.Common,
            EquipAttribute =
            {
                [(byte)Attribute.Resistance] = 5
            }
        };

        var shield = new Item
        {
            Name = "Wooden Shield",
            Description = "A basic wooden shield.",
            Texture = 5, // Shield
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Shield,
            Rarity = Rarity.Common,
            EquipAttribute =
            {
                [(byte)Attribute.Resistance] = 8
            }
        };

        var amulet = new Item
        {
            Name = "Stone Amulet",
            Description = "Imbued with a faint magical energy.",
            Texture = 6, // Amulet
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Amulet,
            Rarity = Rarity.Uncommon,
            EquipAttribute =
            {
                [(byte)Attribute.Intelligence] = 8
            }
        };

        var healthPotion = new Item
        {
            Name = "Health Potion",
            Description = "Restores a moderate amount of HP.",
            Texture = 7, // Bread
            Stackable = true,
            Rarity = Rarity.Common,
            PotionVital =
            {
                [(byte)Vital.Hp] = 50
            }
        };

        var manaPotion = new Item
        {
            Name = "Mana Potion",
            Description = "Restores a moderate amount of MP.",
            Texture = 1, // Leather
            Stackable = true,
            Rarity = Rarity.Common,
            PotionVital =
            {
                [(byte)Vital.Mp] = 30
            }
        };

        foreach (var item in new[] { gold, sword, armor, helmet, shield, amulet, healthPotion, manaPotion })
            _catalog.Items[item.Id] = item;

        Console.WriteLine($"[Seed] Created {_catalog.Items.Count} items.");

        // ── Shop ─────────────────────────────────────────────────────────────

        var generalStore = new ShopDef { Name = "General Store", CurrencyId = gold.Id };
        generalStore.Sold.Add(new ShopItemDef(healthPotion.Id, 1, 10));
        generalStore.Sold.Add(new ShopItemDef(manaPotion.Id, 1, 8));
        generalStore.Sold.Add(new ShopItemDef(helmet.Id, 1, 30));
        generalStore.Sold.Add(new ShopItemDef(shield.Id, 1, 25));
        generalStore.Sold.Add(new ShopItemDef(amulet.Id, 1, 50));
        generalStore.Bought.Add(new ShopItemDef(sword.Id, 1, 5));
        _catalog.Shops[generalStore.Id] = generalStore;

        Console.WriteLine($"[Seed] Created {_catalog.Shops.Count} shops.");

        // ── NPCs ─────────────────────────────────────────────────────────────

        var merchant = new NpcDef
        {
            Name = "Old Merchant",
            SayMsg = "Welcome, traveller! Browse my wares.",
            Texture = 1, // Male Warrior
            Behaviour = Behaviour.Friendly,
            Movement = MovementStyle.TurnRandomly,
            SpawnTime = 10,
            Sight = 5,
            ShopId = generalStore.Id,
            Vital =
            {
                [(byte)Vital.Hp] = 100
            }
        };

        var goblin = new NpcDef
        {
            Name = "Wild Wolf",
            SayMsg = string.Empty,
            Texture = 5, // Wolf
            Behaviour = Behaviour.AttackOnSight,
            Movement = MovementStyle.MoveRandomly,
            SpawnTime = 15,
            Sight = 8,
            Experience = 25,
            Vital =
            {
                [(byte)Vital.Hp] = 60
            },
            Attribute =
            {
                [(byte)Attribute.Strength] = 5
            }
        };
        goblin.Drop.Add(new NpcDropDef(gold.Id, 5, 80));
        goblin.Drop.Add(new NpcDropDef(healthPotion.Id, 1, 25));

        var snake = new NpcDef
        {
            Name = "Venomous Snake",
            SayMsg = string.Empty,
            Texture = 6, // Snake
            Behaviour = Behaviour.AttackWhenAttacked,
            Movement = MovementStyle.MoveRandomly,
            SpawnTime = 20,
            Sight = 6,
            Experience = 15,
            Vital =
            {
                [(byte)Vital.Hp] = 40
            },
            Attribute =
            {
                [(byte)Attribute.Agility] = 8
            }
        };
        snake.Drop.Add(new NpcDropDef(gold.Id, 2, 60));

        foreach (var npc in new[] { merchant, goblin, snake })
            _catalog.Npcs[npc.Id] = npc;

        Console.WriteLine($"[Seed] Created {_catalog.Npcs.Count} NPCs.");

        // ── Map ──────────────────────────────────────────────────────────────

        var map = new Map
        {
            Name = "Starting Village",
            Moral = Moral.Pacific,
            Weather = new MapWeather { Type = Weather.Raining, Intensity = 2 },
            Fog = new MapFog
            {
                Texture = 1,
                SpeedX = 10,
                SpeedY = 5,
                Alpha = 100
            }
        };

        // Fill the entire ground layer with grass (tileset 1, grid 0,0).
        var groundLayer = map.Layer[0];
        for (byte x = 0; x < Map.Width; x++)
            for (byte y = 0; y < Map.Height; y++)
            {
                groundLayer.Tile[x, y].Texture = 1;
                groundLayer.Tile[x, y].X = 0;
                groundLayer.Tile[x, y].Y = 0;
            }

        map.Npc.Add(new MapNpc { NpcId = merchant.Id, Spawn = true, X = 12, Y = 9 });
        map.Npc.Add(new MapNpc { NpcId = goblin.Id, Spawn = true, X = 20, Y = 15 });
        map.Npc.Add(new MapNpc { NpcId = snake.Id, Spawn = true, X = 18, Y = 12 });
        _catalog.Maps[map.Id] = map;

        Console.WriteLine($"[Seed] Created {_catalog.Maps.Count} maps.");

        // ── Warrior Class (powerful starter class) ───────────────────────────

        var warrior = new Class
        {
            Name = "Warrior",
            Description = "A battle-hardened warrior with immense strength and vitality.",
            SpawnMapId = map.Id,
            SpawnX = 12,
            SpawnY = 9,
            SpawnDirection = 1
        };
        warrior.TextureMale.Clear();
        warrior.TextureMale.Add(1); // Male Warrior
        warrior.TextureFemale.Clear();
        warrior.TextureFemale.Add(2); // Female Warrior

        // Stats — favouring strength and vitality.
        warrior.Vital[(byte)Vital.Hp] = 200;
        warrior.Vital[(byte)Vital.Mp] = 50;
        warrior.Attribute[(byte)Attribute.Strength] = 15;
        warrior.Attribute[(byte)Attribute.Resistance] = 10;
        warrior.Attribute[(byte)Attribute.Vitality] = 12;
        warrior.Attribute[(byte)Attribute.Agility] = 6;
        warrior.Attribute[(byte)Attribute.Intelligence] = 3;

        // Starting equipment.
        warrior.Item.Add(new ItemSlot(sword.Id, 1));
        warrior.Item.Add(new ItemSlot(armor.Id, 1));
        warrior.Item.Add(new ItemSlot(helmet.Id, 1));
        warrior.Item.Add(new ItemSlot(healthPotion.Id, 3));

        _catalog.Classes[warrior.Id] = warrior;
        Console.WriteLine($"[Seed] Created class '{warrior.Name}'.");

        // ── Mage Class ───────────────────────────────────────────────────────

        var mage = new Class
        {
            Name = "Mage",
            Description = "A scholarly mage whose mastery of the arcane makes them devastatingly powerful.",
            SpawnMapId = map.Id,
            SpawnX = 12,
            SpawnY = 9,
            SpawnDirection = 1
        };
        mage.TextureMale.Clear();
        mage.TextureMale.Add(3); // Male Mage
        mage.TextureFemale.Clear();
        mage.TextureFemale.Add(4); // Female Mage

        // Stats — favouring intelligence and MP.
        mage.Vital[(byte)Vital.Hp] = 100;
        mage.Vital[(byte)Vital.Mp] = 200;
        mage.Attribute[(byte)Attribute.Intelligence] = 18;
        mage.Attribute[(byte)Attribute.Agility] = 8;
        mage.Attribute[(byte)Attribute.Resistance] = 5;
        mage.Attribute[(byte)Attribute.Vitality] = 4;
        mage.Attribute[(byte)Attribute.Strength] = 2;

        // Starting equipment.
        mage.Item.Add(new ItemSlot(amulet.Id, 1));
        mage.Item.Add(new ItemSlot(manaPotion.Id, 3));
        mage.Item.Add(new ItemSlot(healthPotion.Id, 1));

        _catalog.Classes[mage.Id] = mage;
        Console.WriteLine($"[Seed] Created class '{mage.Name}'.");

        // ── Persist ──────────────────────────────────────────────────────────

        var store = new FileContentStore(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "Data")));
        store.SaveAll(_catalog.Items.Values);
        store.SaveAll(_catalog.Npcs.Values);
        store.SaveAll(_catalog.Shops.Values);
        store.SaveAll(_catalog.Maps.Values);
        store.SaveAll(_catalog.Classes.Values);

        // Rebuild live GameWorld.Maps so the running server uses the new map IDs.
        GameWorld.Current.Maps.Clear();
        foreach (var mapDef in _catalog.Maps.Values)
            MapInstance.Create(mapDef, true, GameWorld.Current.Entities);

        Console.WriteLine("[Seed] All data written to disk. Done.");
    }
}
