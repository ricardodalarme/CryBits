using System;

namespace CryBits.Packets.Client;

[Serializable] public struct PlayerDirectionPacket : IClientPacket { public byte Direction; }
[Serializable] public struct PlayerMovePacket : IClientPacket { public byte X, Y, Movement; }
[Serializable] public struct PlayerAttackPacket : IClientPacket;
[Serializable] public struct AddPointPacket : IClientPacket { public byte Attribute; }
[Serializable] public struct CollectItemPacket : IClientPacket;
[Serializable] public struct DropItemPacket : IClientPacket { public short Slot, Amount; }
[Serializable] public struct InventoryChangePacket : IClientPacket { public short OldSlot, NewSlot; }
[Serializable] public struct InventoryUsePacket : IClientPacket { public byte Slot; }
[Serializable] public struct EquipmentRemovePacket : IClientPacket { public byte Slot; }
[Serializable] public struct HotbarAddPacket : IClientPacket { public short HotbarSlot; public byte Type; public short Slot; }
[Serializable] public struct HotbarChangePacket : IClientPacket { public short OldSlot, NewSlot; }
[Serializable] public struct HotbarUsePacket : IClientPacket { public byte Slot; }
