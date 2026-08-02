using CommandLine;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Slots;
using CryBits.Host.Core;
using CryBits.Persistence.Repositories;
using Microsoft.Extensions.Logging;
using ZLogger;
using Attribute = CryBits.Definitions.Characters.Attribute;
using NpcDef = CryBits.Definitions.Npcs.Npc;
using NpcDropDef = CryBits.Definitions.Npcs.NpcDrop;
using ShopDef = CryBits.Definitions.Shops.Shop;
using ShopItemDef = CryBits.Definitions.Shops.ShopItem;

namespace CryBits.Server.Commands;

[Verb("seed",
    HelpText =
        "Seeds the server with starter items, NPCs, shops and a map. Skips if data already exists (use -f to overwrite).")]
internal sealed class SeedCommand(
    DefinitionCatalog catalog,
    WorldInitializer worldInitializer,
    ILogger<SeedCommand> logger) : IConsoleCommand
{
    public SeedCommand() : this(ServerContext.Catalog!, new WorldInitializer(ServerContext.Host!),
        ServerContext.LoggerFactory!.CreateLogger<SeedCommand>())
    {
    }

    [Option('f', "force", HelpText = "Overwrite existing data even if it is already present.")]
    public bool Force { get; set; }

    public void Execute()
    {
        if (!Force && (catalog.Items.Count > 0 || catalog.Npcs.Count > 0 || catalog.Shops.Count > 0 ||
                       catalog.Maps.Count > 0 ||
                       catalog.Classes.Count > 0))
        {
            logger.ZLogInformation($"Seed skipped: data already exists (use --force to overwrite)");
            return;
        }

        logger.ZLogInformation($"Seeding data");

        catalog.Items.Clear();
        catalog.Npcs.Clear();
        catalog.Shops.Clear();
        catalog.Maps.Clear();
        catalog.Classes.Clear();

        var gold = new Item
        {
            Name = "Gold",
            Description = "Common currency used throughout the world.",
            Texture = 6,
            Stackable = true,
            Rarity = Rarity.Common
        };

        var sword = new Item
        {
            Name = "Iron Sword",
            Description = "A sturdy sword forged from iron.",
            Texture = 2,
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Weapon,
            WeaponDamage = 15,
            Rarity = Rarity.Common
        };

        var armor = new Item
        {
            Name = "Leather Armor",
            Description = "Simple but reliable leather armor.",
            Texture = 3,
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Armor,
            Rarity = Rarity.Common,
            EquipAttribute = { [(byte)Attribute.Resistance] = 10 }
        };

        var helmet = new Item
        {
            Name = "Iron Helmet",
            Description = "A sturdy iron helmet.",
            Texture = 4,
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Helmet,
            Rarity = Rarity.Common,
            EquipAttribute = { [(byte)Attribute.Resistance] = 5 }
        };

        var shield = new Item
        {
            Name = "Wooden Shield",
            Description = "A basic wooden shield.",
            Texture = 5,
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Shield,
            Rarity = Rarity.Common,
            EquipAttribute = { [(byte)Attribute.Resistance] = 8 }
        };

        var amulet = new Item
        {
            Name = "Stone Amulet",
            Description = "Imbued with a faint magical energy.",
            Texture = 6,
            Type = ItemType.Equipment,
            EquipType = (byte)Equipment.Amulet,
            Rarity = Rarity.Uncommon,
            EquipAttribute = { [(byte)Attribute.Intelligence] = 8 }
        };

        var healthPotion = new Item
        {
            Name = "Health Potion",
            Description = "Restores a moderate amount of HP.",
            Texture = 7,
            Stackable = true,
            Rarity = Rarity.Common,
            PotionVital = { [(byte)Vital.Hp] = 50 }
        };

        var manaPotion = new Item
        {
            Name = "Mana Potion",
            Description = "Restores a moderate amount of MP.",
            Texture = 1,
            Stackable = true,
            Rarity = Rarity.Common,
            PotionVital = { [(byte)Vital.Mp] = 30 }
        };

        foreach (var item in new[] { gold, sword, armor, helmet, shield, amulet, healthPotion, manaPotion })
            catalog.Items[item.Id] = item;

        logger.ZLogDebug($"Seeded {catalog.Items.Count} items");

        var generalStore = new ShopDef { Name = "General Store", CurrencyId = gold.Id };
        generalStore.Sold.Add(new ShopItemDef(healthPotion.Id, 1, 10));
        generalStore.Sold.Add(new ShopItemDef(manaPotion.Id, 1, 8));
        generalStore.Sold.Add(new ShopItemDef(helmet.Id, 1, 30));
        generalStore.Sold.Add(new ShopItemDef(shield.Id, 1, 25));
        generalStore.Sold.Add(new ShopItemDef(amulet.Id, 1, 50));
        generalStore.Bought.Add(new ShopItemDef(sword.Id, 1, 5));
        catalog.Shops[generalStore.Id] = generalStore;

        logger.ZLogDebug($"Seeded {catalog.Shops.Count} shops");

        var merchant = new NpcDef
        {
            Name = "Old Merchant",
            SayMsg = "Welcome, traveller! Browse my wares.",
            Texture = 1,
            Behaviour = Behaviour.Friendly,
            Movement = MovementStyle.TurnRandomly,
            SpawnTime = 10,
            Sight = 5,
            ShopId = generalStore.Id,
            Vital = { [(byte)Vital.Hp] = 100 }
        };

        var goblin = new NpcDef
        {
            Name = "Wild Wolf",
            SayMsg = string.Empty,
            Texture = 5,
            Behaviour = Behaviour.AttackOnSight,
            Movement = MovementStyle.MoveRandomly,
            SpawnTime = 15,
            Sight = 8,
            Experience = 25,
            Vital = { [(byte)Vital.Hp] = 60 },
            Attribute = { [(byte)Attribute.Strength] = 5 }
        };
        goblin.Drop.Add(new NpcDropDef(gold.Id, 5, 80));
        goblin.Drop.Add(new NpcDropDef(healthPotion.Id, 1, 25));

        var snake = new NpcDef
        {
            Name = "Venomous Snake",
            SayMsg = string.Empty,
            Texture = 6,
            Behaviour = Behaviour.AttackWhenAttacked,
            Movement = MovementStyle.MoveRandomly,
            SpawnTime = 20,
            Sight = 6,
            Experience = 15,
            Vital = { [(byte)Vital.Hp] = 40 },
            Attribute = { [(byte)Attribute.Agility] = 8 }
        };
        snake.Drop.Add(new NpcDropDef(gold.Id, 2, 60));

        foreach (var npc in new[] { merchant, goblin, snake })
            catalog.Npcs[npc.Id] = npc;

        logger.ZLogDebug($"Seeded {catalog.Npcs.Count} NPCs");

        var map = new Map
        {
            Name = "Starting Village",
            Moral = Moral.Dangerous,
            DefaultWeather = WeatherType.Rain,
            DefaultFog = new FogConfig(1, 10, 5, 100)
        };

        // Create 5 chunks forming a cross: middle (0,0), top, left, right, bottom
        // Each filled with texture 4, row 0, different columns 0-4
        (short cx, short cy, byte col)[] chunks = [(0, 0, 0), (0, -1, 1), (-1, 0, 2), (1, 0, 3), (0, 1, 4)];
        foreach (var (cx, cy, col) in chunks)
        {
            var tiles = new TileData[32, 32];
            for (var x = 0; x < 32; x++)
                for (var y = 0; y < 32; y++)
                    tiles[x, y] = new TileData(Texture: 4, SourceX: col, SourceY: 0, IsAutoTile: false,
                        Attribute: new NoAttribute());
            map.Chunks[new ChunkCoord(cx, cy)] = new MapChunk(cx, cy, 1, tiles);
        }

        map.Npc.Add(new MapNpc { NpcId = merchant.Id, Spawn = true, X = 5, Y = 9 });
        map.Npc.Add(new MapNpc { NpcId = goblin.Id, Spawn = true, X = 20, Y = 15 });
        map.Npc.Add(new MapNpc { NpcId = snake.Id, Spawn = true, X = 18, Y = 12 });
        catalog.Maps[map.Id] = map;

        logger.ZLogDebug($"Seeded {catalog.Maps.Count} maps");

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
        warrior.TextureMale.Add(1);
        warrior.TextureFemale.Clear();
        warrior.TextureFemale.Add(2);

        warrior.Vital[(byte)Vital.Hp] = 200;
        warrior.Vital[(byte)Vital.Mp] = 50;
        warrior.Attribute[(byte)Attribute.Strength] = 15;
        warrior.Attribute[(byte)Attribute.Resistance] = 10;
        warrior.Attribute[(byte)Attribute.Vitality] = 12;
        warrior.Attribute[(byte)Attribute.Agility] = 6;
        warrior.Attribute[(byte)Attribute.Intelligence] = 3;

        warrior.Item.Add(new ItemSlot(sword.Id, 1));
        warrior.Item.Add(new ItemSlot(armor.Id, 1));
        warrior.Item.Add(new ItemSlot(helmet.Id, 1));
        warrior.Item.Add(new ItemSlot(healthPotion.Id, 3));

        catalog.Classes[warrior.Id] = warrior;
        logger.ZLogDebug($"Seeded class {warrior.Name}");

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
        mage.TextureMale.Add(3);
        mage.TextureFemale.Clear();
        mage.TextureFemale.Add(4);

        mage.Vital[(byte)Vital.Hp] = 100;
        mage.Vital[(byte)Vital.Mp] = 200;
        mage.Attribute[(byte)Attribute.Intelligence] = 18;
        mage.Attribute[(byte)Attribute.Agility] = 8;
        mage.Attribute[(byte)Attribute.Resistance] = 5;
        mage.Attribute[(byte)Attribute.Vitality] = 4;
        mage.Attribute[(byte)Attribute.Strength] = 2;

        mage.Item.Add(new ItemSlot(amulet.Id, 1));
        mage.Item.Add(new ItemSlot(manaPotion.Id, 3));
        mage.Item.Add(new ItemSlot(healthPotion.Id, 1));

        catalog.Classes[mage.Id] = mage;
        logger.ZLogDebug($"Seeded class {mage.Name}");

        var store = new ContentRepository();
        var mapRepo = new MapRepository();
        store.SaveAll(catalog.Items.Values);
        store.SaveAll(catalog.Npcs.Values);
        store.SaveAll(catalog.Shops.Values);
        mapRepo.SaveAllMaps(catalog.Maps.Values);
        store.SaveAll(catalog.Classes.Values);

        worldInitializer.Initialize();

        logger.ZLogInformation($"Seed complete");
    }
}
