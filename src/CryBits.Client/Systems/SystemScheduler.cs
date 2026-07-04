using CryBits.Client.Framework.Audio;
using CryBits.Client.Graphics;
using CryBits.Client.Managers;
using CryBits.Client.Network.Senders;
using CryBits.Client.Systems.Character;
using CryBits.Client.UI;
using CryBits.Client.Systems.Combat;
using CryBits.Client.Systems.Core;
using CryBits.Client.Systems.Map;
using CryBits.Client.Systems.Movement;
using CryBits.Client.Systems.Player;
using CryBits.Client.Worlds;

namespace CryBits.Client.Systems;

internal sealed class SystemScheduler(
    GameContext context,
    InputManager inputManager,
    IntentSender intentSender,
    AudioManager audioManager,
    CameraManager cameraManager,
    Renderer renderer,
    UiContext uiContext)
{
    public Client.Core.SystemScheduler Simulation { get; } = new();
    public Client.Core.SystemScheduler Ground { get; } = new();
    public Client.Core.SystemScheduler Fringe { get; } = new();

    public void Initialize()
    {
        Simulation
            .AddSimulation(new FadeSystem(context.World))
            .AddSimulation(new FogSystem(context.World))
            .AddSimulation(new WeatherSimulationSystem(context))
            .AddSimulation(new WeatherSpawnSystem(context))
            .AddSimulation(new LightningSystem(context, audioManager))
            .AddSimulation(new MovementInputSystem(context, inputManager, intentSender))
            .AddSimulation(new ItemPickupSystem(context, inputManager, intentSender))
            .AddSimulation(new MovementSystem(context.World))
            .AddSimulation(new CameraSystem(context, cameraManager))
            .AddSimulation(new CharacterAnimationSystem(context.World))
            .AddSimulation(new AttackHitSystem(context))
            .AddSimulation(new AttackSystem(context, inputManager, intentSender, uiContext))
            .AddSimulation(new DamageDecaySystem(context.World));

        Ground
            .AddRender(new SpriteRenderSystem(context.World, renderer))
            .AddRender(new CharacterRenderSystem(context.World, renderer));

        Fringe
            .AddRender(new VitalBarRenderSystem(context.World, renderer))
            .AddRender(new WeatherRenderSystem(context.World, renderer))
            .AddRender(new FogRenderSystem(context.World, renderer));
    }
}
