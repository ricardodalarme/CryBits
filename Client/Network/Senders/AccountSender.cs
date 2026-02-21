using System.Linq;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal static class AccountSender
{
    public static void CreateCharacter() =>
        Send.Packet(new CreateCharacterPacket
        {
            Name = TextBoxes.CreateCharacterName.Text,
            ClassId = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value.Id.ToString(),
            GenderMale = CheckBoxes.GenderMale.Checked,
            TextureNum = PanelsEvents.CreateCharacterTex
        });

    public static void CharacterUse() =>
        Send.Packet(new CharacterUsePacket
        {
            CharacterIndex = PanelsEvents.SelectCharacter
        });

    public static void CharacterCreate() => Send.Packet(new CharacterCreatePacket());

    public static void CharacterDelete() =>
        Send.Packet(new CharacterDeletePacket
        {
            CharacterIndex = PanelsEvents.SelectCharacter
        });
}
