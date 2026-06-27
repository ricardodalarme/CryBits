namespace CryBits.Protocol.Serialization;

public static class ComponentTypeRegistry
{
    private static readonly Dictionary<Type, byte> _typeToTag = [];
    private static readonly Dictionary<byte, Type> _tagToType = [];

    public static void Register<T>(byte tag) where T : class
    {
        _typeToTag[typeof(T)] = tag;
        _tagToType[tag] = typeof(T);
    }

    public static byte? Tag(Type type) => _typeToTag.TryGetValue(type, out var t) ? t : null;
    public static Type Type(byte tag) => _tagToType[tag];
}
