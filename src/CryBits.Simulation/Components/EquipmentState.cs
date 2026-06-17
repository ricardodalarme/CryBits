namespace CryBits.Simulation.Components;

public sealed class EquipmentState
{
    public Guid[] Slots { get; set; } = new Guid[(byte)CryBits.Definitions.Items.Equipment.Count];
}
