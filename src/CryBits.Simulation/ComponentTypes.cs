using CryBits.Protocol.Serialization;
using CryBits.Simulation.Components;

namespace CryBits.Simulation;

public static class ComponentTypes
{
    public static void RegisterDefault()
    {
        ComponentTypeRegistry.Register<Position>(1);
        ComponentTypeRegistry.Register<PlayerTag>(2);
        ComponentTypeRegistry.Register<NpcTag>(3);
        ComponentTypeRegistry.Register<Vitals>(4);
        ComponentTypeRegistry.Register<LevelComponent>(5);
        ComponentTypeRegistry.Register<AttributesComponent>(6);
        ComponentTypeRegistry.Register<PlayerAppearance>(7);
        ComponentTypeRegistry.Register<InventoryState>(8);
        ComponentTypeRegistry.Register<EquipmentState>(9);
        ComponentTypeRegistry.Register<HotbarState>(10);
        ComponentTypeRegistry.Register<AttackCooldown>(11);
        ComponentTypeRegistry.Register<NpcState>(12);
        ComponentTypeRegistry.Register<GroundItem>(13);
        ComponentTypeRegistry.Register<GroundItemTag>(14);
        ComponentTypeRegistry.Register<ShopState>(15);
        ComponentTypeRegistry.Register<XpShareComponent>(16);
        ComponentTypeRegistry.Register<MapLoadingTag>(19);
        ComponentTypeRegistry.Register<AttackHit>(20);
        ComponentTypeRegistry.Register<CollidableTag>(21);
    }
}
