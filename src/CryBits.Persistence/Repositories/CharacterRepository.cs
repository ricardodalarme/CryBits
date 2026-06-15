using CryBits.Definitions.Characters;
using CryBits.Persistence.Models;
using CryBits.Persistence.Serialization;
using LinqToDB;
using LinqToDB.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace CryBits.Persistence.Repositories;

public sealed class CharacterRepository(DataConnection db)
{
    public Character? Find(string account, string name)
    {
        var record = db.GetTable<CharacterModel>()
            .FirstOrDefault(c => c.Account == account && c.Name == name);
        return record == null ? null :
            JsonSerializer.Deserialize<Character>(record.Data, JsonConfig.Options);
    }

    public List<CharacterModel> GetSlots(string account) =>
        db.GetTable<CharacterModel>()
            .Where(c => c.Account == account)
            .ToList();

    public bool NameExists(string name) =>
        db.GetTable<CharacterModel>().Any(c => c.Name == name);

    public void Save(string account, Character data)
    {
        var now = DateTime.UtcNow.ToString("o");
        var existing = db.GetTable<CharacterModel>()
            .FirstOrDefault(c => c.Account == account && c.Name == data.Name);

        db.InsertOrReplace(new CharacterModel
        {
            Account = account,
            Name = data.Name,
            TextureNum = data.TextureNum,
            Data = JsonSerializer.Serialize(data, JsonConfig.Options),
            CreatedAt = existing?.CreatedAt ?? now,
            UpdatedAt = now
        });
    }

    public void Delete(string account, string name) =>
        db.GetTable<CharacterModel>()
            .Where(c => c.Account == account && c.Name == name)
            .Delete();
}
