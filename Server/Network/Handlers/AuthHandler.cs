using System.IO;
using CryBits.Enums;
using CryBits.Server.Entities;
using CryBits.Server.Library;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class AuthHandler
{
    public static void Latency(Account account)
    {
        var data = new NetDataWriter();

        // Envia os dados
        data.Put((byte)ServerPacket.Latency);
        AuthSender.Latency(account);
    }

    internal static void Connect(Account account, NetDataReader data)
    {
        // Lê os dados
        var user = data.GetString().Trim();
        var password = data.GetString();
        var editor = data.GetBool();

        // Verifica se está tudo certo
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

        // Carrega os dados da conta
        Read.Account(account, user);

        // Verifica se a senha está correta
        if (!account.Password.Equals(password))
        {
            AuthSender.Alert(account, "Password is incorrect.");
            return;
        }

        account.Access = Access.Administrator;

        if (editor)
        {
            // Verifica se o jogador tem permissão para fazer entrar no modo edição
            if (account.Access < Access.Editor)
            {
                AuthSender.Alert(account, "You're not allowed to do this.");
                return;
            }

            // Envia todos os dados
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
            // Carrega os dados do jogador
            Read.Characters(account);

            // Envia os dados das classes e dos personagens ao jogador
            ClassSender.Classes(account);
            AccountSender.Characters(account);

            // Se o jogador não tiver nenhum personagem então abrir o painel de criação de personagem
            if (account.Characters.Count == 0)
            {
                AccountSender.CreateCharacter(account);
                return;
            }

            // Abre a janela de seleção de personagens
            AuthSender.Connect(account);
        }
    }

    internal static void Register(Account account, NetDataReader data)
    {
        // Lê os dados
        var user = data.GetString().Trim();
        var password = data.GetString();

        // Verifica se está tudo certo
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

        // Cria a conta
        account.User = user;
        account.Password = password;

        // Salva a conta
        Write.Account(account);

        // Abre a janela de seleção de personagens
        ClassSender.Classes(account);
        AccountSender.CreateCharacter(account);
    }
}