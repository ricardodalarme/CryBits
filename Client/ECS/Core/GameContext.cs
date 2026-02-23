using System;
using System.Collections.Generic;
using CryBits.Client.ECS.Components;
using CryBits.Client.Entities;

namespace CryBits.Client.ECS;

/// <summary>
/// Single-instance runtime state for the game client.
///
/// Replaces the scattered static singletons that previously lived inside entity
/// classes (<c>Player.Me</c>, <c>Player.List</c>, <c>MapInstance.Current</c>, …).
///
/// Network handlers, update systems and render systems all receive or resolve
/// a reference to this object — nothing should reach into a static field anymore.
/// </summary>
internal sealed class GameContext
{
    // ─── Singleton access (intentionally kept as a static gateway            ───
    // ─── because handlers are still static methods bound by reflection.)    ───

    public static GameContext Instance { get; } = new();

    // ─── ECS world ──────────────────────────────────────────────────────────

    /// <summary>All live game entities and their components.</summary>
    public World World { get; } = new();

    // ─── Map data ───────────────────────────────────────────────────────────

    /// <summary>All known maps loaded from the server or disk cache.</summary>
    public Dictionary<Guid, MapInstance> Maps { get; } = [];

    /// <summary>The map the local player is currently standing on. Null before first join.</summary>
    public MapInstance? CurrentMap { get; set; }

    // ─── Indexed entity slots (server uses array indices, not GUIDs) ────────

    /// <summary>
    /// NPC entity ids indexed by the server-side map NPC slot number.
    /// Value -1 means the slot is empty.  Rebuilt each time the player changes maps.
    /// </summary>
    public int[] NpcSlots { get; private set; } = [];

    /// <summary>
    /// Map-item entity ids indexed by the server-side item slot number.
    /// Value -1 means the slot is empty.  Rebuilt each time the player changes maps.
    /// </summary>
    public int[] MapItemSlots { get; private set; } = [];

    // ─── Player lookup ──────────────────────────────────────────────────────

    private readonly Dictionary<string, int> _playersByName = [];

    /// <summary>
    /// Return the entity id of an existing player, or create a new one with the
    /// core character components already attached.
    /// </summary>
    public int FindOrCreatePlayer(string name)
    {
        if (_playersByName.TryGetValue(name, out var id)) return id;

        id = World.Create();
        _playersByName[name] = id;

        World.Add(id, new PlayerDataComponent { Name = name });
        World.Add(id, new TransformComponent());
        World.Add(id, new MovementComponent());
        World.Add(id, new AnimationComponent());
        World.Add(id, new VitalsComponent());
        World.Add(id, new CharacterSpriteComponent());
        World.Add(id, new MapContextComponent());

        return id;
    }

    /// <summary>Destroy a player entity and clean up the name lookup.</summary>
    public void RemovePlayer(string name)
    {
        if (!_playersByName.Remove(name, out var id)) return;
        World.Destroy(id);
    }

    /// <summary>
    /// Return the entity id of the local player (-1 when not yet initialised).
    /// This is the entity that carries <see cref="LocalPlayerTag"/>.
    /// </summary>
    public int GetLocalPlayer() => World.FindSingle<LocalPlayerTag>();

    /// <summary>
    /// Promote an existing player entity to the "local player" role by attaching
    /// all local-only components and tags.
    /// </summary>
    public void MakeLocalPlayer(int entityId)
    {
        if (!World.Has<LocalPlayerTag>(entityId))
            World.Add(entityId, new LocalPlayerTag());

        if (!World.Has<InputControlledTag>(entityId))
            World.Add(entityId, new InputControlledTag());

        if (!World.Has<CameraFocusTag>(entityId))
            World.Add(entityId, new CameraFocusTag());

        if (!World.Has<InventoryComponent>(entityId))
            World.Add(entityId, new InventoryComponent());

        if (!World.Has<HotbarComponent>(entityId))
            World.Add(entityId, new HotbarComponent());

        if (!World.Has<ExperienceComponent>(entityId))
            World.Add(entityId, new ExperienceComponent());

        if (!World.Has<PartyComponent>(entityId))
            World.Add(entityId, new PartyComponent());
    }

