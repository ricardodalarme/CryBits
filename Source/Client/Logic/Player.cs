using Lidgren.Network;
using SFML.Graphics;
using System;
using System.Drawing;

public class Player
{
    // O maior índice dos jogadores conectados
    public static byte HigherIndex;

    // Inventário
    public static Lists.Structures.Inventory[] Inventory = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
    public static byte Inventory_Change;

    // Hotbar
    public static Lists.Structures.Hotbar[] Hotbar = new Lists.Structures.Hotbar[Game.Max_Hotbar + 1];
    public static byte Hotbar_Change;

    // O próprio jogador
    public static byte MyIndex;
    public static Lists.Structures.Player Me
    {
        get
        {
            return Lists.Player[MyIndex];
        }
        set
        {
            Lists.Player[MyIndex] = value;
        }
    }

    public static bool IsPlaying(byte Index)
    {
        // Verifica se o jogador está dentro do jogo
        if (MyIndex > 0 && !string.IsNullOrEmpty(Lists.Player[Index].Name))
            return true;
        else
            return false;
    }

    public static short Player_Texture(byte Index)
    {
        byte Class = Lists.Player[Index].Class;

        // Retorna com o valor da textura
        if (Lists.Player[Index].Genre)
            return Lists.Class[Class].Texture_Male;
        else
            return Lists.Class[Class].Texture_Female;
    }

    public static void Logic()
    {
        // Verificações
        CheckMovement();
        CheckAttack();

        // Lógica dos jogadores
        for (byte i = 1; i <= Player.HigherIndex; i++)
        {
            // Dano
            if (Lists.Player[i].Hurt + 325 < Environment.TickCount) Lists.Player[i].Hurt = 0;

            // Movimentaçãp
            ProcessMovement(i);
        }
    }

    public static bool TryingMove()
    {
        // Se estiver pressionando alguma tecla, está tentando se mover
        if (Game.Press_Up || Game.Press_Down || Game.Press_Left || Game.Press_Right)
            return true;
        else
            return false;
    }

    public static bool CanMove()
    {
        // Não mover se já estiver tentando movimentar-se
        if (Lists.Player[MyIndex].Movement != Game.Movements.Stopped)
            return false;

        return true;
    }

    public static void CheckMovement()
    {
        if (Me.Movement > 0) return;

        // Move o personagem
        if (Game.Press_Up) Movee(Game.Directions.Up);
        else if (Game.Press_Down) Movee(Game.Directions.Down);
        else if (Game.Press_Left) Movee(Game.Directions.Left);
        else if (Game.Press_Right) Movee(Game.Directions.Right);
    }

    public static void Movee(Game.Directions Direction)
    {
        // Verifica se o jogador pode se mover
        if (!CanMove()) return;

        // Define a direção do jogador
        if (Lists.Player[MyIndex].Direction != Direction)
        {
            Lists.Player[MyIndex].Direction = Direction;
            Send.Player_Direction();
        }

        // Verifica se o azulejo seguinte está livre
        if (Map.Tile_Blocked(Lists.Player[MyIndex].Map, Lists.Player[MyIndex].X, Lists.Player[MyIndex].Y, Direction)) return;

        // Define a velocidade que o jogador se move
        if (Game.Press_Shift)
            Lists.Player[MyIndex].Movement = Game.Movements.Moving;
        else
            Lists.Player[MyIndex].Movement = Game.Movements.Walking;

        // Movimento o jogador
        Send.Player_Move();

        // Define a Posição exata do jogador
        switch (Direction)
        {
            case Game.Directions.Up: Lists.Player[MyIndex].Y2 = Game.Grid; Lists.Player[MyIndex].Y -= 1; break;
            case Game.Directions.Down: Lists.Player[MyIndex].Y2 = Game.Grid * -1; Lists.Player[MyIndex].Y += 1; break;
            case Game.Directions.Right: Lists.Player[MyIndex].X2 = Game.Grid * -1; Lists.Player[MyIndex].X += 1; break;
            case Game.Directions.Left: Lists.Player[MyIndex].X2 = Game.Grid; Lists.Player[MyIndex].X -= 1; break;
        }
    }

