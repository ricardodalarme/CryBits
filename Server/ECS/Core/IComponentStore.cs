namespace CryBits.Server.ECS;

/// <summary>
/// Internal polymorphic handle used by <see cref="World"/> to remove a component
/// from its typed store without knowing the concrete type at the call-site.
/// </summary>
internal interface IComponentStore
{
    void Remove(int entityId);
    void Clear();
}
