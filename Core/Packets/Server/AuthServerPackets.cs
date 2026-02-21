using System;

namespace CryBits.Packets.Server;

[Serializable] public struct ConnectPacket : IServerPacket;
[Serializable] public struct LatencyPacket : IServerPacket;
