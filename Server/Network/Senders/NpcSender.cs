using CryBits.Entities.Npc;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Packets.Server;
using CryBits.Server.Entities;
using CryBits.Server.World;

namespace CryBits.Server.Network.Senders;

internal sealed class NpcSender(PackageSender packageSender)
{
    public static NpcSender Instance { get; } = new(PackageSender.Instance);

    private readonly PackageSender _packageSender = packageSender;

    public void Npcs(GameSession session)
    {
        _packageSender.ToPlayer(session, new NpcsPacket { List = Npc.List });
    }

    public void MapNpcs(Player player, MapInstance mapInstance)
    {
        var packet = new MapNpcsPacket { Npcs = new PacketsMapNpc[mapInstance.Npc.Length] };
        for (byte i = 0; i < mapInstance.Npc.Length; i++)
        {
            packet.Npcs[i] = new PacketsMapNpc
            {
                NpcId = mapInstance.Npc[i].Data.GetId(),
                X = mapInstance.Npc[i].X,
                Y = mapInstance.Npc[i].Y,
                Direction = (byte)mapInstance.Npc[i].Direction,
                Vital = new short[(byte)Vital.Count]
            };
            for (byte n = 0; n < (byte)Vital.Count; n++) packet.Npcs[i].Vital[n] = mapInstance.Npc[i].Vital[n];
        }

        _packageSender.ToPlayer(player, packet);
    }

    public void MapNpc(NpcInstance npcInstance)
    {
        var packet = new MapNpcPacket
        {
            Index = npcInstance.Index,
            NpcId = npcInstance.Data.GetId(),
            X = npcInstance.X,
            Y = npcInstance.Y,
            Direction = (byte)npcInstance.Direction,
            Vital = new short[(byte)Vital.Count]
        };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        _packageSender.ToMap(npcInstance.MapInstance, packet);
    }

    public void MapNpcMovement(NpcInstance npcInstance, byte movement)
    {
        var speed = movement == (byte)Movement.Moving
            ? Globals.RunSpeedPixelsPerSecond
            : Globals.WalkSpeedPixelsPerSecond;

        _packageSender.ToMap(npcInstance.MapInstance,
            new MapNpcMovementPacket
            {
                Index = npcInstance.Index,
                X = npcInstance.X,
                Y = npcInstance.Y,
                Direction = (byte)npcInstance.Direction,
                Movement = movement,
                Speed = speed
            });
    }

    public void MapNpcDirection(NpcInstance npcInstance)
    {
        _packageSender.ToMap(npcInstance.MapInstance,
            new MapNpcDirectionPacket { Index = npcInstance.Index, Direction = (byte)npcInstance.Direction });
    }

    public void MapNpcVitals(NpcInstance npcInstance)
    {
        var packet = new MapNpcVitalsPacket { Index = npcInstance.Index, Vital = new short[(byte)Vital.Count] };
        for (byte n = 0; n < (byte)Vital.Count; n++) packet.Vital[n] = npcInstance.Vital[n];
        _packageSender.ToMap(npcInstance.MapInstance, packet);
    }

    public void MapNpcAttack(NpcInstance npcInstance, string victim = "", Target victimType = 0)
    {
        _packageSender.ToMap(npcInstance.MapInstance,
            new MapNpcAttackPacket { Index = npcInstance.Index, Victim = victim, VictimType = (byte)victimType });
    }

    public void MapNpcDied(NpcInstance npcInstance)
    {
        _packageSender.ToMap(npcInstance.MapInstance, new MapNpcDiedPacket { Index = npcInstance.Index });
    }
}
