using System.Linq;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Packets.Client;

namespace CryBits.Client.Network.Senders;

internal class AccountSender(PacketSender packetSender)
{
    public static AccountSender Instance { get; } = new(PacketSender.Instance);

    public void CreateCharacter() =>
        packetSender.Packet(new CreateCharacterPacket
        {
            Name = TextBoxes.CreateCharacterName.Text,
            ClassId = Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value.Id.ToString(),
            GenderMale = CheckBoxes.GenderMale.Checked,
            TextureNum = PanelsEvents.CreateCharacterTex
        });

    public void CharacterUse() =>
        packetSender.Packet(new CharacterUsePacket
        {
            CharacterIndex = PanelsEvents.SelectCharacter
        });

    public void CharacterCreate() => packetSender.Packet(new CharacterCreatePacket());

    public void CharacterDelete() =>
        packetSender.Packet(new CharacterDeletePacket
        {
            CharacterIndex = PanelsEvents.SelectCharacter
        });
}
