using CryBits.Client.Framework.Audio;
using CryBits.Client.UI;
using CryBits.Client.UI.Menu;
using CryBits.Client.UI.Game;
using CryBits.Client.Worlds;
using CryBits.Definitions.Catalog;
using CryBits.Transport;
using CryBits.Transport.Packets.Server;

namespace CryBits.Client.Network.Handlers;

internal class AccountHandler(AudioManager audioManager, GameContext context, DefinitionCatalog catalog)
{
    private readonly DefinitionCatalog _catalog = catalog;
    [PacketHandler]
    internal void Join(JoinPacket packet)
    {
        context.CurrentMap = null!;
        context.LocalPlayer.Id = packet.PlayerId;
    }

    [PacketHandler]
    internal void CreateCharacter(CreateCharacterPacket _)
    {
        MenuState.CurrentClass = 0;
        MenuState.CurrentTexture = 0;
        MenuEvents.FireCharacterCreateOpened();
    }

    [PacketHandler]
    internal void Characters(CharactersPacket packet)
    {
        MenuState.Characters = new MenuState.TempCharacter[packet.Characters.Length];

        for (byte i = 0; i < MenuState.Characters.Length; i++)
            MenuState.Characters[i] = new MenuState.TempCharacter
            {
                Name = packet.Characters[i].Name,
                TextureNum = packet.Characters[i].TextureNum
            };

        MenuEvents.FireCharactersUpdated();
    }

    [PacketHandler]
    internal void JoinGame(JoinGamePacket _)
    {
        // Reset chat state
        Chat.Order = [];
        Chat.LinesFirst = 0;
        Chat.VisibilityTimer = Environment.TickCount64 + Chat.SleepTimer;

        // Enter the game
        audioManager.StopMusic();
        GameState.CurrentScreen = ScreenType.Game;
        MenuEvents.FireJoinGame();
    }
}
