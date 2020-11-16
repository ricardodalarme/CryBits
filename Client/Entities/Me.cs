using System;
using CryBits.Client.Media;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Entities;
using SFML.Window;
using static CryBits.Defaults;

namespace CryBits.Client.Entities
{
    // Dados somente do próprio jogador
    internal class Me : Player
    {
        // Dados
        public ItemSlot[] Inventory = new ItemSlot[MaxInventory];
        public Hotbar[] Hotbar = new Hotbar[MaxHotbar];
        public ItemSlot[] TradeOffer;
        public ItemSlot[] TradeTheirOffer;
        public Player[] Party = Array.Empty<Player>();
        public int Experience;
        public int ExpNeeded;
        public short Points;
        public int CollectTimer;

        // Construtor
        public Me(string name) : base(name) { }

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

        public void Move(Directions direction)
        {
            // Verifica se o jogador pode se mover
            if (Movement != Movements.Stopped) return;

            // Define a direção do jogador
            if (Direction != direction)
            {
                Direction = direction;
                Send.Player_Direction();
            }

            // Verifica se o azulejo seguinte está livre
            if (Map.Tile_Blocked(X, Y, direction)) return;

            // Define a velocidade que o jogador se move
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Graphics.RenderWindow.HasFocus())
                Movement = Movements.Moving;
            else
                Movement = Movements.Walking;

            // Movimento o jogador
            Send.Player_Move();

            // Define a Posição exata do jogador
            switch (direction)
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
            if (AttackTimer + AttackSpeed < Environment.TickCount)
            {
                AttackTimer = 0;
                Attacking = false;
            }

            // Somente se estiver pressionando a tecla de ataque e não estiver atacando
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl) || !Graphics.RenderWindow.HasFocus()) return;
            if (AttackTimer > 0) return;
            if (Panels.List["Trade"].Visible) return;
            if (Panels.List["Shop"].Visible) return;

            // Envia os dados para o servidor
            AttackTimer = Environment.TickCount;
            Send.Player_Attack();
        }

        public void CollectItem()
        {
            bool hasItem = false, hasSlot = false;

            // Previne erros
            if (TextBoxes.Focused != null) return;

            // Verifica se tem algum item nas coordenadas 
            for (byte i = 0; i < TempMap.Current.Item.Length; i++)
                if (TempMap.Current.Item[i].X == X && TempMap.Current.Item[i].Y == Y)
                    hasItem = true;

            // Verifica se tem algum espaço vazio no inventário
            for (byte i = 0; i < MaxInventory; i++)
                if (Inventory[i].Item == null)
                    hasSlot = true;

            // Somente se necessário
            if (!hasItem) return;
            if (!hasSlot) return;
            if (Environment.TickCount <= CollectTimer + 250) return;

            // Coleta o item
            Send.CollectItem();
            CollectTimer = Environment.TickCount;
        }

        public void Leave()
        {
            // Reseta os dados
            List.Clear();
            Me = null;
        }
    }

    public struct Hotbar
    {
        public byte Type;
        public byte Slot;
    }
}