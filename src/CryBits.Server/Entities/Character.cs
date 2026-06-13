using System;
using CryBits.Definitions.Common;

namespace CryBits.Server.Entities;

internal abstract class Character
{
    public abstract Guid Id { get; }

    // Core character fields.
    public MapInstance MapInstance { get; set; }
    public byte X { get; set; }
    public byte Y { get; set; }
    public Direction Direction { get; set; }
    public short[] Vital { get; } = new short[(byte)CryBits.Definitions.Characters.Vital.Count];
}
