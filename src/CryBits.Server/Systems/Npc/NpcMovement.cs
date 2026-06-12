using CryBits.Definitions.Maps;
using CryBits.Definitions.Common;
using CryBits.Definitions.Characters;
using CryBits.Definitions.Npcs;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using static CryBits.Utils.DirectionUtils;
using static CryBits.Utils.RandomUtils;

namespace CryBits.Server.Systems.Npc;

internal static class NpcMovement
{
    internal static void TickMovement(NpcInstance npc, NpcSender npcSender, ChatSender chatSender)
    {
        var canMove = new bool[(byte)Direction.Count];
        var moved = false;
        var move = false;
        byte targetX = 0, targetY = 0;

        if (npc.Target != null)
        {
            targetX = npc.Target.X;
            targetY = npc.Target.Y;
            move = true;
        }
        else if (npc.MapInstance.Data.Npc[npc.Index].Zone > 0 &&
                 npc.MapInstance.Data.Attribute[npc.X, npc.Y].Zone != npc.MapInstance.Data.Npc[npc.Index].Zone)
        {
            for (byte x = 0; x < Map.Width; x++)
                for (byte y = 0; y < Map.Height; y++)
                    if (npc.MapInstance.Data.Attribute[x, y].Zone == npc.MapInstance.Data.Npc[npc.Index].Zone &&
                        !npc.MapInstance.Data.TileBlocked(x, y))
                    {
                        targetX = x;
                        targetY = y;
                        move = true;
                        break;
                    }
        }

        if (move)
        {
            if (npc.Vital[(byte)Vital.Hp] > npc.Data.Vital[(byte)Vital.Hp] * (npc.Data.FleeHealth / 100.0))
            {
                canMove[(byte)Direction.Up] = npc.Y > targetY;
                canMove[(byte)Direction.Down] = npc.Y < targetY;
                canMove[(byte)Direction.Left] = npc.X > targetX;
                canMove[(byte)Direction.Right] = npc.X < targetX;
            }
            else
            {
                canMove[(byte)Direction.Up] = npc.Y < targetY;
                canMove[(byte)Direction.Down] = npc.Y > targetY;
                canMove[(byte)Direction.Left] = npc.X < targetX;
                canMove[(byte)Direction.Right] = npc.X > targetX;
            }

            if (MyRandom.Next(0, 2) == 0)
            {
                for (byte d = 0; d < (byte)Direction.Count; d++)
                    if (!moved && canMove[d] && Move(npc, npcSender, (Direction)d))
                        moved = true;
            }
            else
            {
                for (short d = (byte)Direction.Count - 1; d >= 0; d--)
                    if (!moved && canMove[d] && Move(npc, npcSender, (Direction)d))
                        moved = true;
            }
        }

        if (npc.Data.Behaviour == (byte)Behaviour.Friendly || npc.Target == null)
            if (MyRandom.Next(0, 3) == 0 && !moved)
            {
                if (npc.Data.Movement == MovementStyle.MoveRandomly)
                    Move(npc, npcSender, (Direction)MyRandom.Next(0, 4), 1, true);
                else if (npc.Data.Movement == MovementStyle.TurnRandomly)
                {
                    npc.Direction = (Direction)MyRandom.Next(0, 4);
                    npcSender.MapNpcDirection(npc);
                }
            }
    }

    internal static bool Move(NpcInstance npc, NpcSender npcSender, Direction direction, byte movement = 1, bool checkZone = false)
    {
        byte nextX = npc.X, nextY = npc.Y;

        npc.Direction = direction;
        npcSender.MapNpcDirection(npc);
        NextTile(direction, ref nextX, ref nextY);

        if (Map.OutLimit(nextX, nextY)) return false;
        if (npc.MapInstance.TileBlocked(npc.X, npc.Y, direction)) return false;
        if (checkZone && npc.MapInstance.Data.Attribute[nextX, nextY].Zone != npc.MapInstance.Data.Npc[npc.Index].Zone) return false;

        npc.X = nextX;
        npc.Y = nextY;
        npcSender.MapNpcMovement(npc, movement);
        return true;
    }
}
