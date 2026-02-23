using System;
using System.Collections.Generic;
using CryBits.Client.ECS;
using CryBits.Client.ECS.Components;
using CryBits.Client.Framework.Constants;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Packets.Server;
using static CryBits.Globals;
using static CryBits.Utils;
using Attribute = CryBits.Enums.Attribute;

namespace CryBits.Client.Network.Handlers;

internal static class PlayerHandler
{
    private static GameContext Ctx => GameContext.Instance;

    [PacketHandler]
    internal static void PlayerData(PlayerDataPacket packet)
    {
        var name = packet.Name;
        var localId = Ctx.GetLocalPlayer();

        // Determine whether this packet describes the local player.
        bool isLocal = localId > 0 &&
                       Ctx.World.TryGet<PlayerDataComponent>(localId, out var lpd) &&
                       lpd.Name == name;

        var id = isLocal ? localId : Ctx.FindOrCreatePlayer(name);

        // Mark as local player on first receive (name matches what we logged in as).
        if (isLocal) Ctx.MakeLocalPlayer(id);

        Ctx.World.Get<PlayerDataComponent>(id).Level = packet.Level;
        Ctx.World.Get<CharacterSpriteComponent>(id).TextureNum = packet.TextureNum;

        var transform = Ctx.World.Get<TransformComponent>(id);
        transform.TileX = packet.X;
        transform.TileY = packet.Y;
        transform.Direction = (Direction)packet.Direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;

        Ctx.World.Get<MovementComponent>(id).Current = Movement.Stopped;

        var vitals = Ctx.World.Get<VitalsComponent>(id);
        for (byte n = 0; n < (byte)Vital.Count; n++)
        {
            vitals.Current[n] = packet.Vital[n];
            vitals.Max[n] = packet.MaxVital[n];
        }

        var playerData = Ctx.World.Get<PlayerDataComponent>(id);
        for (byte n = 0; n < (byte)Attribute.Count; n++) playerData.Attributes[n] = packet.Attribute[n];
        for (byte n = 0; n < (byte)Equipment.Count; n++)
            playerData.EquippedItems[n] = Item.List.GetValueOrDefault(packet.Equipment[n]);

        playerData.MapId = packet.MapId;

        if (Ctx.World.TryGet<MapContextComponent>(id, out var mc)) mc.MapId = packet.MapId;

        if (Ctx.Maps.TryGetValue(packet.MapId, out var mapInstance))
            Ctx.CurrentMap = mapInstance;
    }

    [PacketHandler]
    internal static void PlayerPosition(PlayerPositionPacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;

        var transform = Ctx.World.Get<TransformComponent>(id);
        transform.TileX = packet.X;
        transform.TileY = packet.Y;
        transform.Direction = (Direction)packet.Direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;

        Ctx.World.Get<MovementComponent>(id).Current = Movement.Stopped;
    }

    [PacketHandler]
    internal static void PlayerVitals(PlayerVitalsPacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;

        var vitals = Ctx.World.Get<VitalsComponent>(id);
        for (byte i = 0; i < (byte)Vital.Count; i++)
        {
            vitals.Current[i] = packet.Vital[i];
            vitals.Max[i] = packet.MaxVital[i];
        }
    }

    [PacketHandler]
    internal static void PlayerEquipments(PlayerEquipmentsPacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;

        var playerData = Ctx.World.Get<PlayerDataComponent>(id);
        for (byte i = 0; i < (byte)Equipment.Count; i++)
            playerData.EquippedItems[i] = Item.List.GetValueOrDefault(packet.Equipments[i]);
    }

    [PacketHandler]
    internal static void PlayerLeave(PlayerLeavePacket packet)
    {
        Ctx.RemovePlayer(packet.Name);
    }

    [PacketHandler]
    internal static void PlayerMove(PlayerMovePacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;

        // Skip the local player — PlayerInputSystem already applied the move
        // client-side for smooth prediction. Applying it again from the server
        // echo would zero the pixel offset mid-slide and snap the character back.
        if (id == Ctx.GetLocalPlayer()) return;

        var transform = Ctx.World.Get<TransformComponent>(id);
        var movement = Ctx.World.Get<MovementComponent>(id);

        var prevX = transform.TileX;
        var prevY = transform.TileY;

        transform.TileX = packet.X;
        transform.TileY = packet.Y;
        transform.Direction = (Direction)packet.Direction;
        transform.PixelOffsetX = 0;
        transform.PixelOffsetY = 0;

        movement.Current = (Movement)packet.Movement;

        // Set leading-edge pixel offset for smooth interpolation.
        if (prevX != transform.TileX || prevY != transform.TileY)
            switch (transform.Direction)
            {
                case Direction.Up: transform.PixelOffsetY = Grid; break;
                case Direction.Down: transform.PixelOffsetY = Grid * -1; break;
                case Direction.Right: transform.PixelOffsetX = Grid * -1; break;
                case Direction.Left: transform.PixelOffsetX = Grid; break;
            }
    }

