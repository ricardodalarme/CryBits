using Arch.System;
using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics;
using CryBits.Client.Logic;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Systems.Character;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Systems.Player;
using CryBits.Client.Worlds;

namespace CryBits.Client.Systems;

/// <summary>
/// Owns and initialises every ECS system group used by the client.
///
/// <see cref="GameLoop"/> drives <see cref="Update"/> each tick (simulation).
/// <see cref="RenderPipeline"/> invokes the render groups in strict
/// world-draw order, interleaving them with <c>MapRenderer</c> tile passes:
///
/// To add, remove, or reorder a system, edit only this class.
/// </summary>
internal sealed class SystemScheduler(
    GameContext context,
    InputManager inputManager,
    PlayerSender playerSender,
    AudioManager audioManager,
    CameraManager cameraManager,
    Renderer renderer)
{
    public static SystemScheduler Instance { get; } = new(
        GameContext.Instance,
        InputManager.Instance,
        PlayerSender.Instance,
        AudioManager.Instance,
        CameraManager.Instance,
        Renderer.Instance);

    /// <summary>
    /// All dt-based simulation systems — ticked once per rendered frame by <see cref="GameLoop"/>.
    /// </summary>
    public Group<float> Update { get; } = new Group<float>(
        "DeltaTimeSystems",
        new FadeSystem(context.World),
        new FogSystem(context.World),
        new WeatherSimulationSystem(context),
        new WeatherSpawnSystem(context),
        new LightningSystem(context, audioManager),
        new MovementInputSystem(context, inputManager, playerSender),
        new ItemPickupSystem(context, inputManager, playerSender),
        new MovementSystem(context.World),
        new CameraSystem(context, cameraManager),
        new CharacterAnimationControllerSystem(context.World),
        new AnimatedSpriteSystem(context.World),
        new AttackSystem(context, inputManager, playerSender)
    );

    // ── Render phases ─────────────────────────────────────────────────────────
    // Each group is invoked by RenderPipeline.InGame() at a specific point in
    // the draw stack. Comments describe their position in the layer order.

    /// <summary> Drawn after the ground tile layer.</summary>
    public Group<int> GroundRender { get; } = new Group<int>(
            "GroundRenderSystems",
            new SpriteRenderSystem(context.World, renderer),
            new CharacterRenderSystem(context.World, renderer)
        );

    /// <summary>Drawn after the fringe tile layer.</summary>
    public Group<int> FringeRender { get; } = new Group<int>(
            "FringeRenderSystems",
            new VitalBarRenderSystem(context.World, renderer),
            new WeatherRenderSystem(context.World, renderer),
            new FogRenderSystem(context.World, renderer)
        );

    /// <summary>Call once before the main loop to let every system run its setup.</summary>
    public void Initialize()
    {
        Update.Initialize();
        GroundRender.Initialize();
        FringeRender.Initialize();
    }

    /// <summary>Call once when the application exits to release system resources.</summary>
    public void Dispose()
    {
        Update.Dispose();
        GroundRender.Dispose();
        FringeRender.Dispose();
    }
}
