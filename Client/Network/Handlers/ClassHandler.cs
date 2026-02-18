using System;
using System.Collections.Generic;
using CryBits.Entities;
using CryBits.Extensions;
using LiteNetLib.Utils;


namespace CryBits.Client.Network.Handlers;

internal static class ClassHandler
{
    internal static void Classes(NetDataReader data)
    {
        // Recebe os dados
        Class.List = (Dictionary<Guid, Class>)data.ReadObject();
    }
}