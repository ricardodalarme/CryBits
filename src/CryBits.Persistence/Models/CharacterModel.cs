using LinqToDB.Mapping;

namespace CryBits.Persistence.Models;

[Table("characters")]
public sealed class CharacterModel
{
    [PrimaryKey(1)][NotNull] public string Account { get; set; } = "";
    [PrimaryKey(2)][NotNull] public string Name { get; set; } = "";
    [Column("texture_num")][NotNull] public short TextureNum { get; set; }
    [Column][NotNull] public string Data { get; set; } = "";
    [Column("created_at")][NotNull] public string CreatedAt { get; set; } = "";
    [Column("updated_at")][NotNull] public string UpdatedAt { get; set; } = "";
}
