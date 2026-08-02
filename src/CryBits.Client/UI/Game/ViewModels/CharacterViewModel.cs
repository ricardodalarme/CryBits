using CryBits.Client.Components;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Components;
using CryBits.Simulation.Core;
using CryBits.Simulation.Events;
using CryBits.Simulation.Intents;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class CharacterEquipmentItemViewModel
{
    public short Index { get; set; }
    public Guid ItemId { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class CharacterViewModel : IDisposable
{
    private readonly DefinitionCatalog _catalog;
    private readonly IntentSender _intentSender;

    private readonly IDisposable _appearanceSubscription;
    private readonly IDisposable _levelSubscription;
    private readonly IDisposable _attributesSubscription;
    private readonly IDisposable _equipmentSubscription;

    public string Name { get; private set; } = string.Empty;
    public int Level { get; private set; }
    public int Points { get; private set; }
    public bool HasPoints => Points > 0;
    public short Strength { get; private set; }
    public short Resistance { get; private set; }
    public short Intelligence { get; private set; }
    public short Agility { get; private set; }
    public short Vitality { get; private set; }
    public byte TextureNum { get; private set; }

    public CharacterEquipmentItemViewModel[] Equipment { get; } =
        new CharacterEquipmentItemViewModel[(byte)CryBits.Definitions.Items.Equipment.Count];

    public CharacterViewModel(World world, IntentSender intentSender, DefinitionCatalog catalog)
    {
        _catalog = catalog;
        _intentSender = intentSender;

        _appearanceSubscription = world.Events.On<PlayerAppearance>()
            .With<LocalPlayerTag>()
            .OnChanged(OnAppearanceChanged);

        _levelSubscription = world.Events.On<LevelComponent>()
            .With<LocalPlayerTag>()
            .OnChanged(OnLevelChanged);

        _attributesSubscription = world.Events.On<AttributesComponent>()
            .With<LocalPlayerTag>()
            .OnChanged(OnAttributesChanged);

        _equipmentSubscription = world.Events.On<EquipmentState>()
            .With<LocalPlayerTag>()
            .OnChanged(OnEquipmentChanged);
    }

    private void OnAppearanceChanged(ComponentChanged<PlayerAppearance> evt)
    {
        Name = evt.Component.Name;
        TextureNum = (byte)evt.Component.TextureNum;
    }

    private void OnLevelChanged(ComponentChanged<LevelComponent> evt)
    {
        Level = evt.Component.Level;
        Points = evt.Component.Points;
    }

    private void OnAttributesChanged(ComponentChanged<AttributesComponent> evt)
    {
        Strength = evt.Component.Values[(byte)Attribute.Strength];
        Resistance = evt.Component.Values[(byte)Attribute.Resistance];
        Intelligence = evt.Component.Values[(byte)Attribute.Intelligence];
        Agility = evt.Component.Values[(byte)Attribute.Agility];
        Vitality = evt.Component.Values[(byte)Attribute.Vitality];
    }

    private void OnEquipmentChanged(ComponentChanged<EquipmentState> evt)
    {
        for (var i = 0; i < evt.Component.Slots.Length; i++)
        {
            var itemId = evt.Component.Slots[i];
            Equipment[i] ??= new CharacterEquipmentItemViewModel { Index = (short)i };
            Equipment[i].ItemId = itemId;
            Equipment[i].Definition = itemId != Guid.Empty ? _catalog.Items.Get(itemId) : null;
        }
    }

    public void SpendPoint(Attribute attr)
    {
        _intentSender.Send(new AddPointIntent(default, (byte)attr));
    }

    public void RemoveEquipment(short slot)
    {
        _intentSender.Send(new EquipmentRemoveIntent(default, (byte)slot));
    }

    public void Dispose()
    {
        _appearanceSubscription.Dispose();
        _levelSubscription.Dispose();
        _attributesSubscription.Dispose();
        _equipmentSubscription.Dispose();
    }
}
