using Lidgren.Network;
using SFML.Graphics;
using System;
using System.Drawing;

class NPC
{
    public static void Logic()
    {
        // Lógica dos NPCs
        for (byte i = 1; i < Lists.Map.Temp_NPC.Length; i++)
            if (Lists.Map.Temp_NPC[i].Index > 0)
            {
                // Dano
                if (Lists.Map.Temp_NPC[i].Hurt + 325 < Environment.TickCount) Lists.Map.Temp_NPC[i].Hurt = 0;

                // Movimento
                ProcessMovement(i);
            }
    }

    public static void ProcessMovement(byte Index)
    {
        byte Speed = 0;
        short x = Lists.Map.Temp_NPC[Index].X2, y = Lists.Map.Temp_NPC[Index].Y2;

        // Reseta a animação se necessário
        if (Lists.Map.Temp_NPC[Index].Animation == Game.Animation_Stopped) Lists.Map.Temp_NPC[Index].Animation = Game.Animation_Right;

        // Define a velocidade que o jogador se move
        switch (Lists.Map.Temp_NPC[Index].Movement)
        {
            case Game.Movements.Walking: Speed = 2; break;
            case Game.Movements.Moving: Speed = 3; break;
            case Game.Movements.Stopped:
                // Reseta os dados
                Lists.Map.Temp_NPC[Index].X2 = 0;
                Lists.Map.Temp_NPC[Index].Y2 = 0;
                return;
        }

        // Define a Posição exata do jogador
        switch (Lists.Map.Temp_NPC[Index].Direction)
        {
            case Game.Directions.Up: Lists.Map.Temp_NPC[Index].Y2 -= Speed; break;
            case Game.Directions.Down: Lists.Map.Temp_NPC[Index].Y2 += Speed; break;
            case Game.Directions.Right: Lists.Map.Temp_NPC[Index].X2 += Speed; break;
            case Game.Directions.Left: Lists.Map.Temp_NPC[Index].X2 -= Speed; break;
        }

        // Verifica se não passou do limite
        if (x > 0 && Lists.Map.Temp_NPC[Index].X2 < 0) Lists.Map.Temp_NPC[Index].X2 = 0;
        if (x < 0 && Lists.Map.Temp_NPC[Index].X2 > 0) Lists.Map.Temp_NPC[Index].X2 = 0;
        if (y > 0 && Lists.Map.Temp_NPC[Index].Y2 < 0) Lists.Map.Temp_NPC[Index].Y2 = 0;
        if (y < 0 && Lists.Map.Temp_NPC[Index].Y2 > 0) Lists.Map.Temp_NPC[Index].Y2 = 0;

        // Alterar as animações somente quando necessário
        if (Lists.Map.Temp_NPC[Index].Direction == Game.Directions.Right || Lists.Map.Temp_NPC[Index].Direction == Game.Directions.Down)
        {
            if (Lists.Map.Temp_NPC[Index].X2 < 0 || Lists.Map.Temp_NPC[Index].Y2 < 0)
                return;
        }
        else if (Lists.Map.Temp_NPC[Index].X2 > 0 || Lists.Map.Temp_NPC[Index].Y2 > 0)
            return;

        // Define as animações
        Lists.Map.Temp_NPC[Index].Movement = Game.Movements.Stopped;
        if (Lists.Map.Temp_NPC[Index].Animation == Game.Animation_Left)
            Lists.Map.Temp_NPC[Index].Animation = Game.Animation_Right;
        else
            Lists.Map.Temp_NPC[Index].Animation = Game.Animation_Left;
    }
}

partial class Receive
{
    public static void NPCs(NetIncomingMessage Data)
    {
        // Quantidade
        Lists.NPC = new Lists.Structures.NPCs[Data.ReadInt16() + 1];

        // Lê os dados de todos
        for (byte i = 1; i < Lists.NPC.Length; i++)
        {
            // Geral
            Lists.NPC[i].Name = Data.ReadString();
            Lists.NPC[i].Texture = Data.ReadInt16();
            Lists.NPC[i].Type = Data.ReadByte();

            // Vitais
            Lists.NPC[i].Vital = new short[(byte)Game.Vitals.Count];
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
                Lists.NPC[i].Vital[n] = Data.ReadInt16();
        }
    }

