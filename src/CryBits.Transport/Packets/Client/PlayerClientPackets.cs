using MemoryPack;

namespace CryBits.Transport.Packets.Client;

[MemoryPackable] public partial class PlayerMovePacket : IClientPacket { public byte Direction, Movement; }
[MemoryPackable] public partial class PlayerAttackPacket : IClientPacket;
[MemoryPackable] public partial class AddPointPacket : IClientPacket { public byte Attribute; }
[MemoryPackable] public partial class CollectItemPacket : IClientPacket;
[MemoryPackable] public partial class DropItemPacket : IClientPacket { public short Slot, Amount; }
[MemoryPackable] public partial class InventoryChangePacket : IClientPacket { public short OldSlot, NewSlot; }
[MemoryPackable] public partial class InventoryUsePacket : IClientPacket { public byte Slot; }
[MemoryPackable] public partial class EquipmentRemovePacket : IClientPacket { public byte Slot; }
[MemoryPackable] public partial class HotbarAddPacket : IClientPacket { public short HotbarSlot; public byte Type; public short Slot; }
[MemoryPackable] public partial class HotbarChangePacket : IClientPacket { public short OldSlot, NewSlot; }
[MemoryPackable] public partial class HotbarUsePacket : IClientPacket { public byte Slot; }
