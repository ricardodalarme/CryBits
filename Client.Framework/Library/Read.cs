using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using CryBits.Client.Framework.Constants;
using CryBits.Client.Framework.Entities.Tile;
using CryBits.Client.Framework.Graphics;
using CryBits.Client.Framework.Interfacily.Components;
using CryBits.Client.Framework.Interfacily.Enums;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Library;

public static class Read
{
    // ─── Interface (Tools) ──────────────────────────────────────────────────

    public static void Tools()
    {
        var file = new FileInfo(Directories.ToolsData.FullName);
        if (!file.Exists) return;

        // Lê todas as ferramentas a partir de JSON
        var root = JsonSerializer.Deserialize<ToolsJsonRoot>(file.OpenRead(), JsonConfig.Options)
                   ?? new ToolsJsonRoot();

        foreach (var screenDto in root.Screens)
        {
            var screen = new Screen { Name = screenDto.Name };
            Screens.List.Add(screen.Name, screen);
            LoadChildren(null, screen.Body, screenDto.Children);
        }
    }

    private static void LoadChildren(Component? parent, List<Component> node, List<ComponentDto> dtos)
    {
        foreach (var dto in dtos)
        {
            Component component = dto switch
            {
                ButtonDto b => new Button
                {
                    Name = b.Name,
                    Position = new Point(b.X, b.Y),
                    Visible = b.Visible,
                    TextureNum = b.TextureNum
                },
                TextBoxDto tb => new TextBox
                {
                    Name = tb.Name,
                    Position = new Point(tb.X, tb.Y),
                    Visible = tb.Visible,
                    MaxCharacters = tb.MaxCharacters,
                    Width = tb.Width,
                    Password = tb.Password
                },
                PanelDto p => new Panel
                {
                    Name = p.Name,
                    Position = new Point(p.X, p.Y),
                    Visible = p.Visible,
                    TextureNum = p.TextureNum
                },
                CheckBoxDto cb => new CheckBox
                {
                    Name = cb.Name,
                    Position = new Point(cb.X, cb.Y),
                    Visible = cb.Visible,
                    Text = cb.Text,
                    Checked = cb.Checked
                },
                _ => throw new InvalidOperationException($"Unknown component DTO type: {dto.GetType().Name}")
            };

            switch (component)
            {
                case Button btn: Buttons.List.TryAdd(btn.Name, btn); break;
                case TextBox tb: TextBoxes.List.TryAdd(tb.Name, tb); break;
                case Panel pnl: Panels.List.TryAdd(pnl.Name, pnl); break;
                case CheckBox chk: CheckBoxes.List.TryAdd(chk.Name, chk); break;
            }

            component.Parent = parent;
            node.Add(component);

            // Recursion into children
            LoadChildren(component, component.Children, dto.Children);
        }
    }

    // ─── Options ────────────────────────────────────────────────────────────

    public static void Options()
    {
        // Cria o arquivo se ele não existir
        if (!Directories.Options.Exists)
        {
            Write.Options();
            return;
        }

        // Carrega as configurações a partir de JSON
        using var stream = Directories.Options.OpenRead();
        var opts = JsonSerializer.Deserialize<OptionsDto>(stream, JsonConfig.Options) ?? new OptionsDto();
        Framework.Options.SaveUsername = opts.SaveUsername;
        Framework.Options.Username = opts.Username;
        Framework.Options.Sounds = opts.Sounds;
        Framework.Options.Musics = opts.Musics;
        Framework.Options.Chat = opts.Chat;
        Framework.Options.Fps = opts.Fps;
        Framework.Options.Latency = opts.Latency;
        Framework.Options.Party = opts.Party;
        Framework.Options.Trade = opts.Trade;
        Framework.Options.PreMapGrid = opts.PreMapGrid;
        Framework.Options.PreMapView = opts.PreMapView;
        Framework.Options.PreMapAudio = opts.PreMapAudio;
    }

    // ─── Tiles (keep BinaryFormatter – tile data is internal build output) ──

    public static void Tiles()
    {
        // Lê os dados
        Entities.Tile.Tile.List = new Tile[Textures.Tiles.Count];
        for (byte i = 1; i < Entities.Tile.Tile.List.Length; i++) Tile(i);
    }

    private static void Tile(byte index)
    {
        var file = new FileInfo(Path.Combine(Directories.Tiles.FullName, index.ToString()) + Directories.Format);

        // Evita erros
        if (!file.Exists)
        {
            var textureSize = Textures.Tiles[index].ToSize();
            Entities.Tile.Tile.List[index] = new Tile(textureSize);
            Write.Tile(index);
            return;
        }

        // Lê os dados
        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        Entities.Tile.Tile.List[index] = (Tile)new BinaryFormatter().Deserialize(stream);
#pragma warning restore SYSLIB0011
    }

    // ─── Maps (keep BinaryFormatter – maps are sent over the network) ───────

    public static void Map(Guid id)
    {
        var file = new FileInfo(Path.Combine(Directories.MapsData.FullName, id.ToString()) + Directories.Format);

        // Lê os dados
        using var stream = file.OpenRead();
#pragma warning disable SYSLIB0011
        CryBits.Entities.Map.Map.List.Add(id, (Map)new BinaryFormatter().Deserialize(stream));
#pragma warning restore SYSLIB0011
    }
}