    public static void Map_NPCs(NetIncomingMessage Data)
    {
        // Lê os dados
        Lists.Map.Temp_NPC = new Lists.Structures.Map_NPCs[Data.ReadInt16() + 1];
        for (byte i = 1; i < Lists.Map.Temp_NPC.Length; i++)
        {
            Lists.Map.Temp_NPC[i].X2 = 0;
            Lists.Map.Temp_NPC[i].Y2 = 0;
            Lists.Map.Temp_NPC[i].Index = Data.ReadInt16();
            Lists.Map.Temp_NPC[i].X = Data.ReadByte();
            Lists.Map.Temp_NPC[i].Y = Data.ReadByte();
            Lists.Map.Temp_NPC[i].Direction = (Game.Directions)Data.ReadByte();

            // Vitais
            Lists.Map.Temp_NPC[i].Vital = new short[(byte)Game.Vitals.Count];
            for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
                Lists.Map.Temp_NPC[i].Vital[n] = Data.ReadInt16();
        }
    }

    public static void Map_NPC(NetIncomingMessage Data)
    {
        // Lê os dados
        byte i = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Index = Data.ReadInt16();
        Lists.Map.Temp_NPC[i].X = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Y = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Direction = (Game.Directions)Data.ReadByte();
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++) Lists.Map.Temp_NPC[i].Vital[n] = Data.ReadInt16();
    }

    public static void Map_NPC_Movement(NetIncomingMessage Data)
    {
        // Lê os dados
        byte i = Data.ReadByte();
        byte x = Lists.Map.Temp_NPC[i].X, y = Lists.Map.Temp_NPC[i].Y;
        Lists.Map.Temp_NPC[i].X2 = 0;
        Lists.Map.Temp_NPC[i].Y2 = 0;
        Lists.Map.Temp_NPC[i].X = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Y = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Direction = (Game.Directions)Data.ReadByte();
        Lists.Map.Temp_NPC[i].Movement = (Game.Movements)Data.ReadByte();

        // Posição exata do jogador
        if (x != Lists.Map.Temp_NPC[i].X || y != Lists.Map.Temp_NPC[i].Y)
            switch (Lists.Map.Temp_NPC[i].Direction)
            {
                case Game.Directions.Up: Lists.Map.Temp_NPC[i].Y2 = Game.Grid; break;
                case Game.Directions.Down: Lists.Map.Temp_NPC[i].Y2 = Game.Grid * -1; break;
                case Game.Directions.Right: Lists.Map.Temp_NPC[i].X2 = Game.Grid * -1; break;
                case Game.Directions.Left: Lists.Map.Temp_NPC[i].X2 = Game.Grid; break;
            }
    }

    public static void Map_NPC_Attack(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte(), Victim = Data.ReadByte(), Victim_Type = Data.ReadByte();

        // Inicia o ataque
        Lists.Map.Temp_NPC[Index].Attacking = true;
        Lists.Map.Temp_NPC[Index].Attack_Timer = Environment.TickCount;

        // Sofrendo dano
        if (Victim > 0)
            if (Victim_Type == (byte)Game.Target.Player)
                Lists.Player[Victim].Hurt = Environment.TickCount;
            else if (Victim_Type == (byte)Game.Target.NPC)
                Lists.Map.Temp_NPC[Victim].Hurt = Environment.TickCount;
    }

    public static void Map_NPC_Direction(NetIncomingMessage Data)
    {
        // Define a direção de determinado NPC
        byte i = Data.ReadByte();
        Lists.Map.Temp_NPC[i].Direction = (Game.Directions)Data.ReadByte();
        Lists.Map.Temp_NPC[i].X2 = 0;
        Lists.Map.Temp_NPC[i].Y2 = 0;
    }

    public static void Map_NPC_Vitals(NetIncomingMessage Data)
    {
        byte Index = Data.ReadByte();

        // Define os vitais de determinado NPC
        for (byte n = 0; n < (byte)Game.Vitals.Count; n++)
            Lists.Map.Temp_NPC[Index].Vital[n] = Data.ReadInt16();
    }

    public static void Map_NPC_Died(NetIncomingMessage Data)
    {
        byte i = Data.ReadByte();

        // Limpa os dados do NPC
        Lists.Map.Temp_NPC[i].X2 = 0;
        Lists.Map.Temp_NPC[i].Y2 = 0;
        Lists.Map.Temp_NPC[i].Index = 0;
        Lists.Map.Temp_NPC[i].X = 0;
        Lists.Map.Temp_NPC[i].Y = 0;
        Lists.Map.Temp_NPC[i].Vital = new short[(byte)Game.Vitals.Count];
    }
}

