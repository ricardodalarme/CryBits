using System.Linq;
using CryBits.Client.Framework.Constants;
using CryBits.Client.UI.Events;
using CryBits.Entities;
using CryBits.Enums;
using LiteNetLib.Utils;

namespace CryBits.Client.Network.Senders;

internal static class AccountSender
{
    public static void CreateCharacter()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CreateCharacter);
        data.Put(TextBoxes.CreateCharacterName.Text);
        data.Put(Class.List.ElementAt(PanelsEvents.CreateCharacterClass).Value.Id.ToString());
        data.Put(CheckBoxes.GenderMale.Checked);
        data.Put(PanelsEvents.CreateCharacterTex);
        Send.Packet(data);
    }

    public static void CharacterUse()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterUse);
        data.Put(PanelsEvents.SelectCharacter);
        Send.Packet(data);
    }

    public static void CharacterCreate()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterCreate);
        Send.Packet(data);
    }

    public static void CharacterDelete()
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ClientPacket.CharacterDelete);
        data.Put(PanelsEvents.SelectCharacter);
        Send.Packet(data);
    }
}
