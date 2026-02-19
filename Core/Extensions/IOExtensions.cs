using System;
using System.IO;

namespace CryBits.Extensions;

public static class IOExtensions
{
    public static void Write(this BinaryWriter buffer, Guid id) => buffer.Write(id.ToString());
}
