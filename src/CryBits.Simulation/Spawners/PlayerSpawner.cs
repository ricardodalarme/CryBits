using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
using CryBits.Definitions.Common;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Definitions.Slots;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Formulas;
using CryBits.Simulation.State;
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

        world.Set(entityId, new Position(MapId: data.MapId, X: data.X, Y: data.Y, Direction: (Direction)data.Direction));

        world.Set(entityId, new PlayerAppearance(
            Name: data.Name,
            ClassId: data.ClassId,
            TextureNum: data.Gender == Gender.Male ? @class.TextureMale[data.TextureNum] : @class.TextureFemale[data.TextureNum],
            Gender: data.Gender
        ));

        world.Set(entityId, new LevelComponent(Level: data.Level, Experience: data.Experience, Points: data.Points));
        world.Set(entityId, new AttributesComponent((short[])data.Attributes.Clone()));

        var hp = data.Hp > 0 ? data.Hp : maxHp;
        var mp = data.Mp > 0 ? data.Mp : maxMp;
        world.Set(entityId, new Vitals(Hp: hp, Mp: mp, MaxHp: maxHp, MaxMp: maxMp));

        var invSlots = new ItemSlot[MaxInventory];
        for (byte i = 0; i < MaxInventory; i++)
            invSlots[i] = i < data.InventoryIds.Length
                ? new ItemSlot(data.InventoryIds[i], data.InventoryAmounts[i])
                : new ItemSlot(Guid.Empty, 0);
        world.Set(entityId, new InventoryState(invSlots));

        var equipSlots = new Guid[(byte)Equipment.Count];
        for (byte i = 0; i < (byte)Equipment.Count; i++)
            equipSlots[i] = i < data.Equipment.Length ? data.Equipment[i] : Guid.Empty;
        world.Set(entityId, new EquipmentState(equipSlots));

        var hotbarSlots = new HotbarSlot[MaxHotbar];
        for (byte i = 0; i < MaxHotbar; i++)
            hotbarSlots[i] = i < data.HotbarTypes.Length
                ? new HotbarSlot((SlotType)data.HotbarTypes[i], data.HotbarSlots[i])
                : new HotbarSlot(SlotType.None, 0);
        world.Set(entityId, new HotbarState(hotbarSlots));

        world.Set(entityId, new AttackCooldown(0));
        world.Set(entityId, new PlayerTag());

        return entityId;
    }

    public static EntityId Spawn(World world, DefinitionCatalog catalog, string name,
        Class @class, Gender gender, short textureNum)
    {
        var entityId = world.Entities.Create();

        var maxHp = VitalFormulas.MaxVital(Vital.Hp, @class.Vital[(byte)Vital.Hp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);
        var maxMp = VitalFormulas.MaxVital(Vital.Mp, @class.Vital[(byte)Vital.Mp],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Vitality],
            @class.Attribute[(byte)CryBits.Definitions.Characters.Attribute.Intelligence], 1);

        world.Set(entityId, new Position(MapId: @class.SpawnMapId, X: @class.SpawnX, Y: @class.SpawnY, Direction: (Direction)@class.SpawnDirection));

        world.Set(entityId, new PlayerAppearance(
            Name: name,
            ClassId: @class.Id,
            TextureNum: gender == Gender.Male ? @class.TextureMale[textureNum] : @class.TextureFemale[textureNum],
            Gender: gender
        ));

        world.Set(entityId, new LevelComponent(Level: 1));
        world.Set(entityId, new AttributesComponent((short[])@class.Attribute.Clone()));
        world.Set(entityId, new Vitals(Hp: maxHp, Mp: maxMp, MaxHp: maxHp, MaxMp: maxMp));

        var invSlots = new ItemSlot[MaxInventory];
        for (byte i = 0; i < MaxInventory; i++)
            invSlots[i] = new ItemSlot(Guid.Empty, 0);
        world.Set(entityId, new InventoryState(invSlots));

        var equipSlots = new Guid[(byte)Equipment.Count];
        world.Set(entityId, new EquipmentState(equipSlots));

        var hotbarSlots = new HotbarSlot[MaxHotbar];
        for (byte i = 0; i < MaxHotbar; i++)
            hotbarSlots[i] = new HotbarSlot(SlotType.None, 0);
        world.Set(entityId, new HotbarState(hotbarSlots));

        world.Set(entityId, new AttackCooldown());
        world.Set(entityId, new PlayerTag());

        return entityId;
    }
}
