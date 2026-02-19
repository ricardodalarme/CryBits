using System.IO;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Library;
using CryBits.Server.Library.Repositories;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Systems;

/// <summary>Owns account authentication and registration.</summary>
internal static class AuthSystem
{
    /// <summary>
    /// Authenticates <paramref name="account"/> with the credentials read from <paramref name="data"/>.
    /// On success, sends the editor or character-selection payload and opens the appropriate screen.
    /// </summary>
    internal static void Connect(Account account, NetDataReader data)
    {
        var user = data.GetString().Trim();
        var password = data.GetString();
        var editor = data.GetBool();

        if (!Directory.Exists(Path.Combine(Directories.Accounts.FullName, user)))
        {
            AuthSender.Alert(account, "This username isn't registered.");
            return;
        }

        if (Account.List.Find(x => x.User.Equals(user)) != null)
        {
            AuthSender.Alert(account, "Someone already signed in to this account.");
            return;
        }

        AccountRepository.Read(account, user);

        if (!account.Password.Equals(password))
        {
            AuthSender.Alert(account, "Password is incorrect.");
            return;
        }

        account.Access = Access.Administrator;

        if (editor)
        {
            if (account.Access < Access.Editor)
            {
                AuthSender.Alert(account, "You're not allowed to do this.");
                return;
            }

            account.InEditor = true;
            SettingsSender.ServerData(account);
            MapSender.Maps(account);
            ItemSender.Items(account);
            ShopSender.Shops(account);
            ClassSender.Classes(account);
            NpcSender.Npcs(account);
            AuthSender.Connect(account);
        }
        else
        {
            AccountRepository.ReadCharacters(account);
            ClassSender.Classes(account);
            AccountSender.Characters(account);

            if (account.Characters.Count == 0)
            {
                AccountSender.CreateCharacter(account);
                return;
            }

            AuthSender.Connect(account);
        }
    }

    /// <summary>
    /// Registers a new account using the credentials read from <paramref name="data"/>,
    /// saves it to disk, and opens the character-creation screen.
    /// </summary>
    internal static void Register(Account account, NetDataReader data)
    {
        var user = data.GetString().Trim();
        var password = data.GetString();

        if (user.Length < MinNameLength || user.Length > MaxNameLength)
        {
            AuthSender.Alert(account,
                "The username must contain between " + MinNameLength + " and " + MaxNameLength + " characters.");
            return;
        }

        if (password.Length < MinNameLength || password.Length > MaxNameLength)
        {
            AuthSender.Alert(account,
                "The password must contain between " + MinNameLength + " and " + MaxNameLength + " characters.");
            return;
        }

        if (File.Exists(Path.Combine(Directories.Accounts.FullName, user) + Directories.Format))
        {
            AuthSender.Alert(account, "There is already someone registered with this name.");
            return;
        }

        account.User = user;
        account.Password = password;

        AccountRepository.Write(account);

        ClassSender.Classes(account);
        AccountSender.CreateCharacter(account);
    }
}
