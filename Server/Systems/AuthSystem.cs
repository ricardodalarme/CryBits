using System.IO;
using CryBits.Enums;
using CryBits.Packets.Client;
using CryBits.Server.Entities;
using CryBits.Server.Network.Senders;
using CryBits.Server.Persistence;
using CryBits.Server.Persistence.Repositories;
using CryBits.Server.World;
using static CryBits.Globals;
using BcryptNet = BCrypt.Net.BCrypt;

namespace CryBits.Server.Systems;

/// <summary>Owns account authentication and registration.</summary>
internal static class AuthSystem
{
    /// <summary>
    /// Authenticates <paramref name="account"/> with the credentials read from <paramref name="data"/>.
    /// On success, sends the editor or character-selection payload and opens the appropriate screen.
    /// </summary>
    internal static void Connect(GameSession session, ConnectPacket packet)
    {
        var user = packet.Username.Trim();
        var password = packet.Password;
        var editor = packet.IsClientAccess;

        if (!Directory.Exists(Path.Combine(Directories.Accounts.FullName, user)))
        {
            AuthSender.Alert(session, "This username isn't registered.");
            return;
        }

        if (GameWorld.Current.Sessions.Find(x => x.Username.Equals(user)) != null)
        {
            AuthSender.Alert(session, "Someone already signed in to this account.");
            return;
        }

        AccountRepository.Read(session, user);

        if (!BcryptNet.Verify(password, session.PasswordHash))
        {
            AuthSender.Alert(session, "Password is incorrect.");
            return;
        }

        session.AccessLevel = Access.Administrator;

        if (editor)
        {
            if (session.AccessLevel < Access.Editor)
            {
                AuthSender.Alert(session, "You're not allowed to do this.");
                return;
            }

            session.InEditor = true;
            SettingsSender.ServerData(session);
            MapSender.Maps(session);
            ItemSender.Items(session);
            ShopSender.Shops(session);
            ClassSender.Classes(session);
            NpcSender.Npcs(session);
            AuthSender.Connect(session);
        }
        else
        {
            AccountRepository.ReadCharacters(session);
            ClassSender.Classes(session);
            AccountSender.Characters(session);

            if (session.Characters.Count == 0)
            {
                AccountSender.CreateCharacter(session);
                return;
            }

            AuthSender.Connect(session);
        }
    }

    /// <summary>
    /// Registers a new account using the credentials read from <paramref name="data"/>,
    /// saves it to disk, and opens the character-creation screen.
    /// </summary>
    internal static void Register(GameSession session, RegisterPacket packet)
    {
        var user = packet.Username.Trim();
        var password = packet.Password;

        if (user.Length < Config.MinNameLength || user.Length > Config.MaxNameLength)
        {
            AuthSender.Alert(session,
                "The username must contain between " + Config.MinNameLength + " and " + Config.MaxNameLength + " characters.");
            return;
        }

        if (password.Length < Config.MinPasswordLength || password.Length > Config.MaxPasswordLength)
        {
            AuthSender.Alert(session,
                "The password must contain between " + Config.MinPasswordLength + " and " + Config.MaxPasswordLength + " characters.");
            return;
        }

        if (File.Exists(Path.Combine(Directories.Accounts.FullName, user) + Directories.Format))
        {
            AuthSender.Alert(session, "There is already someone registered with this name.");
            return;
        }

        session.Username = user;
        session.PasswordHash = BcryptNet.HashPassword(password);

        AccountRepository.Write(session);

        ClassSender.Classes(session);
        AccountSender.CreateCharacter(session);
    }
}
