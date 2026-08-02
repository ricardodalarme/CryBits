using LinqToDB.Mapping;

namespace CryBits.Persistence.Models;

[Table("accounts")]
public sealed class AccountModel
{
    [PrimaryKey][NotNull] public string Username { get; set; } = "";
    [Column][NotNull] public string PasswordHash { get; set; } = "";
    [Column][NotNull] public byte Access { get; set; }
}
