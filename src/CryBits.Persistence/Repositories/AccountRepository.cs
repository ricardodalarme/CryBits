using CryBits.Persistence.Models;
using LinqToDB;
using LinqToDB.Data;
using System.Linq;

namespace CryBits.Persistence.Repositories;

public sealed class AccountRepository(DataConnection db)
{
    public AccountModel? Find(string username) =>
        db.GetTable<AccountModel>().FirstOrDefault(a => a.Username == username);

    public void Save(AccountModel account) =>
        db.InsertOrReplace(account);
}
