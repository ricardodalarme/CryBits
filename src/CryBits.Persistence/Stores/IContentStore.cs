using CryBits.Definitions.Common;
using System;
using System.Collections.Generic;

namespace CryBits.Persistence.Stores;

public interface IContentStore
{
    T? Load<T>(Guid id) where T : Entity;
    IEnumerable<T> LoadAll<T>() where T : Entity;
    void Save<T>(T entity) where T : Entity;
    void SaveAll<T>(IEnumerable<T> entities) where T : Entity;
}
