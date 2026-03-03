using CryBits.Entities.Npc;

namespace CryBits.Client.Entities;

internal class NpcInstance : Character
{
    public Npc Data { get; set; }

    /// <summary>Current vital values. Populated before spawn; kept in sync by MapNpcVitals handler.</summary>
    public short[] Vital = new short[(byte)Enums.Vital.Count];
}
