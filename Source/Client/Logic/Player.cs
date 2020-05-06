using System;
using SFML.Window;

class Player
{
    // Obtém um jogador com determinado nome
    public static Structure Get(string Name) => Lists.Player.Find(x => x.Name.Equals(Name));

    // O próprio jogador
    public static Me_Structure Me;

    public static void Logic()
    {
        // Verificações
        Me.CheckMovement();
        Me.CheckAttack();

        for (byte i = 0; i < Lists.Player.Count; i++)
        {
            // Dano
            if (Lists.Player[i].Hurt + 325 < Environment.TickCount)
                Lists.Player[i].Hurt = 0;

            // Movimentaçãp
            Lists.Player[i].ProcessMovement();
        }
    }

    // Dados gerais dos jogadores
    public class Structure
    {
        // Geral
        public string Name = string.Empty;
        public short Texture_Num;
        public short Level;
        public short[] Vital = new short[(byte)Game.Vitals.Count];
        public short[] Max_Vital = new short[(byte)Game.Vitals.Count];
        public short[] Attribute = new short[(byte)Game.Attributes.Count];
        public short[] Equipment = new short[(byte)Game.Equipments.Count];
        public short Map_Num;
        public byte X;
        public byte Y;
        public Game.Directions Direction;
        public Game.Movements Movement;

        // Dados temporários
        public short X2;
        public short Y2;
        public byte Animation;
        public bool Attacking;
        public int Hurt;
        public int Attack_Timer;

        public Structure(string Name)
        {
            this.Name = Name;
        }

        // Posição do pixel exato em que o jogador está
        public int Pixel_X => X * Game.Grid + X2;
        public int Pixel_Y => Y * Game.Grid + Y2;

        public void ProcessMovement()
        {
            byte Speed = 0;
            short x = X2, y = Y2;

            // Reseta a animação se necessário
            if (Animation == Game.Animation_Stopped) Animation = Game.Animation_Right;

            // Define a velocidade que o jogador se move
            switch (Movement)
            {
                case Game.Movements.Walking: Speed = 2; break;
                case Game.Movements.Moving: Speed = 3; break;
                case Game.Movements.Stopped:
                    // Reseta os dados
                    X2 = 0;
                    Y2 = 0;
                    return;
            }

            // Define a Posição exata do jogador
            switch (Direction)
            {
                case Game.Directions.Up: Y2 -= Speed; break;
                case Game.Directions.Down: Y2 += Speed; break;
                case Game.Directions.Right: X2 += Speed; break;
                case Game.Directions.Left: X2 -= Speed; break;
            }

            // Verifica se não passou do limite
            if (x > 0 && X2 < 0) X2 = 0;
            if (x < 0 && X2 > 0) X2 = 0;
            if (y > 0 && Y2 < 0) Y2 = 0;
            if (y < 0 && Y2 > 0) Y2 = 0;

            // Alterar as animações somente quando necessário
            if (Direction == Game.Directions.Right || Direction == Game.Directions.Down)
            {
                if (X2 < 0 || Y2 < 0)
                    return;
            }
            else if (X2 > 0 || Y2 > 0)
                return;

            // Define as animações
            Movement = Game.Movements.Stopped;
            if (Animation == Game.Animation_Left)
                Animation = Game.Animation_Right;
            else
                Animation = Game.Animation_Left;
        }
    }

    // Dados somente do próprio jogador
    public class Me_Structure : Structure
    {
        // Dados
        public Lists.Structures.Inventory[] Inventory = new Lists.Structures.Inventory[Game.Max_Inventory + 1];
        public Lists.Structures.Hotbar[] Hotbar = new Lists.Structures.Hotbar[Game.Max_Hotbar + 1];
        public Lists.Structures.Inventory[] Trade_Offer;
        public Lists.Structures.Inventory[] Trade_Their_Offer;
        public Structure[] Party = Array.Empty<Structure>();
        public int Experience;
        public int ExpNeeded;
        public short Points;
        public int Collect_Timer;

        // Construtor
        public Me_Structure(string Name) : base(Name) { }

        public void CheckMovement()
        {
            if (Movement > 0 || !Graphics.RenderWindow.HasFocus()) return;

            // Move o personagem
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) Move(Game.Directions.Up);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) Move(Game.Directions.Down);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) Move(Game.Directions.Left);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) Move(Game.Directions.Right);
        }

        public void Move(Game.Directions Direction)
        {
            // Verifica se o jogador pode se mover
            if (Movement != Game.Movements.Stopped) return;

            // Define a direção do jogador
            if (this.Direction != Direction)
            {
                this.Direction = Direction;
                Send.Player_Direction();
            }

            // Verifica se o azulejo seguinte está livre
            if (Map.Tile_Blocked(Map_Num, X, Y, Direction)) return;

            // Define a velocidade que o jogador se move
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Graphics.RenderWindow.HasFocus())
                Movement = Game.Movements.Moving;
            else
                Movement = Game.Movements.Walking;

            // Movimento o jogador
            Send.Player_Move();

            // Define a Posição exata do jogador
            switch (Direction)
            {
                case Game.Directions.Up: Y2 = Game.Grid; Y -= 1; break;
                case Game.Directions.Down: Y2 = Game.Grid * -1; Y += 1; break;
                case Game.Directions.Right: X2 = Game.Grid * -1; X += 1; break;
                case Game.Directions.Left: X2 = Game.Grid; X -= 1; break;
            }
        }

        public void CheckAttack()
        {
            // Reseta o ataque
            if (Attack_Timer + Game.Attack_Speed < Environment.TickCount)
            {
                Attack_Timer = 0;
                Attacking = false;
            }

            // Somente se estiver pressionando a tecla de ataque e não estiver atacando
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl) || !Graphics.RenderWindow.HasFocus()) return;
            if (Attack_Timer > 0) return;
            if (Panels.Get("Trade").Visible) return;
            if (Panels.Get("Shop").Visible) return;

            // Envia os dados para o servidor
            Attack_Timer = Environment.TickCount;
            Send.Player_Attack();
        }

        public void CollectItem()
        {
            bool HasItem = false, HasSlot = false;

            // Previne erros
            if (TextBoxes.Focused != null) return;

            // Verifica se tem algum item nas coordenadas 
            for (byte i = 1; i < Lists.Temp_Map.Item.Length; i++)
                if (Lists.Temp_Map.Item[i].X == X && Lists.Temp_Map.Item[i].Y == Y)
                    HasItem = true;

            // Verifica se tem algum espaço vazio no inventário
            for (byte i = 1; i <= Game.Max_Inventory; i++)
                if (Inventory[i].Item_Num == 0)
                    HasSlot = true;

            // Somente se necessário
            if (!HasItem) return;
            if (!HasSlot) return;
            if (Environment.TickCount <= Collect_Timer + 250) return;

            // Coleta o item
            Send.CollectItem();
            Collect_Timer = Environment.TickCount;
        }

        public void Leave()
        {
            // Reseta os dados
            Lists.Player.Clear();
            Me = null;

            // Volta ao menu
            Window.OpenMenu();
        }
    }
}