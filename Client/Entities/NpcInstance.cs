using Arch.Core;
using CryBits.Client.Spawners;
using CryBits.Client.Worlds;
using CryBits.Entities.Npc;

namespace CryBits.Client.Entities;

internal class NpcInstance : Character
{
    public Npc Data { get; set; }

    public void Logic()
    {
        if (Entity == Entity.Null) Entity = NpcSpawner.Spawn(GameContext.Instance.World, this);

        Update();
    }
}