partial class Graphics
{
    public static void NPC(byte Index)
    {
        int x2 = Lists.Map.Temp_NPC[Index].X2, y2 = Lists.Map.Temp_NPC[Index].Y2;
        byte Column = 0;
        bool Hurt = false;
        short Texture = Lists.NPC[Lists.Map.Temp_NPC[Index].Index].Texture;

        // Previne sobrecargas
        if (Texture <= 0 || Texture > Tex_Character.GetUpperBound(0)) return;

        // Define a animação
        if (Lists.Map.Temp_NPC[Index].Attacking && Lists.Map.Temp_NPC[Index].Attack_Timer + Game.Attack_Speed / 2 > Environment.TickCount)
            Column = Game.Animation_Attack;
        else
        {
            if (x2 > 8 && x2 < Game.Grid) Column = Lists.Map.Temp_NPC[Index].Animation;
            else if (x2 < -8 && x2 > Game.Grid * -1) Column = Lists.Map.Temp_NPC[Index].Animation;
            else if (y2 > 8 && y2 < Game.Grid) Column = Lists.Map.Temp_NPC[Index].Animation;
            else if (y2 < -8 && y2 > Game.Grid * -1) Column = Lists.Map.Temp_NPC[Index].Animation;
        }

        // Demonstra que o personagem está sofrendo dano
        if (Lists.Map.Temp_NPC[Index].Hurt > 0) Hurt = true;

        // Desenha o jogador
        int x = Lists.Map.Temp_NPC[Index].X * Game.Grid + x2;
        int y = Lists.Map.Temp_NPC[Index].Y * Game.Grid + y2;
        Character(Texture, new Point(Game.ConvertX(x), Game.ConvertY(y)), Lists.Map.Temp_NPC[Index].Direction, Column, Hurt);
        NPC_Name(Index, x, y);
        NPC_Bars(Index, x, y);
    }

    public static void NPC_Name(byte Index, int x, int y)
    {
        Point Position = new Point(); SFML.Graphics.Color Color;
        short NPC_Num = Lists.Map.Temp_NPC[Index].Index;
        int Name_Size = Tools.MeasureString(Lists.NPC[NPC_Num].Name);
        Texture Texture = Tex_Character[Lists.NPC[NPC_Num].Texture];

        // Posição do texto
        Position.X = x + TSize(Texture).Width / Game.Animation_Amount / 2 - Name_Size / 2;
        Position.Y = y - TSize(Texture).Height / Game.Animation_Amount / 2;

        // Cor do texto
        switch ((Game.NPCs)Lists.NPC[NPC_Num].Type)
        {
            case Game.NPCs.Friendly: Color = SFML.Graphics.Color.White; break;
            case Game.NPCs.AttackOnSight: Color = SFML.Graphics.Color.Red; break;
            case Game.NPCs.AttackWhenAttacked: Color = new SFML.Graphics.Color(228, 120, 51); break;
            default: Color = SFML.Graphics.Color.White; break;
        }

        // Desenha o texto
        DrawText(Lists.NPC[NPC_Num].Name, Game.ConvertX(Position.X), Game.ConvertY(Position.Y), Color);
    }

    public static void NPC_Bars(byte Index, int x, int y)
    {
        Lists.Structures.Map_NPCs NPC = Lists.Map.Temp_NPC[Index];
        Texture Texture = Tex_Character[Lists.NPC[NPC.Index].Texture];
        short Value = NPC.Vital[(byte)Game.Vitals.HP];

        // Apenas se necessário
        if (Value <= 0 || Value >= Lists.NPC[NPC.Index].Vital[(byte)Game.Vitals.HP]) return;

        // Posição
        Point Position = new Point(Game.ConvertX(x), Game.ConvertY(y) + TSize(Texture).Height / Game.Animation_Amount + 4);
        int FullWidth = TSize(Texture).Width / Game.Animation_Amount;
        int Width = (Value * FullWidth) / Lists.NPC[NPC.Index].Vital[(byte)Game.Vitals.HP];

        // Desenha a barra 
        Render(Tex_Bars, Position.X, Position.Y, 0, 4, FullWidth, 4);
        Render(Tex_Bars, Position.X, Position.Y, 0, 0, Width, 4);
    }
}