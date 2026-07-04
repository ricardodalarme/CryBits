using CryBits.Client.Core;
using CryBits.Client.Network.Senders;
using CryBits.Definitions.Catalog;
using CryBits.Definitions.Helpers.Extensions;
using CryBits.Definitions.Items;
using CryBits.Simulation.Intents;
using Attribute = CryBits.Definitions.Characters.Attribute;

namespace CryBits.Client.UI.Game.ViewModels;

internal sealed class CharacterEquipmentItemViewModel
{
    public short Index { get; set; }
    public Guid ItemId { get; set; }
    public Item? Definition { get; set; }
}

internal sealed class CharacterViewModel(
    GameContext context,
    IntentSender intentSender,
    DefinitionCatalog catalog)
{
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

    public CharacterEquipmentItemViewModel[] Equipment { get; private set; } = new CharacterEquipmentItemViewModel[(byte)CryBits.Definitions.Items.Equipment.Count];

    public void Refresh()
    {
        if (!context.LocalPlayerEntity.HasValue) return;
        var entity = context.LocalPlayerEntity.Value;

        var appearance = context.World.Get<CryBits.Simulation.Components.PlayerAppearance>(entity);
        Name = appearance?.Name ?? string.Empty;
        TextureNum = (byte)(appearance?.TextureNum ?? 0);

        var lvl = context.World.Get<CryBits.Simulation.Components.LevelComponent>(entity);
        if (lvl != null)
        {
            Level = lvl.Level;
            Points = lvl.Points;
        }

        var attrs = context.World.Get<CryBits.Simulation.Components.AttributesComponent>(entity);
        if (attrs != null)
        {
            Strength = attrs.Values[(byte)Attribute.Strength];
            Resistance = attrs.Values[(byte)Attribute.Resistance];
            Intelligence = attrs.Values[(byte)Attribute.Intelligence];
            Agility = attrs.Values[(byte)Attribute.Agility];
            Vitality = attrs.Values[(byte)Attribute.Vitality];
        }

        var equip = context.World.Get<CryBits.Simulation.Components.EquipmentState>(entity);
        if (equip != null)
        {
            for (var i = 0; i < equip.Slots.Length; i++)
            {
                var itemId = equip.Slots[i];
                if (Equipment[i] == null)
                {
                    Equipment[i] = new CharacterEquipmentItemViewModel { Index = (short)i };
                }
                Equipment[i].ItemId = itemId;
                Equipment[i].Definition = itemId != Guid.Empty ? catalog.Items.Get(itemId) : null;
            }
        }
    }

    public void SpendPoint(Attribute attr)
    {
        intentSender.Send(new AddPointIntent(default, (byte)attr));
    }

    public void RemoveEquipment(short slot)
    {
        intentSender.Send(new EquipmentRemoveIntent(default, (byte)slot));
    }
}
