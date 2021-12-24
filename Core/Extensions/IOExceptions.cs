using System;
using System.IO;

namespace CryBits.Extensions;

public static class IOExceptions
{
    public static void Write(this BinaryWriter buffer, Guid id) => buffer.Write(id.ToString());
}