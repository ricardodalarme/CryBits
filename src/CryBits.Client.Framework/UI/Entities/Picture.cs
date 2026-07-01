using Iguina;
using Iguina.Entities;

namespace CryBits.Client.Framework.UI.Entities;

public class Picture(UISystem system) : Entity(system, null)
{
    public event Action? OnRenderPicture;

    public void Render()
    {
        if (!IsCurrentlyVisible()) return;
        OnRenderPicture?.Invoke();
    }
}