    public static void ProcessMovement(byte Index)
    {
        byte Speed = 0;
        short x = Lists.Player[Index].X2, y = Lists.Player[Index].Y2;

        // Reseta a animação se necessário
        if (Lists.Player[Index].Animation == Game.Animation_Stopped) Lists.Player[Index].Animation = Game.Animation_Right;

        // Define a velocidade que o jogador se move
        switch (Lists.Player[Index].Movement)
        {
            case Game.Movements.Walking: Speed = 2; break;
            case Game.Movements.Moving: Speed = 3; break;
            case Game.Movements.Stopped:
                // Reseta os dados
                Lists.Player[Index].X2 = 0;
                Lists.Player[Index].Y2 = 0;
                return;
        }

        // Define a Posição exata do jogador
        switch (Lists.Player[Index].Direction)
        {
            case Game.Directions.Up: Lists.Player[Index].Y2 -= Speed; break;
            case Game.Directions.Down: Lists.Player[Index].Y2 += Speed; break;
            case Game.Directions.Right: Lists.Player[Index].X2 += Speed; break;
            case Game.Directions.Left: Lists.Player[Index].X2 -= Speed; break;
        }

        // Verifica se não passou do limite
        if (x > 0 && Lists.Player[Index].X2 < 0) Lists.Player[Index].X2 = 0;
        if (x < 0 && Lists.Player[Index].X2 > 0) Lists.Player[Index].X2 = 0;
        if (y > 0 && Lists.Player[Index].Y2 < 0) Lists.Player[Index].Y2 = 0;
        if (y < 0 && Lists.Player[Index].Y2 > 0) Lists.Player[Index].Y2 = 0;

        // Alterar as animações somente quando necessário
        if (Lists.Player[Index].Direction == Game.Directions.Right || Lists.Player[Index].Direction == Game.Directions.Down)
        {
            if (Lists.Player[Index].X2 < 0 || Lists.Player[Index].Y2 < 0)
                return;
        }
        else if (Lists.Player[Index].X2 > 0 || Lists.Player[Index].Y2 > 0)
            return;

        // Define as animações
        Lists.Player[Index].Movement = Game.Movements.Stopped;
        if (Lists.Player[Index].Animation == Game.Animation_Left)
            Lists.Player[Index].Animation = Game.Animation_Right;
        else
            Lists.Player[Index].Animation = Game.Animation_Left;
    }

    public static void CheckAttack()
    {
        // Reseta o ataque
        if (Me.Attack_Timer + Game.Attack_Speed < Environment.TickCount)
        {
            Me.Attack_Timer = 0;
            Me.Attacking = false;
        }

        // Somente se estiver pressionando a tecla de ataque e não estiver atacando
        if (!Game.Press_Control) return;
        if (Me.Attack_Timer > 0) return;

        // Envia os dados para o servidor
        Me.Attack_Timer = Environment.TickCount;
        Send.Player_Attack();
    }

    public static void CollectItem()
    {
        bool HasItem = false, HasSlot = false;

        // Previne erros
        if (Tools.CurrentWindow != Tools.Windows.Game) return;

        // Verifica se tem algum item nas coordenadas 
        for (byte i = 1; i <= Lists.Map.Temp_Item.GetUpperBound(0); i++)
            if (Lists.Map.Temp_Item[i].X == Me.X && Lists.Map.Temp_Item[i].Y == Me.Y)
                HasItem = true;

        // Verifica se tem algum espaço vazio no inventário
        for (byte i = 1; i <= Game.Max_Inventory; i++)
            if (Inventory[i].Item_Num == 0)
                HasSlot = true;

        // Somente se necessário
        if (!HasItem) return;
        if (!HasSlot) return;
        if (Environment.TickCount <= Me.Collect_Timer + 250) return;
        if (Panels.Find("Chat").General.Visible) return;

        // Coleta o item
        Send.CollectItem();
        Me.Collect_Timer = Environment.TickCount;
    }
}

partial class Send
{
    public static void Player_Direction()
    {
        NetOutgoingMessage Dados = Socket.Device.CreateMessage();

        // Envia os dados
        Dados.Write((byte)Packets.Player_Direction);
        Dados.Write((byte)Player.Me.Direction);
        Packet(Dados);
    }

    public static void Player_Move()
    {
        NetOutgoingMessage Dados = Socket.Device.CreateMessage();

        // Envia os dados
        Dados.Write((byte)Packets.Player_Move);
        Dados.Write(Player.Me.X);
        Dados.Write(Player.Me.Y);
        Dados.Write((byte)Player.Me.Movement);
        Packet(Dados);
    }

    public static void Player_Attack()
    {
        NetOutgoingMessage Dados = Socket.Device.CreateMessage();

        // Envia os dados
        Dados.Write((byte)Packets.Player_Attack);
        Packet(Dados);
    }
}

