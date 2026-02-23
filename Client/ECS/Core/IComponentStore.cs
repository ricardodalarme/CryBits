namespace CryBits.Client.ECS;

/// <summary>
/// Internal polymorphic handle used by <see cref="World"/> to remove a component
/// from its typed store without knowing the concrete type at call-site.
/// </summary>
internal interface IComponentStore
{
    void Remove(int entityId);
    void Clear();
}
