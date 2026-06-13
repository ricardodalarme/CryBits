using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Common;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Formulas;
using CryBits.Simulation.State;
using System;
using static CryBits.Definitions.Globals;

namespace CryBits.Simulation.Spawners;

public static class PlayerSpawner
{
    public static EntityId Spawn(World world, DefinitionCatalog catalog, Character data)
    {
        var @class = catalog.Classes.Get(data.ClassId);
        ArgumentNullException.ThrowIfNull(@class);

        var maxHp = VitalFormulas.MaxVital(Vital.Hp, @class.Vital[(byte)Vital.Hp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], data.Level);
        var maxMp = VitalFormulas.MaxVital(Vital.Mp, @class.Vital[(byte)Vital.Mp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], data.Level);

        var entityId = world.Entities.Create();
        var entity = world.Entities.Get(entityId)!;

        entity.Set(new Position
        {
            MapId = data.MapId,
            X = data.X,
            Y = data.Y,
            Direction = (Direction)data.Direction
        });

        entity.Set(new PlayerAppearance
        {
            Name = data.Name,
            ClassId = data.ClassId,
            TextureNum = data.Gender == Gender.Male ? @class.TextureMale[data.TextureNum] : @class.TextureFemale[data.TextureNum],
            Gender = data.Gender
        });

        var stats = new StatBlock
        {
            Level = data.Level,
            Experience = data.Experience,
            Points = data.Points,
            Attribute = (short[])data.Attributes.Clone()
        };
        entity.Set(stats);

        var hp = data.Hp > 0 ? data.Hp : maxHp;
        var mp = data.Mp > 0 ? data.Mp : maxMp;
        entity.Set(new Vitals { Hp = hp, Mp = mp, MaxHp = maxHp, MaxMp = maxMp });

        var inv = new InventoryState();
        for (byte i = 0; i < MaxInventory; i++)
            inv.Slots[i] = i < data.InventoryIds.Length
                ? new ItemSlot(data.InventoryIds[i], data.InventoryAmounts[i])
                : new ItemSlot(Guid.Empty, 0);
        entity.Set(inv);

        var equip = new EquipmentState();
        for (byte i = 0; i < (byte)Equipment.Count; i++)
            equip.Slots[i] = i < data.Equipment.Length ? data.Equipment[i] : Guid.Empty;
        entity.Set(equip);

        var hotbar = new HotbarState();
        for (byte i = 0; i < MaxHotbar; i++)
            hotbar.Slots[i] = i < data.HotbarTypes.Length
                ? new HotbarSlot((SlotType)data.HotbarTypes[i], data.HotbarSlots[i])
                : new HotbarSlot(SlotType.None, 0);
        entity.Set(hotbar);

        entity.Set(new AttackCooldown());
        entity.Set(new TradeState());
        entity.Set(new PartyState());
        entity.Set(new ShopState());
        entity.Set(new PlayerTag());

        return entityId;
    }

    public static EntityId Spawn(World world, DefinitionCatalog catalog, string name,
        Class @class, Gender gender, short textureNum)
    {
        var entityId = world.Entities.Create();
        var entity = world.Entities.Get(entityId)!;

        var maxHp = VitalFormulas.MaxVital(Vital.Hp, @class.Vital[(byte)Vital.Hp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);
        var maxMp = VitalFormulas.MaxVital(Vital.Mp, @class.Vital[(byte)Vital.Mp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);

        entity.Set(new Position
        {
            MapId = @class.SpawnMapId,
            X = @class.SpawnX,
            Y = @class.SpawnY,
            Direction = (Direction)@class.SpawnDirection
        });

        entity.Set(new PlayerAppearance
        {
            Name = name,
            ClassId = @class.Id,
            TextureNum = gender == Gender.Male ? @class.TextureMale[textureNum] : @class.TextureFemale[textureNum],
            Gender = gender
        });

        entity.Set(new StatBlock { Level = 1, Attribute = (short[])@class.Attribute.Clone() });
        entity.Set(new Vitals { Hp = maxHp, Mp = maxMp, MaxHp = maxHp, MaxMp = maxMp });

        var inv = new InventoryState();
        for (byte i = 0; i < MaxInventory; i++)
            inv.Slots[i] = new ItemSlot(Guid.Empty, 0);
        entity.Set(inv);

        entity.Set(new EquipmentState());

        var hotbar = new HotbarState();
        for (byte i = 0; i < MaxHotbar; i++)
            hotbar.Slots[i] = new HotbarSlot(SlotType.None, 0);
        entity.Set(hotbar);

        entity.Set(new AttackCooldown());
        entity.Set(new TradeState());
        entity.Set(new PartyState());
        entity.Set(new ShopState());
        entity.Set(new PlayerTag());

        return entityId;
    }
}
