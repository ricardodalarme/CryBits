namespace CryBits.Protocol.Serialization;

public static class IntentRegistry
{
    private static readonly Dictionary<byte, Type> TagToType = [];
    private static readonly Dictionary<Type, byte> TypeToTag = [];

    public static void Register<T>(byte tag)
    {
        var type = typeof(T);
        TypeToTag[type] = tag;
        TagToType[tag] = type;
    }

    public static Type? GetTypeForTag(byte tag) =>
        TagToType.TryGetValue(tag, out var type) ? type : null;

    public static byte? GetTag(Type type) =>
        TypeToTag.TryGetValue(type, out var tag) ? tag : null;
}
