using System;
using System.IO;
using CryBits.Entities;
using CryBits.Entities.Slots;
using CryBits.Enums;
using CryBits.Extensions;
using CryBits.Server.Entities;
using CryBits.Server.Entities.TempMap;
using CryBits.Server.Library;
using CryBits.Server.Network.Senders;
using LiteNetLib.Utils;
using static CryBits.Globals;

namespace CryBits.Server.Network.Handlers;

internal static class AccountHandler
{
  internal static void CreateCharacter(Account account, NetDataReader data)
  {
    // Lê os dados
    var name = data.GetString().Trim();

    // Verifica se está tudo certo
    if (name.Length < MinNameLength || name.Length > MaxNameLength)
    {
      AccountSender.Alert(account,
        "The character name must contain between " + MinNameLength + " and " + MaxNameLength + " characters.",
        false);
      return;
    }

    if (name.Contains(';') || name.Contains(':'))
    {
      AccountSender.Alert(account, "Can't contain ';' and ':' in the character name.", false);
      return;
    }

    if (Read.CharactersName().Contains(";" + name + ":"))
    {
      AccountSender.Alert(account, "A character with this name already exists", false);
      return;
    }

    // Define os valores iniciais do personagem
    Class @class;
    account.Character = new Player(account);
    account.Character.Name = name;
    account.Character.Level = 1;
    account.Character.Class = @class = Class.List.Get(new Guid(data.GetString()));
    account.Character.Genre = data.GetBool();
    account.Character.TextureNum = account.Character.Genre
      ? @class.TextureMale[data.GetByte()]
      : @class.TextureFemale[data.GetByte()];
    account.Character.Attribute = @class.Attribute;
    account.Character.Map = TempMap.List.Get(@class.SpawnMap.Id);
    account.Character.Direction = (Direction)@class.SpawnDirection;
    account.Character.X = @class.SpawnX;
    account.Character.Y = @class.SpawnY;
    for (byte i = 0; i < (byte)Vital.Count; i++) account.Character.Vital[i] = account.Character.MaxVital(i);
    for (byte i = 0; i < (byte)@class.Item.Count; i++)
      if (@class.Item[i].Item.Type == ItemType.Equipment &&
          account.Character.Equipment[@class.Item[i].Item.EquipType] == null)
        account.Character.Equipment[@class.Item[i].Item.EquipType] = @class.Item[i].Item;
      else
        account.Character.GiveItem(@class.Item[i].Item, @class.Item[i].Amount);
    for (byte i = 0; i < MaxHotbar; i++) account.Character.Hotbar[i] = new HotbarSlot(SlotType.None, 0);

    // Salva a conta
    Write.CharacterName(name);
    Write.Character(account);

    // Entra no jogo
    account.Character.Join();
  }

  internal static void CharacterUse(Account account, NetDataReader data)
  {
    var character = data.GetInt();

    // Verifica se o personagem existe
    if (character < 0 || character >= account.Characters.Count) return;

    // Entra no jogo
    Read.Character(account, account.Characters[character].Name);
    account.Character.Join();
  }

  internal static void CharacterCreate(Account account)
  {
    // Verifica se o jogador já criou o máximo de personagens possíveis
    if (account.Characters.Count == MaxCharacters)
    {
      AccountSender.Alert(account, "You can only have " + MaxCharacters + " characters.", false);
      return;
    }

    // Abre a janela de seleção de personagens
    ClassSender.Classes(account);
    AccountSender.CreateCharacter(account);
  }

  internal static void CharacterDelete(Account account, NetDataReader data)
  {
    var character = data.GetInt();

    // Verifica se o personagem existe
    if (character < 0 || character >= account.Characters.Count) return;

    // Deleta o personagem
    var name = account.Characters[character].Name;
    AccountSender.Alert(account, "The character '" + name + "' has been deleted.", false);
    Write.CharactersName(Read.CharactersName().Replace(":;" + name + ":", ":"));
    account.Characters.RemoveAt(character);
    File.Delete(Path.Combine(Directories.Accounts.FullName, account.User, "Characters", name) + Directories.Format);

    // Salva o personagem
    AccountSender.Characters(account);
    Write.Account(account);
  }
}
