using CryBits.Client.UI.Game;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Menu.Views;
using CryBits.Client.Worlds;
using CryBits.Protocol;
using CryBits.Protocol.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AccountHandler(GameContext context)
{
    [PacketHandler]
    internal void Join(JoinPacket packet)
    {
        context.CurrentMap = null;
        context.LocalPlayer.Id = packet.PlayerId;
    }

    [PacketHandler]
    internal void CreateCharacter(CreateCharacterPacket _)
    {
        // Reset character-creation inputs
        MenuScreen.Instance.CreateCharacterView.NameTextBox.Value = string.Empty;
        MenuScreen.Instance.CreateCharacterView.GenderMaleRadio.Checked = true;
        MenuScreen.Instance.CreateCharacterView.GenderFemaleRadio.Checked = false;
        CreateCharacterView.CurrentClass = 0;
        CreateCharacterView.CurrentTexture = 0;

        // Show character creation panel
        MenuScreen.Instance.CloseMenus();
        MenuScreen.Instance.CreateCharacterView.CreateCharacterPanel.Visible = true;
    }

    [PacketHandler]
    internal void Characters(CharactersPacket packet)
    {
        // Resize character list
        SelectCharacterView.Characters = new SelectCharacterView.TempCharacter[packet.Characters.Length];

        for (byte i = 0; i < SelectCharacterView.Characters.Length; i++)
        {
            // Read character data
            SelectCharacterView.Characters[i] = new SelectCharacterView.TempCharacter
            {
                Name = packet.Characters[i].Name,
                TextureNum = packet.Characters[i].TextureNum
            };
        }

        SelectCharacterView.UpdateButtonVisibility();
    }

    [PacketHandler]
    internal void JoinGame(JoinGamePacket _)
    {
        GameScreen.Instance.Open();
    }
}
