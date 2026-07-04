using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AccountHandler(ClientShell shell, MenuScreen menuScreen)
{
    [PacketHandler]
    internal void Join(JoinPacket packet)
    {
        shell.StartSession(packet.PlayerId);
    }

    [PacketHandler]
    internal void CreateCharacter(CreateCharacterPacket _)
    {
        var createCharacterView = menuScreen.CreateCharacterView;
        createCharacterView.NameTextBox.Value = string.Empty;
        createCharacterView.GenderMaleRadio.Checked = true;
        createCharacterView.GenderFemaleRadio.Checked = false;
        createCharacterView.CurrentClass = 0;
        createCharacterView.CurrentTexture = 0;

        menuScreen.CloseMenus();
        createCharacterView.CreateCharacterPanel.Visible = true;
    }

    [PacketHandler]
    internal void Characters(CharactersPacket packet)
    {
        var selectCharacterView = menuScreen.SelectCharacterView;
        selectCharacterView.Characters = new SelectCharacterView.TempCharacter[packet.Characters.Length];

        for (byte i = 0; i < selectCharacterView.Characters.Length; i++)
        {
            selectCharacterView.Characters[i] = new SelectCharacterView.TempCharacter
            {
                Name = packet.Characters[i].Name,
                TextureNum = packet.Characters[i].TextureNum
            };
        }

        selectCharacterView.UpdateButtonVisibility();
    }

    [PacketHandler]
    internal void JoinGame(JoinGamePacket _)
    {
        shell.OpenSessionScreen();
    }
}