    [PacketHandler]
    internal static void PlayerDirection(PlayerDirectionPacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;
        if (id == Ctx.GetLocalPlayer()) return;

        Ctx.World.Get<TransformComponent>(id).Direction = (Direction)packet.Direction;
    }

    [PacketHandler]
    internal static void PlayerAttack(PlayerAttackPacket packet)
    {
        var id = Ctx.FindPlayer(packet.Name);
        if (id < 0) return;

        var animation = Ctx.World.Get<AnimationComponent>(id);
        animation.IsAttacking = true;
        animation.AttackTimer = Environment.TickCount;

        if (packet.Victim == string.Empty) return;

        if (packet.VictimType == (byte)Target.Player)
        {
            var victimId = Ctx.FindPlayer(packet.Victim);
            if (victimId < 0) return;

            if (Ctx.World.TryGet<CharacterSpriteComponent>(victimId, out var vs)) vs.HurtTimer = Environment.TickCount;
            if (Ctx.World.TryGet<TransformComponent>(victimId, out var vt))
                SpawnBlood(vt.TileX, vt.TileY);
        }
        else if (packet.VictimType == (byte)Target.Npc)
        {
            var npcIndex = byte.Parse(packet.Victim);
            var npcId = npcIndex < Ctx.NpcSlots.Length ? Ctx.NpcSlots[npcIndex] : -1;
            if (npcId < 0) return;

            if (Ctx.World.TryGet<CharacterSpriteComponent>(npcId, out var ns)) ns.HurtTimer = Environment.TickCount;
            if (Ctx.World.TryGet<TransformComponent>(npcId, out var nt))
                SpawnBlood(nt.TileX, nt.TileY);
        }
    }

    [PacketHandler]
    internal static void PlayerExperience(PlayerExperiencePacket packet)
    {
        var localId = Ctx.GetLocalPlayer();
        if (localId < 0) return;

        var xp = Ctx.World.Get<ExperienceComponent>(localId);
        xp.Current = packet.Experience;
        xp.Needed = packet.ExpNeeded;
        xp.Points = packet.Points;

        var hasPoints = xp.Points > 0;
        Buttons.AttributesStrength.Visible = hasPoints;
        Buttons.AttributesResistance.Visible = hasPoints;
        Buttons.AttributesIntelligence.Visible = hasPoints;
        Buttons.AttributesAgility.Visible = hasPoints;
        Buttons.AttributesVitality.Visible = hasPoints;
    }

    [PacketHandler]
    internal static void PlayerInventory(PlayerInventoryPacket packet)
    {
        var localId = Ctx.GetLocalPlayer();
        if (localId < 0) return;

        var inv = Ctx.World.Get<InventoryComponent>(localId);
        for (byte i = 0; i < MaxInventory; i++)
            inv.Slots[i] = new ItemSlot(Item.List.GetValueOrDefault(packet.ItemIds[i]), packet.Amounts[i]);
    }

    [PacketHandler]
    internal static void PlayerHotbar(PlayerHotbarPacket packet)
    {
        var localId = Ctx.GetLocalPlayer();
        if (localId < 0) return;

        var hotbar = Ctx.World.Get<HotbarComponent>(localId);
        for (byte i = 0; i < MaxHotbar; i++)
            hotbar.Slots[i] = new HotbarSlot((SlotType)packet.Types[i], packet.Slots[i]);
    }

    // ─── Helpers ─────────────────────────────────────────────────────────────

    private static void SpawnBlood(byte tileX, byte tileY)
    {
        var bloodId = Ctx.World.Create();
        Ctx.World.Add(bloodId, new BloodSplatComponent
        {
            TextureNum = (byte)MyRandom.Next(0, 3),
            TileX = tileX,
            TileY = tileY,
            Opacity = 255,
            NextFadeAt = Environment.TickCount + 100
        });
    }
}
