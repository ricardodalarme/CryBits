using CryBits.Enums;

namespace CryBits;

/// <summary>
/// Marks a static handler method as the dispatch target for a specific packet id.
/// Discovered automatically at startup by each project's <c>PacketDispatcher</c>.
/// </summary>
/// <remarks>
/// Server handlers (receiving from clients) use <see cref="ClientPacket"/>.<br/>
/// Client and Editor handlers (receiving from server) use <see cref="ServerPacket"/>.
/// </remarks>
[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class PacketHandlerAttribute : System.Attribute
{
    public int PacketId { get; }

    public PacketHandlerAttribute(ClientPacket packet) => PacketId = (int)packet;
    public PacketHandlerAttribute(ServerPacket packet) => PacketId = (int)packet;
}
