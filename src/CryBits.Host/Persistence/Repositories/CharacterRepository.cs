using CryBits.Definitions.Characters;
using CryBits.Host.Core;
using CryBits.Persistence.Serialization;
using System.IO;
using System.Text.Json;

namespace CryBits.Host.Persistence.Repositories;

internal sealed class CharacterRepository
{
    public Character? Read(Account account, string name)
    {
        var file = new FileInfo(Path.Combine(Directories.Accounts.FullName, account.Username, "Characters", name) + ".json");
        if (!file.Exists) return null;

        var text = File.ReadAllText(file.FullName);
        return JsonSerializer.Deserialize<Character>(text, JsonConfig.Options);
    }

    public string ReadAllNames()
    {
        if (!Directories.Characters.Exists)
        {
            WriteAllNames(string.Empty);
            return string.Empty;
        }

        using var data = new StreamReader(Directories.Characters.FullName);
        return data.ReadToEnd();
    }

    public void Write(Account account, Character data)
    {
        var file = new FileInfo(
            Path.Combine(Directories.Accounts.FullName, account.Username, "Characters", data.Name) + ".json");
        if (!file.Directory.Exists) file.Directory.Create();

        var json = JsonSerializer.Serialize(data, JsonConfig.Options);
        File.WriteAllText(file.FullName, json);
    }

    public void WriteName(string name)
    {
        using var data = new StreamWriter(Directories.Characters.FullName, true);
        data.Write(";" + name + ":");
    }

    public void WriteAllNames(string charactersName)
    {
        using var data = new StreamWriter(Directories.Characters.FullName);
        data.Write(charactersName);
    }
}
