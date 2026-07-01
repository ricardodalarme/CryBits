namespace CryBits.Client.UI;

public abstract class ViewBase
{
    private readonly List<Action> _cleanup = [];

    public abstract void Bind();

    public virtual void Unbind()
    {
        foreach (var c in _cleanup)
            c();
        _cleanup.Clear();
    }

    protected void Track(Action subscribe, Action unsubscribe)
    {
        subscribe();
        _cleanup.Add(unsubscribe);
    }
}
