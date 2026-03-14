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
            authSender.Alert(session, "This username isn't registered.");
            return;
        }

        if (GameWorld.Current.Sessions.Find(x => x.Username.Equals(user)) != null)
        {
            authSender.Alert(session, "Someone already signed in to this account.");
            return;
        }

        accountRepository.Read(session, user);

        if (!BcryptNet.Verify(password, session.PasswordHash))
        {
            authSender.Alert(session, "Password is incorrect.");
            return;
        }

        session.AccessLevel = Access.Administrator;

        if (editor)
        {
            if (session.AccessLevel < Access.Editor)
            {
                authSender.Alert(session, "You're not allowed to do this.");
                return;
            }

            session.InEditor = true;
            settingsSender.ServerData(session);
            mapSender.Maps(session);
            itemSender.Items(session);
            shopSender.Shops(session);
            classSender.Classes(session);
            npcSender.Npcs(session);
            authSender.Connect(session);
        }
        else
        {
            accountRepository.ReadCharacters(session);
            classSender.Classes(session);
            accountSender.Characters(session);

            if (session.Characters.Count == 0)
            {
                accountSender.CreateCharacter(session);
                return;
            }

            authSender.Connect(session);
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
            authSender.Alert(session,
                "The username must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength + " characters.");
            return;
        }

        if (password.Length < Config.MinPasswordLength || password.Length > Config.MaxPasswordLength)
        {
            authSender.Alert(session,
                "The password must contain between " + Config.MinPasswordLength + " and " + Config.MaxPasswordLength + " characters.");
            return;
        }

        if (File.Exists(Path.Combine(Directories.Accounts.FullName, user) + Directories.Format))
        {
            authSender.Alert(session, "There is already someone registered with this name.");
            return;
        }

        session.Username = user;
        session.PasswordHash = BcryptNet.HashPassword(password);

        accountRepository.Write(session);

        classSender.Classes(session);
        accountSender.CreateCharacter(session);
    }
}
