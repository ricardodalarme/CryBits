using CryBits.Persistence.Models;
using LinqToDB;
using LinqToDB.Data;

namespace CryBits.Persistence;

public static class SchemaBootstrap
{
    public static void EnsureCreated(DataConnection db)
    {
        db.CreateTable<AccountModel>(tableOptions: TableOptions.CheckExistence);
        db.CreateTable<CharacterModel>(tableOptions: TableOptions.CheckExistence);
    }
}
