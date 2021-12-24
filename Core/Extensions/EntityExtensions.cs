using System;
using CryBits.Entities;

namespace CryBits.Extensions;

public static class EntityExtensions
{
    // Obtém o ID de alguma entidade, caso ela não existir retorna um ID zerado
    public static Guid GetId(this Entity @object) => @object?.Id ?? Guid.Empty;
}