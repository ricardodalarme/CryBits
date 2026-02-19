using System;
using CryBits.Entities;

namespace CryBits.Extensions;

public static class EntityExtensions
{
    /// <summary>Return the entity's Id or <see cref="Guid.Empty"/> when the instance is null.</summary>
    public static Guid GetId(this Entity @object) => @object?.Id ?? Guid.Empty;
}
