using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AccountHandler(Game client, MenuScreen menuScreen)
{
    [PacketHandler]
    internal void Join(JoinPacket packet)
    {
        client.StartSession(packet.PlayerId);
    }

    [PacketHandler]
    internal void CreateCharacter(CreateCharacterPacket _)
    {
        menuScreen.ShowCreateCharacter();
    }

    [PacketHandler]
    internal void Characters(CharactersPacket packet)
    {
        var chars = new SelectCharacterView.TempCharacter[packet.Characters.Length];

        for (byte i = 0; i < chars.Length; i++)
            chars[i] = new SelectCharacterView.TempCharacter
            {
                Name = packet.Characters[i].Name,
                TextureNum = packet.Characters[i].TextureNum
            };

        menuScreen.ShowSelectCharacter(chars);
    }

    [PacketHandler]
    internal void JoinGame(JoinGamePacket _)
    {
        client.OpenSessionScreen();
    }
}
