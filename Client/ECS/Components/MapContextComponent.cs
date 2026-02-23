using System;

namespace CryBits.Client.ECS.Components;

/// <summary>
/// Records which map an entity is currently located on.
/// The <see cref="Systems.PlayerRenderSystem"/> uses this to skip rendering of
/// players that are on a different map from the local player.
/// </summary>
public sealed class MapContextComponent : IComponent
{
    public Guid MapId { get; set; }
}
