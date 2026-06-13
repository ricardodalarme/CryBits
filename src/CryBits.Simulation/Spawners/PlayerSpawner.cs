using CryBits.Definitions.Catalog;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Classes;
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
    public static EntityId Spawn(World world, DefinitionCatalog catalog, string name,
        Class @class, bool genderMale, short textureNum)
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
            TextureNum = genderMale ? @class.TextureMale[textureNum] : @class.TextureFemale[textureNum],
            Genre = genderMale
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

        entity.Set(new CombatState());
        entity.Set(new TradeState());
        entity.Set(new PartyState());
        entity.Set(new ShopState());
        entity.Set(new PlayerTag());

        return entityId;
    }
}
