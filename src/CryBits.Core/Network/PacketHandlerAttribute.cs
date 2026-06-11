namespace CryBits;

/// <summary>
/// Marks a static handler method as the dispatch target for the packet type
/// declared in its parameter list. Discovered automatically at startup by each
/// project's <c>PacketDispatcher</c>.
/// The packet type is inferred from the method's <see cref="Packets.Client.IClientPacket"/>
/// or <see cref="Packets.Server.IServerPacket"/> parameter.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PacketHandlerAttribute : System.Attribute { }
