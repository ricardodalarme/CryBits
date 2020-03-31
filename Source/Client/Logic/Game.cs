using System;
using System.Drawing;
using System.Windows.Forms;

class Game
{
    // Números aleatórios
    public static Random Random = new Random();

    // Dimensão das grades 
    public const byte Grid = 32;

    // Medida de calculo do atraso do jogo
    public static short FPS;

    // Interface
    public static byte CreateCharacter_Class = 1;
    public static byte CreateCharacter_Tex = 0;
    public static int SelectCharacter = 1;
    public static short Infomation_Index;
    public static byte Drop_Slot = 0;
    public static string Party_Invitation;
    public static string Trade_Invitation;
    public static byte Trade_Slot = 0;
    public static byte Trade_Inventory_Slot = 0;
    public static short Shop_Open = 0;
    public static byte Shop_Inventory_Slot = 0;

    // Ataque
    public const short Attack_Speed = 750;

    // Animação
    public const byte Animation_Amount = 4;
    public const byte Animation_Stopped = 1;
    public const byte Animation_Right = 0;
    public const byte Animation_Left = 2;
    public const byte Animation_Attack = 2;

    // Movimentação
    public const byte Movement_Up = 3;
    public const byte Movement_Down = 0;
    public const byte Movement_Left = 1;
    public const byte Movement_Right = 2;

    // Visão do jogador
    private static Rectangle Camera;
    public static Rectangle Tile_Sight;

    // Bloqueio direcional
    public const byte Max_DirBlock = 3;

    // Tamanho da tela
    public const short Screen_Width = (Map.Min_Width + 1) * Grid;
    public const short Screen_Height = (Map.Min_Height + 1) * Grid;

    // Limites em geral
    public const byte Max_Inventory = 30;
    public const byte Max_Hotbar = 10;

    #region Enums
    public enum Situations
    {
        Connect,
        Registrar,
        SelectCharacter,
        CreateCharacter
    }

