using CryBits.Definitions.Common;
using CryBits.Transport;
using CryBits.Transport.Packets.Client;
using CryBits.Host.Core;
using CryBits.Host.Network.Senders;
using CryBits.Persistence.Models;
using CryBits.Persistence.Repositories;
using static CryBits.Definitions.Globals;
using BcryptNet = BCrypt.Net.BCrypt;

namespace CryBits.Host.Services;

internal sealed class AuthService(
    AuthSender authSender,
    MapSender mapSender,
    ItemSender itemSender,
    ShopSender shopSender,
    ClassSender classSender,
    NpcSender npcSender,
    AccountSender accountSender,
    AccountRepository accountRepository,
    CharacterRepository characterRepository,
    WorldHost host)
{
    [PacketHandler]
    internal void Connect(Session session, ConnectPacket packet)
    {
        var user = packet.Username.Trim();
        var password = packet.Password;
        var editor = packet.IsClientAccess;

        var record = accountRepository.Find(user);
        if (record == null)
        {
            authSender.Alert(session, "This username isn't registered.");
            return;
        }

        if (host.Sessions.Find(x => x.Account?.Username.Equals(user) == true) != null)
        {
            authSender.Alert(session, "Someone already signed in to this account.");
            return;
        }

        session.Account = new Account
        {
            Username = record.Username,
            PasswordHash = record.PasswordHash,
            AccessLevel = (Access)record.Access
        };

        if (!BcryptNet.Verify(password, session.Account.PasswordHash))
        {
            authSender.Alert(session, "Password is incorrect.");
            return;
        }

        session.Account.AccessLevel = Access.Administrator;

        if (editor)
        {
            if (session.Account.AccessLevel < Access.Editor)
            {
                authSender.Alert(session, "You're not allowed to do this.");
                return;
            }

            session.InEditor = true;
            mapSender.Maps(session);
            itemSender.Items(session);
            shopSender.Shops(session);
            classSender.Classes(session);
            npcSender.Npcs(session);
            authSender.Connect(session);
        }
        else
        {
            session.Account.Characters = characterRepository
                .GetSlots(session.Account.Username)
                .Select(c => new Account.CharacterSlot { Name = c.Name, TextureNum = c.TextureNum })
                .ToList();
            classSender.Classes(session);
            accountSender.Characters(session);

            if (session.Account.Characters.Count == 0)
            {
                accountSender.CreateCharacter(session);
                return;
            }

            authSender.Connect(session);
        }
    }

    [PacketHandler]
    internal void Register(Session session, RegisterPacket packet)
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

        if (accountRepository.Find(user) != null)
        {
            authSender.Alert(session, "There is already someone registered with this name.");
            return;
        }

        session.Account = new Account
        {
            Username = user,
            PasswordHash = BcryptNet.HashPassword(password)
        };

        accountRepository.Save(new AccountModel
        {
            Username = session.Account.Username,
            PasswordHash = session.Account.PasswordHash,
            Access = (byte)session.Account.AccessLevel
        });

        classSender.Classes(session);
        accountSender.CreateCharacter(session);
    }
}