    // ─── NPC slot management ────────────────────────────────────────────────

    /// <summary>
    /// Destroy all existing NPC entities and allocate a fresh slot array.
    /// Called when entering a new map.
    /// </summary>
    public void ResetNpcSlots(int count)
    {
        foreach (var id in NpcSlots)
            if (id > 0) World.Destroy(id);

        NpcSlots = new int[count];
        Array.Fill(NpcSlots, -1);
    }

    /// <summary>
    /// Return the entity id for the given NPC slot, creating it (with default
    /// components) if it does not yet exist.
    /// </summary>
    public int GetOrCreateNpcEntity(byte slotIndex)
    {
        var id = NpcSlots[slotIndex];
        if (id > 0) return id;

        id = World.Create();
        NpcSlots[slotIndex] = id;

        World.Add(id, new NpcDataComponent { SlotIndex = slotIndex });
        World.Add(id, new TransformComponent());
        World.Add(id, new MovementComponent());
        World.Add(id, new AnimationComponent());
        World.Add(id, new VitalsComponent());
        World.Add(id, new CharacterSpriteComponent());

        return id;
    }

    /// <summary>Clear the NPC entity at the given slot (e.g. the NPC died).</summary>
    public void ClearNpcSlot(byte slotIndex)
    {
        var id = NpcSlots[slotIndex];
        if (id <= 0) return;
        World.Destroy(id);
        NpcSlots[slotIndex] = -1;
    }

    // ─── Map-item slot management ───────────────────────────────────────────

    /// <summary>
    /// Destroy all existing map-item entities and allocate a fresh slot array.
    /// Called when entering a new map.
    /// </summary>
    public void ResetMapItemSlots(int count)
    {
        foreach (var id in MapItemSlots)
            if (id > 0) World.Destroy(id);

        MapItemSlots = new int[count];
        Array.Fill(MapItemSlots, -1);
    }

    /// <summary>
    /// Return the entity id for the given map-item slot, creating it if needed.
    /// </summary>
    public int GetOrCreateMapItemEntity(byte slotIndex)
    {
        var id = MapItemSlots[slotIndex];
        if (id > 0) return id;

        id = World.Create();
        MapItemSlots[slotIndex] = id;
        World.Add(id, new MapItemComponent { SlotIndex = slotIndex });

        return id;
    }

    /// <summary>Clear the map-item entity at the given slot.</summary>
    public void ClearMapItemSlot(byte slotIndex)
    {
        var id = MapItemSlots[slotIndex];
        if (id <= 0) return;
        World.Destroy(id);
        MapItemSlots[slotIndex] = -1;
    }

    // ─── Player list (mirrors legacy Player.List) ────────────────────────────

    /// <summary>
    /// Find a player entity by name.
    /// Returns -1 when no such player is known.
    /// </summary>
    public int FindPlayer(string name) =>
        _playersByName.GetValueOrDefault(name, -1);

    /// <summary>Clear all remote player state (called when leaving a map).</summary>
    public void ClearAllPlayers()
    {
        foreach (var (name, id) in _playersByName)
        {
            // Keep the local player entity alive — only remove remote players.
            if (World.Has<LocalPlayerTag>(id)) continue;
            World.Destroy(id);
        }

        // Rebuild lookup keeping only the local player entry.
        var localId = GetLocalPlayer();
        _playersByName.Clear();

        if (localId > 0 && World.TryGet<PlayerDataComponent>(localId, out var pd))
            _playersByName[pd.Name] = localId;
    }

    /// <summary>
    /// Fully reset world state on disconnect: destroys all entities,
    /// clears map data, and zeroes out slot arrays.
    /// </summary>
    public void Reset()
    {
        World.Clear();
        _playersByName.Clear();
        Maps.Clear();
        CurrentMap = null;
        NpcSlots = [];
        MapItemSlots = [];
    }
}