partial class Receive
{
    private static void Player_Data(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Defini os dados do jogador
        Lists.Player[Index].Name = Data.ReadString();
        Lists.Player[Index].Class = Data.ReadByte();
        Lists.Player[Index].Genre = Data.ReadBoolean();
        Lists.Player[Index].Level = Data.ReadInt16();
        Lists.Player[Index].Map = Data.ReadInt16();
        Lists.Player[Index].X = Data.ReadByte();
        Lists.Player[Index].Y = Data.ReadByte();
        Lists.Player[Index].Direction = (Game.Directions)Data.ReadByte();
        for (byte n = 0; n <= (byte)Game.Vitals.Amount - 1; n++)
        {
            Lists.Player[Index].Vital[n] = Data.ReadInt16();
            Lists.Player[Index].Max_Vital[n] = Data.ReadInt16();
        }
        for (byte n = 0; n <= (byte)Game.Attributes.Amount - 1; n++) Lists.Player[Index].Attribute[n] = Data.ReadInt16();
        for (byte n = 0; n <= (byte)Game.Equipments.Amount - 1; n++) Lists.Player[Index].Equipment[n] = Data.ReadInt16();
    }

    private static void Player_Position(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Defini os dados do jogador
        Lists.Player[Index].X = Data.ReadByte();
        Lists.Player[Index].Y = Data.ReadByte();
        Lists.Player[Index].Direction = (Game.Directions)Data.ReadByte();

        // Para a movimentação
        Lists.Player[Index].X2 = 0;
        Lists.Player[Index].Y2 = 0;
        Lists.Player[Index].Movement = Game.Movements.Stopped;
    }

    private static void Player_Vitals(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Define os dados
        for (byte i = 0; i <= (byte)Game.Vitals.Amount - 1; i++)
        {
            Lists.Player[Index].Vital[i] = Data.ReadInt16();
            Lists.Player[Index].Max_Vital[i] = Data.ReadInt16();
        }
    }

    private static void Player_Equipments(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Define os dados
        for (byte i = 0; i <= (byte)Game.Equipments.Amount - 1; i++) Lists.Player[Index].Equipment[i] = Data.ReadInt16();
    }

    private static void Player_Leave(NetIncomingMessage Dados)
    {
        // Limpa os dados do jogador
        Clear.Player(Dados.ReadByte());
    }

    public static void Player_Move(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Move o jogador
        Lists.Player[Index].X = Data.ReadByte();
        Lists.Player[Index].Y = Data.ReadByte();
        Lists.Player[Index].Direction = (Game.Directions)Data.ReadByte();
        Lists.Player[Index].Movement = (Game.Movements)Data.ReadByte();
        Lists.Player[Index].X2 = 0;
        Lists.Player[Index].Y2 = 0;

        // Posição exata do jogador
        switch (Lists.Player[Index].Direction)
        {
            case Game.Directions.Up: Lists.Player[Index].Y2 = Game.Grid; break;
            case Game.Directions.Down: Lists.Player[Index].Y2 = Game.Grid * -1; break;
            case Game.Directions.Right: Lists.Player[Index].X2 = Game.Grid * -1; break;
            case Game.Directions.Left: Lists.Player[Index].X2 = Game.Grid; break;
        }
    }

    public static void Player_Direction(NetIncomingMessage Data)
    {
        // Define a direção de determinado jogador
        Lists.Player[Data.ReadByte()].Direction = (Game.Directions)Data.ReadByte();
    }

    public static void Player_Attack(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte(), Victim = Data.ReadByte(), Victim_Type = Data.ReadByte();

        // Inicia o ataque
        Lists.Player[Index].Attacking = true;
        Lists.Player[Index].Attack_Timer = Environment.TickCount;

        // Sofrendo dano
        if (Victim > 0)
            if (Victim_Type == (byte)Game.Target.Player)
                Lists.Player[Victim].Hurt = Environment.TickCount;
            else if (Victim_Type == (byte)Game.Target.NPC)
                Lists.Map.Temp_NPC[Victim].Hurt = Environment.TickCount;
    }

    public static void Player_Experience(NetIncomingMessage Data)
    {
        // Define os dados
        Player.Me.Experience = Data.ReadInt16();
        Player.Me.ExpNeeded = Data.ReadInt16();
        Player.Me.Points = Data.ReadByte();
    }

