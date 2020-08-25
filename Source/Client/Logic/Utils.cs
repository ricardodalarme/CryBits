using System;
using Entities;
using Logic;
using System.Drawing;
using Interface;

static class Utils
{
    // Números aleatórios
    public static Random Random = new Random();

    // Dimensão das grades 
    public const byte Grid = 32;

    // Medida de calculo do atraso do jogo
    public static short FPS;

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

    // Bloqueio direcional
    public const byte Max_DirBlock = 3;

    // Tamanho da tela
    public const short Screen_Width = Map_Width * Grid;
    public const short Screen_Height = Map_Height * Grid;

    // Limites em geral
    public const byte Max_Inventory = 30;
    public const byte Max_Hotbar = 10;

    // Limitações dos mapas
    public const byte Map_Width = 25;
    public const byte Map_Height = 19;

    // Opções
    public static Options Option = new Options();
    [Serializable]
    public class Options
    {
        public string Game_Name = "CryBits";
        public bool SaveUsername = true;
        public bool Sounds = true;
        public bool Musics = true;
        public bool Chat = true;
        public bool FPS;
        public bool Latency;
        public bool Party;
        public bool Trade;
        public string Username = string.Empty;
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

    // Converte o valor em uma posição adequada à camera
    public static int ConvertX(int x) => x - (Camera.Tile_Sight.X * Grid) - Camera.Start_Sight.X;
    public static int ConvertY(int y) => y - (Camera.Tile_Sight.Y * Grid) - Camera.Start_Sight.Y;

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


    // Obtém o ID de algum dado, caso ele não existir retorna um ID zerado
    public static string GetID(Entity Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();

    public static bool IsAbove(Rectangle Rectangle)
    {
        // Verficia se o Window.Mouse está sobre o objeto
        if (Windows.Mouse.X >= Rectangle.X && Windows.Mouse.X <= Rectangle.X + Rectangle.Width)
            if (Windows.Mouse.Y >= Rectangle.Y && Windows.Mouse.Y <= Rectangle.Y + Rectangle.Height)
                return true;

        // Se não, retornar um valor nulo
        return false;
    }

    public static short MeasureString(string Text)
    {
        // Dados do texto
        SFML.Graphics.Text TempText = new SFML.Graphics.Text(Text, Graphics.Font_Default);
        TempText.CharacterSize = 10;
        return (short)TempText.GetLocalBounds().Width;
    }

    public static string TextBreak(string Text, int Width)
    {
        // Previne sobrecargas
        if (string.IsNullOrEmpty(Text)) return Text;

        // Usado para fazer alguns calculosk
        int Text_Width = MeasureString(Text);

        // Diminui o tamanho do texto até que ele caiba no digitalizador
        while (Text_Width - Width >= 0)
        {
            Text = Text.Substring(1);
            Text_Width = MeasureString(Text);
        }

        return Text;
    }

    public static byte Slot(Panels Panel, byte OffX, byte OffY, byte Lines, byte Columns, byte Grid = 32, byte Gap = 4)
    {
        int Size = Grid + Gap;
        Point Start = Panel.Position + new Size(OffX, OffY);
        Point Slot = new Point((Windows.Mouse.X - Start.X) / Size, (Windows.Mouse.Y - Start.Y) / Size);

        // Verifica se o Window.Mouse está sobre o slot
        if (Slot.Y < 0 || Slot.X < 0 || Slot.X >= Columns || Slot.Y >= Lines) return 0;
        if (!IsAbove(new Rectangle(Start.X + Slot.X * Size, Start.Y + Slot.Y * Size, Grid, Grid))) return 0;
        if (!Panel.Visible) return 0;

        // Retorna o slot
        return (byte)(Slot.Y * Columns + Slot.X + 1);
    }
}