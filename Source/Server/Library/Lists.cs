using Objects;
using System;
using System.Collections.Generic;

static class Lists
{
    // As listas dos dados
    public static List<Account> Account = new List<Account>();
    public static Dictionary<Guid, Class> Class = new Dictionary<Guid, Class>();
    public static Dictionary<Guid, Item> Item = new Dictionary<Guid, Item>();
    public static Dictionary<Guid, Shop> Shop = new Dictionary<Guid, Shop>();
    public static Dictionary<Guid, NPC> NPC = new Dictionary<Guid, NPC>();
    public static Dictionary<Guid, Map> Map = new Dictionary<Guid, Map>();
    public static Dictionary<Guid, TMap> Temp_Map = new Dictionary<Guid, TMap>();

    // Obtém o ID de algum dado, caso ele não existir retorna um ID zerado
    public static string GetID(Data Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();

    // Obtém o dado, caso ele não existir retorna nulo
    public static object GetData<T>(Dictionary<Guid, T> Dictionary, Guid ID)
    {
        if (Dictionary.ContainsKey(ID)) return Dictionary[ID];
        return null;
    }
}