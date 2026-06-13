using CryBits.Definitions.Items;
using System;

namespace CryBits.Server.Simulation.State.Components;

internal sealed class EquipmentState
{
    public Guid[] Slots { get; set; } = new Guid[(byte)CryBits.Definitions.Items.Equipment.Count];
}
