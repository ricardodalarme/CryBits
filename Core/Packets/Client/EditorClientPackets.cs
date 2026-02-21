using System;

namespace CryBits.Packets.Client;

[Serializable] public struct WriteSettingsPacket : IClientPacket { public ServerConfig Config; }
[Serializable] public struct WriteClassesPacket : IClientPacket { public System.Collections.Generic.Dictionary<Guid, Entities.Class> Classes; }
[Serializable] public struct WriteMapsPacket : IClientPacket { public System.Collections.Generic.Dictionary<Guid, Entities.Map.Map> Maps; }
[Serializable] public struct WriteNpcsPacket : IClientPacket { public System.Collections.Generic.Dictionary<Guid, Entities.Npc.Npc> Npcs; }
[Serializable] public struct WriteItemsPacket : IClientPacket { public System.Collections.Generic.Dictionary<Guid, Entities.Item> Items; }
[Serializable] public struct WriteShopsPacket : IClientPacket { public System.Collections.Generic.Dictionary<Guid, Entities.Shop.Shop> Shops; }
[Serializable] public struct RequestSettingPacket : IClientPacket;
[Serializable] public struct RequestClassesPacket : IClientPacket;
[Serializable] public struct RequestMapPacket : IClientPacket { public Guid Id; public bool SendMap; }
[Serializable] public struct RequestMapsPacket : IClientPacket;
[Serializable] public struct RequestNpcsPacket : IClientPacket;
[Serializable] public struct RequestItemsPacket : IClientPacket;
[Serializable] public struct RequestShopsPacket : IClientPacket;
