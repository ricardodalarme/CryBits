using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Managers;
using Iguina;
using Iguina.Drivers.Sfml;

namespace CryBits.Client.Iguina;

internal sealed class IguinaUiRoot
{
    public UISystem System { get; }

    public IguinaUiRoot(Graphics.Renderer renderer, InputManager inputManager)
    {
        var themePath = Directories.IguinaTheme.FullName;
        var sfmlRenderer = new SfmlRenderer(renderer.RenderWindow, themePath, Fonts.Default);
        var sfmlInput = new SfmlInputProvider(renderer.RenderWindow);

        System = new UISystem(
            Path.Combine(themePath, "system_style.json"),
            sfmlRenderer,
            sfmlInput);
    }

    public void Update(float deltaTime) => System.Update(deltaTime);
    public void Draw() => System.Draw();
}