    public enum Attributes
    {
        Strength,
        Resistance,
        Intelligence,
        Agility,
        Vitality,
        Count
    }

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        Count
    }

    public enum Movements
    {
        Stopped,
        Walking,
        Moving
    }

    public enum Messages
    {
        Game,
        Map,
        Global,
        Private
    }

    public enum Vitals
    {
        HP,
        MP,
        Count
    }

    public enum NPCs
    {
        Friendly,
        AttackOnSight,
        AttackWhenAttacked,
        ShopKeeper
    }

    public enum Target
    {
        Player = 1,
        NPC
    }

    public enum Items
    {
        None,
        Equipment,
        Potion
    }

    public enum Equipments
    {
        Weapon,
        Armor,
        Helmet,
        Shield,
        Amulet,
        Count
    }

    public enum Hotbar
    {
        None,
        Item
    }

    public enum Rarity
    {
        Common,
        Uncommon,
        Rare,
        Epic,
        Legendary,
        Count
    }

    public enum BindOn
    {
        None,
        Pickup,
        Equip,
        Count
    }

    public enum Trade_Status
    {
        Waiting,
        Confirmed,
        Accepted,
        Declined
    }
    #endregion

    public static void OpenMenu()
    {
        // Reproduz a música de fundo
        if (Lists.Options.Musics)
            Audio.Music.Play(Audio.Musics.Menu);

        // Reseta os dados
        CheckBoxes.Get("Connect_Save_Username").Checked = Lists.Options.SaveUsername;
        if (Lists.Options.SaveUsername) TextBoxes.Get("Connect_Username").Text = Lists.Options.Username;

        // Abre o menu
        Tools.CurrentWindow = Tools.Windows.Menu;
    }

    public static void Leave()
    {
        // Volta ao menu
        OpenMenu();

        // Termina a conexão
        Socket.Disconnect();
    }

    public static void SetSituation(Situations Situation)
    {
        // Verifica se é possível se conectar ao servidor
        if (!Socket.TryConnect())
        {
            MessageBox.Show("The server is currently unavailable.");
            return;
        }

        // Envia os dados
        switch (Situation)
        {
            case Situations.Connect: Send.Connect(); break;
            case Situations.Registrar: Send.Register(); break;
            case Situations.CreateCharacter: Send.CreateCharacter(); break;
        }
    }

    public static void Disconnect()
    {
        // Não fechar os paineis se não for necessário
        if (Panels.Get("Options").Visible || Panels.Get("Connect").Visible || Panels.Get("Register").Visible) return;

        // Limpa os valores
        Audio.Sound.Stop_All();
        Player.MyIndex = 0;

        // Traz o jogador de volta ao menu
        Panels.Menu_Close();
        Panels.Get("Connect").Visible = true;
        Tools.CurrentWindow = Tools.Windows.Menu;
    }

    public static void UpdateCamera()
    {
        Point End = new Point(), Start = new Point(), Position = new Point();

        // Centro da tela
        Position.X = Player.Me.X2 + Grid;
        Position.Y = Player.Me.Y2 + Grid;

        // Início da tela
        Start.X = Player.Me.X - ((Map.Min_Width + 1) / 2) - 1;
        Start.Y = Player.Me.Y - ((Map.Min_Height + 1) / 2) - 1;

        // Reajusta a posição horizontal da tela
        if (Start.X < 0)
        {
            Position.X = 0;
            if (Start.X == -1 && Player.Me.X2 > 0) Position.X = Player.Me.X2;
            Start.X = 0;
        }

        // Reajusta a posição vertical da tela
        if (Start.Y < 0)
        {
            Position.Y = 0;
            if (Start.Y == -1 && Player.Me.Y2 > 0) Position.Y = Player.Me.Y2;
            Start.Y = 0;
        }

        // Final da tela
        End.X = Start.X + (Map.Min_Width + 1) + 1;
        End.Y = Start.Y + (Map.Min_Height + 1) + 1;

        // Reajusta a posição horizontal da tela
        if (End.X > Lists.Map.Width)
        {
            Position.X = Grid;
            if (End.X == Lists.Map.Width + 1 && Player.Me.X2 < 0) Position.X = Player.Me.X2 + Grid;
            End.X = Lists.Map.Width;
            Start.X = End.X - Map.Min_Width - 1;
        }

        // Reajusta a posição vertical da tela
        if (End.Y > Lists.Map.Height)
        {
            Position.Y = Grid;
            if (End.Y == Lists.Map.Height + 1 && Player.Me.Y2 < 0) Position.Y = Player.Me.Y2 + Grid;
            End.Y = Lists.Map.Height;
            Start.Y = End.Y - Map.Min_Height - 1;
        }

        // Define a dimensão dos azulejos vistos
        Tile_Sight.Y = Start.Y;
        Tile_Sight.Height = End.Y;
        Tile_Sight.X = Start.X;
        Tile_Sight.Width = End.X;

        // Define a posição da câmera
        Camera.Y = Position.Y;
        Camera.Height = Camera.Y + Screen_Height;
        Camera.X = Position.X;
        Camera.Width = Camera.X + Screen_Width;
    }

    // Converte o valor em uma posição adequada à camera
    public static int ConvertX(int x) => x - (Tile_Sight.X * Grid) - Camera.X;
    public static int ConvertY(int y) => y - (Tile_Sight.Y * Grid) - Camera.Y;

    public static Directions ReverseDirection(Directions Direction)
    {
        // Retorna a direção inversa
        switch (Direction)
        {
            case Directions.Up: return Directions.Down;
            case Directions.Down: return Directions.Up;
            case Directions.Left: return Directions.Right;
            case Directions.Right: return Directions.Left;
            default: return Directions.Count;
        }
    }

    public static short Find_Shop_Bought(short Item_Num)
    {
        for (byte i = 0; i < Lists.Shop[Shop_Open].Bought.Length; i++)
            if (Lists.Shop[Shop_Open].Bought[i].Item_Num == Item_Num)
                return i;

        return -1;
    }
}