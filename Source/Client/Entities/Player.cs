using CryBits.Client.Interface;
using CryBits;
using CryBits.Client.Network;
using SFML.Window;
using System;
using System.Collections.Generic;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Entities
{
    class Player : Character
    {
        // Lista de dados
        public static List<Player> List;

        // Obtém um jogador com determinado nome
        public static Player Get(string Name) => List.Find(x => x.Name.Equals(Name));

        // O próprio jogador
        public static Me_Structure Me;

        // Dados gerais dos jogadores
        public string Name = string.Empty;
        public short Texture_Num;
        public short Level;
        public short[] Max_Vital = new short[(byte)Vitals.Count];
        public short[] Attribute = new short[(byte)Attributes.Count];
        public Item[] Equipment = new Item[(byte)Equipments.Count];
        public TempMap Map;

        public Player(string Name)
        {
            this.Name = Name;
        }

        public virtual void Logic()
        {
            // Dano
            if (Hurt + 325 < Environment.TickCount) Hurt = 0;

            // Movimentaçãp
            ProcessMovement();
        }
    }

    // Dados somente do próprio jogador
    class Me_Structure : Player
    {
        // Dados
        public Inventory[] Inventory = new Inventory[Max_Inventory + 1];
        public Hotbar[] Hotbar = new Hotbar[Max_Hotbar];
        public Inventory[] Trade_Offer;
        public Inventory[] Trade_Their_Offer;
        public Player[] Party = Array.Empty<Player>();
        public int Experience;
        public int ExpNeeded;
        public short Points;
        public int Collect_Timer;

        // Construtor
        public Me_Structure(string Name) : base(Name) { }

        public override void Logic()
        {
            // Verificações
            Me.CheckMovement();
            Me.CheckAttack();
            base.Logic();
        }

        public void CheckMovement()
        {
            if (Movement > 0 || !Graphics.RenderWindow.HasFocus()) return;

            // Move o personagem
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) Move(Directions.Up);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) Move(Directions.Down);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) Move(Directions.Left);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) Move(Directions.Right);
        }

        public void Move(Directions Direction)
        {
            // Verifica se o jogador pode se mover
            if (Movement != Movements.Stopped) return;

            // Define a direção do jogador
            if (this.Direction != Direction)
            {
                this.Direction = Direction;
                Send.Player_Direction();
            }

            // Verifica se o azulejo seguinte está livre
            if (Map.Tile_Blocked(X, Y, Direction)) return;

            // Define a velocidade que o jogador se move
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Graphics.RenderWindow.HasFocus())
                Movement = Movements.Moving;
            else
                Movement = Movements.Walking;

            // Movimento o jogador
            Send.Player_Move();

            // Define a Posição exata do jogador
            switch (Direction)
            {
                case Directions.Up: Y2 = Grid; Y -= 1; break;
                case Directions.Down: Y2 = Grid * -1; Y += 1; break;
                case Directions.Right: X2 = Grid * -1; X += 1; break;
                case Directions.Left: X2 = Grid; X -= 1; break;
            }
        }

        public void CheckAttack()
        {
            // Reseta o ataque
            if (Attack_Timer + Attack_Speed < Environment.TickCount)
            {
                Attack_Timer = 0;
                Attacking = false;
            }

            // Somente se estiver pressionando a tecla de ataque e não estiver atacando
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl) || !Graphics.RenderWindow.HasFocus()) return;
            if (Attack_Timer > 0) return;
            if (Panels.List["Trade"].Visible) return;
            if (Panels.List["Shop"].Visible) return;

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
            for (byte i = 0; i < Mapper.Current.Item.Length; i++)
                if (Mapper.Current.Item[i].X == X && Mapper.Current.Item[i].Y == Y)
                    HasItem = true;

            // Verifica se tem algum espaço vazio no inventário
            for (byte i = 1; i <= Max_Inventory; i++)
                if (Inventory[i].Item == null)
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
            Player.List.Clear();
            Me = null;
        }
    }

    struct Inventory
    {
        public Item Item;
        public short Amount;
    }

    public struct Hotbar
    {
        public byte Type;
        public byte Slot;
    }
}