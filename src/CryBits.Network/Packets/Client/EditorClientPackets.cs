using CryBits.Definitions.Classes;
using CryBits.Definitions.Items;
using CryBits.Definitions.Maps;
using CryBits.Definitions.Npcs;
using CryBits.Definitions.Shops;
using System;
using System.Collections.Generic;

namespace CryBits.Network.Packets.Client;

[Serializable] public struct WriteClassesPacket : IClientPacket { public Dictionary<Guid, Class> Classes; }
[Serializable] public struct WriteMapsPacket : IClientPacket { public Dictionary<Guid, Map> Maps; }
[Serializable] public struct WriteNpcsPacket : IClientPacket { public Dictionary<Guid, Npc> Npcs; }
[Serializable] public struct WriteItemsPacket : IClientPacket { public Dictionary<Guid, Item> Items; }
[Serializable] public struct WriteShopsPacket : IClientPacket { public Dictionary<Guid, Shop> Shops; }
[Serializable] public struct RequestClassesPacket : IClientPacket;
[Serializable] public struct RequestMapPacket : IClientPacket { public Guid Id; public bool SendMap; }
[Serializable] public struct RequestMapsPacket : IClientPacket;
[Serializable] public struct RequestNpcsPacket : IClientPacket;
[Serializable] public struct RequestItemsPacket : IClientPacket;
[Serializable] public struct RequestShopsPacket : IClientPacket;
