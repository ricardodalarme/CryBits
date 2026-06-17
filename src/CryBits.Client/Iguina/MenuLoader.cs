using Iguina;
using Iguina.Defs;
using Iguina.Entities;
using System.Text.Json;
using Ent = Iguina.Entities.Entity;

namespace CryBits.Client.Iguina;

public static class MenuLoader
{
    public static MenuConfig Load(string path)
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<MenuConfig>(json) ?? new MenuConfig();
    }

    public static Panel BuildRoot(UISystem ui, MenuConfig config)
    {
        var root = new Panel(ui);
        root.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = config.Background.Texture,
            SourceRect = new Rectangle { Width = config.Background.Width, Height = config.Background.Height }
        };
        root.Size.SetPixels(config.Background.Width, config.Background.Height);
        root.Anchor = Anchor.TopLeft;
        root.Offset.SetPixels(0, 0);
        ui.Root.AddChild(root);

        return root;
    }

    public static Button BuildOptionsButton(UISystem ui, MenuConfig config)
    {
        var el = config.OptionsButton;
        var btn = new Button(ui);
        btn.OverrideStyles.Icon = new IconTexture
        {
            TextureId = el.Texture,
            SourceRect = new Rectangle { Width = el.Width, Height = el.Height },
            TextureScale = 1
        };
        btn.Size.SetPixels(el.Width, el.Height);
        btn.Anchor = Anchor.TopLeft;
        btn.Offset.SetPixels(el.X, el.Y);
        btn.Paragraph.Text = string.Empty;
        return btn;
    }

    public static (Panel panel, Dictionary<string, Ent> registry) BuildScreen(
        UISystem ui, ScreenData screen, Panel root)
    {
        var registry = new Dictionary<string, Ent>();

        var panel = new Panel(ui);
        panel.OverrideStyles.FillTextureStretched = new StretchedTexture
        {
            TextureId = screen.Panel.Texture,
            SourceRect = new Rectangle { Width = screen.Panel.Width, Height = screen.Panel.Height }
        };
        panel.Size.SetPixels(screen.Panel.Width, screen.Panel.Height);
        panel.Anchor = Anchor.TopLeft;
        panel.Offset.SetPixels(screen.Panel.X, screen.Panel.Y);
        root.AddChild(panel);

        foreach (var el in screen.Elements)
        {
            var entity = BuildElement(ui, el, screen.Panel.X, screen.Panel.Y, panel);
            if (entity != null)
                registry[el.Name] = entity;
        }

        return (panel, registry);
    }

    private static Ent? BuildElement(UISystem ui, Element el, int panelX, int panelY, Panel parent)
    {
        Ent? entity = null;
        var rx = el.X - panelX;
        var ry = el.Y - panelY;

        switch (el.Type)
        {
            case "Button":
                var btn = new Button(ui);
                btn.OverrideStyles.Icon = new IconTexture
                {
                    TextureId = el.Texture,
                    SourceRect = new Rectangle { Width = el.Width, Height = el.Height },
                    TextureScale = 1
                };
                btn.Size.SetPixels(el.Width, el.Height);
                btn.Paragraph.Text = string.Empty;
                entity = btn;
                break;

            case "TextInput":
                var input = new TextInput(ui);
                input.Size.SetPixels(el.Width, el.Height);
                if (el.Masked)
                    input.MaskingCharacter = '*';
                entity = input;
                break;

            case "Checkbox":
                var cb = new Checkbox(ui);
                cb.Paragraph.Text = el.Text;
                cb.Checked = el.Checked;
                cb.Paragraph.OverrideStyles = new StyleSheetState
                {
                    Padding = new Sides { Left = 18 }
                };
                entity = cb;
                break;

            case "Label":
                var label = new Label(ui) { Text = el.Text };
                if (el.MaxWidth > 0)
                {
                    label.Size.SetPixels(el.MaxWidth, 20);
                    label.TextOverflowMode = TextOverflowMode.WrapWords;
                }
                entity = label;
                break;

            case "Panel":
                var placeholder = new Panel(ui);
                placeholder.OverrideStyles.FillTextureStretched = new StretchedTexture
                {
                    TextureId = "Textures/TextBox.png",
                    SourceRect = new Rectangle { Width = 1, Height = 1 }
                };
                placeholder.Size.SetPixels(el.Width, el.Height);
                entity = placeholder;
                break;

            case "ProgressBar":
                var bar = new ProgressBar(ui);
                bar.Size.SetPixels(el.Width, el.Height);
                bar.MaxValue = 100;
                entity = bar;
                break;

            case "SlotGrid":
                // Create container panel — the view handles the IguinaSlotGrid creation
                var gridContainer = new Panel(ui);
                gridContainer.Size.SetPixels(el.Width > 0 ? el.Width : 32 * el.Columns + el.SlotPadding * (el.Columns - 1),
                    el.Height > 0 ? el.Height : 32 * el.Rows + el.SlotPadding * (el.Rows - 1));
                gridContainer.OverrideStyles.FillTextureStretched = new StretchedTexture
                {
                    TextureId = "Textures/TextBox.png",
                    SourceRect = new Rectangle { Width = 1, Height = 1 }
                };
                entity = gridContainer;
                break;
        }

        if (entity != null)
        {
            entity.Anchor = Anchor.TopLeft;
            entity.Offset.SetPixels(rx, ry);
            entity.Visible = el.Visible;
            parent.AddChild((Ent)entity);

            // Build children recursively
            if (el.Children.Count > 0 && entity is Panel entityPanel)
            {
                foreach (var child in el.Children)
                    BuildElement(ui, child, el.X, el.Y, entityPanel);
            }
        }

        return entity;
    }
}
