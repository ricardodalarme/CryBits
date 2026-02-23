using System.Linq;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.ECS;
using CryBits.Server.ECS.Components;
using CryBits.Server.Entities;
using CryBits.Server.World;
using LiteNetLib;
using LiteNetLib.Utils;

namespace CryBits.Server.Network;

internal static class Send
{
    public static void ToPlayer(GameSession session, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);
        session.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToPlayer(Player player, IServerPacket packet) =>
        ToPlayer(player.Session, packet);

    public static void ToAll(IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying))
            t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public static void ToAllBut(Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying && t.Character != player))
            ToPlayer(t, packet);
    }

    public static void ToMap(MapInstance mapInstance, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        var world = ServerContext.Instance.World;
        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying))
        {
            var pos = world.Get<PositionComponent>(t.Character!.EntityId);
            if (pos.MapId == mapInstance.Data.Id)
                t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
        }
    }

    public static void ToMapBut(MapInstance mapInstance, Player player, IServerPacket packet)
    {
        var data = new NetDataWriter();
        data.WriteObject(packet);

        var world = ServerContext.Instance.World;
        foreach (var t in GameWorld.Current.Sessions.Where(t => t.IsPlaying && t.Character != player))
        {
            var pos = world.Get<PositionComponent>(t.Character!.EntityId);
            if (pos.MapId == mapInstance.Data.Id)
                t.Connection.Send(data, DeliveryMethod.ReliableOrdered);
        }
    }
}
