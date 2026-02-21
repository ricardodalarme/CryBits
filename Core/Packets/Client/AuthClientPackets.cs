using System;

namespace CryBits.Packets.Client;

[Serializable]
public struct ConnectPacket : IClientPacket
{
    public string Username;
    public string Password;
    public bool IsClientAccess;
}

[Serializable] public struct LatencyPacket : IClientPacket;

[Serializable]
public struct RegisterPacket : IClientPacket
{
    public string Username;
    public string Password;
}
