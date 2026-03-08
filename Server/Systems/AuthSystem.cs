using System.IO;
using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;
using static CryBits.Globals;
using BcryptNet = BCrypt.Net.BCrypt;

namespace CryBits.Server.Systems;

/// <summary>Owns account authentication and registration.</summary>
internal sealed class AuthSystem(
    AuthSender authSender,
    MapSender mapSender,
    ItemSender itemSender,
    ShopSender shopSender,
    ClassSender classSender,
    NpcSender npcSender,
    AccountSender accountSender,
    SettingsSender settingsSender,
    AccountRepository accountRepository)
{
    public static AuthSystem Instance { get; } = new(
        AuthSender.Instance,
        MapSender.Instance,
        ItemSender.Instance,
        ShopSender.Instance,
        ClassSender.Instance,
        NpcSender.Instance,
        AccountSender.Instance,
        SettingsSender.Instance,
        AccountRepository.Instance);

    private readonly AuthSender _authSender = authSender;
    private readonly MapSender _mapSender = mapSender;
    private readonly ItemSender _itemSender = itemSender;
    private readonly ShopSender _shopSender = shopSender;
    private readonly ClassSender _classSender = classSender;
    private readonly NpcSender _npcSender = npcSender;
    private readonly AccountSender _accountSender = accountSender;
    private readonly SettingsSender _settingsSender = settingsSender;
    private readonly AccountRepository _accountRepository = accountRepository;

    /// <summary>
    /// Authenticates <paramref name="session"/> with the credentials read from <paramref name="packet"/>.
    /// On success, sends the editor or character-selection payload and opens the appropriate screen.
    /// </summary>
    internal void Connect(GameSession session, ConnectPacket packet)
    {
        var user = packet.Username.Trim();
        var password = packet.Password;
        var editor = packet.IsClientAccess;

        if (!Directory.Exists(Path.Combine(Directories.Accounts.FullName, user)))
        {
            _authSender.Alert(session, "This username isn't registered.");
            return;
        }

        if (GameWorld.Current.Sessions.Find(x => x.Username.Equals(user)) != null)
        {
            _authSender.Alert(session, "Someone already signed in to this account.");
            return;
        }

        _accountRepository.Read(session, user);

        if (!BcryptNet.Verify(password, session.PasswordHash))
        {
            _authSender.Alert(session, "Password is incorrect.");
            return;
        }

        session.AccessLevel = Access.Administrator;

        if (editor)
        {
            if (session.AccessLevel < Access.Editor)
            {
                _authSender.Alert(session, "You're not allowed to do this.");
                return;
            }

            session.InEditor = true;
            _settingsSender.ServerData(session);
            _mapSender.Maps(session);
            _itemSender.Items(session);
            _shopSender.Shops(session);
            _classSender.Classes(session);
            _npcSender.Npcs(session);
            _authSender.Connect(session);
        }
        else
        {
            _accountRepository.ReadCharacters(session);
            _classSender.Classes(session);
            _accountSender.Characters(session);

            if (session.Characters.Count == 0)
            {
                _accountSender.CreateCharacter(session);
                return;
            }

            _authSender.Connect(session);
        }
    }

    /// <summary>
    /// Registers a new account using the credentials read from <paramref name="packet"/>,
    /// saves it to disk, and opens the character-creation screen.
    /// </summary>
    internal void Register(GameSession session, RegisterPacket packet)
    {
        var user = packet.Username.Trim();
        var password = packet.Password;

        if (user.Length < Config.MinNameLength || user.Length > Config.MaxNameLength)
        {
            _authSender.Alert(session,
                "The username must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength + " characters.");
            return;
        }

        if (password.Length < Config.MinPasswordLength || password.Length > Config.MaxPasswordLength)
        {
            _authSender.Alert(session,
                "The password must contain between " + Config.MinPasswordLength + " and " + Config.MaxPasswordLength + " characters.");
            return;
        }

        if (File.Exists(Path.Combine(Directories.Accounts.FullName, user) + Directories.Format))
        {
            _authSender.Alert(session, "There is already someone registered with this name.");
            return;
        }

        session.Username = user;
        session.PasswordHash = BcryptNet.HashPassword(password);

        _accountRepository.Write(session);

        _classSender.Classes(session);
        _accountSender.CreateCharacter(session);
    }
}