    private static void Player_Inventory(NetIncomingMessage Data)
    {
        // Define os dados
        for (byte i = 1; i <= Game.Max_Inventory; i++)
        {
            Player.Inventory[i].Item_Num = Data.ReadInt16();
            Player.Inventory[i].Amount = Data.ReadInt16();
        }
    }

    private static void Player_Hotbar(NetIncomingMessage Data)
    {
        // Define os dados
        for (byte i = 1; i <= Game.Max_Hotbar; i++)
        {
            Player.Hotbar[i].Type = Data.ReadByte();
            Player.Hotbar[i].Slot = Data.ReadByte();
        }
    }
}

partial class Graphics
{
    public static void Player_Character(byte Index)
    {
        // Desenha o jogador
        Player_Texture(Index);
        Player_Name(Index);
        Player_Bars(Index);
    }

    public static void Player_Texture(byte Index)
    {
        byte Column = Game.Animation_Stopped;
        int x = Lists.Player[Index].X * Game.Grid + Lists.Player[Index].X2, y = Lists.Player[Index].Y * Game.Grid + Lists.Player[Index].Y2;
        short x2 = Lists.Player[Index].X2, y2 = Lists.Player[Index].Y2;
        bool Hurt = false;
        short Texture = Player.Player_Texture(Index);

        // Previne sobrecargas
        if (Texture <= 0 || Texture > Tex_Character.GetUpperBound(0)) return;

        // Define a animação
        if (Lists.Player[Index].Attacking && Lists.Player[Index].Attack_Timer + Game.Attack_Speed / 2 > Environment.TickCount)
            Column = Game.Animation_Attack;
        else
        {
            if (x2 > 8 && x2 < Game.Grid) Column = Lists.Player[Index].Animation;
            if (x2 < -8 && x2 > Game.Grid * -1) Column = Lists.Player[Index].Animation;
            if (y2 > 8 && y2 < Game.Grid) Column = Lists.Player[Index].Animation;
            if (y2 < -8 && y2 > Game.Grid * -1) Column = Lists.Player[Index].Animation;
        }

        // Demonstra que o personagem está sofrendo dano
        if (Lists.Player[Index].Hurt > 0) Hurt = true;

        // Desenha o jogador
        Character(Texture, new Point(Game.ConvertX(x), Game.ConvertY(y)), Lists.Player[Index].Direction, Column, Hurt);
    }

    public static void Player_Bars(byte Index)
    {
        Size Chracater_Size = TSize(Tex_Character[Player.Player_Texture(Index)]);
        int x = Lists.Player[Index].X * Game.Grid + Lists.Player[Index].X2, y = Lists.Player[Index].Y * Game.Grid + Lists.Player[Index].Y2;
        Point Position = new Point(Game.ConvertX(x), Game.ConvertY(y) + Chracater_Size.Height / Game.Animation_Amount + 4);
        int FullWidth = Chracater_Size.Width / Game.Animation_Amount;
        short Value = Lists.Player[Index].Vital[(byte)Game.Vitals.HP];

        // Apenas se necessário
        if (Value <= 0 || Value >= Lists.Player[Index].Max_Vital[(byte)Game.Vitals.HP]) return;

        // Cálcula a largura da barra
        int Width = (Value * FullWidth) / Lists.Player[Index].Max_Vital[(byte)Game.Vitals.HP];

        // Desenha as barras 
        Render(Tex_Bars, Position.X, Position.Y, 0, 4, FullWidth, 4);
        Render(Tex_Bars, Position.X, Position.Y, 0, 0, Width, 4);
    }

    public static void Player_Name(byte Index)
    {
        Texture Texture = Tex_Character[Player.Player_Texture(Index)];
        int Name_Size = Tools.MeasureString(Lists.Player[Index].Name);
        int x = Lists.Player[Index].X * Game.Grid + Lists.Player[Index].X2, y = Lists.Player[Index].Y * Game.Grid + Lists.Player[Index].Y2;

        // Posição do texto
        Point Position = new Point();
        Position.X = x + TSize(Texture).Width / Game.Animation_Amount / 2 - Name_Size / 2;
        Position.Y = y - TSize(Texture).Height / Game.Animation_Amount / 2;

        // Cor do texto
        SFML.Graphics.Color Color;
        if (Index == Player.MyIndex)
            Color = SFML.Graphics.Color.Yellow;
        else
            Color = SFML.Graphics.Color.White;

        // Desenha o texto
        DrawText(Lists.Player[Index].Name, Game.ConvertX(Position.X), Game.ConvertY(Position.Y), Color);
    }
}