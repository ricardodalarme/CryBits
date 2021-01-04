using System;
using CryBits.Client.Media;
using CryBits.Client.Network;
using CryBits.Client.UI;
using CryBits.Entities;
using CryBits.Enums;
using SFML.Window;
using static CryBits.Globals;

namespace CryBits.Client.Entities
{
    // Dados somente do próprio jogador
    internal class Me : Player
    {
        // Dados
        public ItemSlot[] Inventory = new ItemSlot[MaxInventory];
        public HotbarSlot[] Hotbar = new HotbarSlot[MaxHotbar];
        public ItemSlot[] TradeOffer;
        public ItemSlot[] TradeTheirOffer;
        public Player[] Party = Array.Empty<Player>();
        public int Experience;
        public int ExpNeeded;
        public short Points;
        private int _collectTimer;

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
            if (Movement > 0 || !Graphicss.RenderWindow.HasFocus()) return;

            // Move o personagem
            if (Keyboard.IsKeyPressed(Keyboard.Key.Up)) Move(Direction.Up);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Down)) Move(Direction.Down);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Left)) Move(Direction.Left);
            else if (Keyboard.IsKeyPressed(Keyboard.Key.Right)) Move(Direction.Right);
        }

        public void Move(Direction direction)
        {
            // Verifica se o jogador pode se mover
            if (Movement != Movement.Stopped) return;

            // Define a direção do jogador
            if (Direction != direction)
            {
                Direction = direction;
                Send.PlayerDirection();
            }

            // Verifica se o azulejo seguinte está livre
            if (Map.TileBlocked(X, Y, direction)) return;

            // Define a velocidade que o jogador se move
            if (Keyboard.IsKeyPressed(Keyboard.Key.LShift) && Graphicss.RenderWindow.HasFocus())
                Movement = Movement.Moving;
            else
                Movement = Movement.Walking;

            // Movimento o jogador
            Send.PlayerMove();

            // Define a Posição exata do jogador
            switch (direction)
            {
                case Direction.Up: Y2 = Grid; Y -= 1; break;
                case Direction.Down: Y2 = Grid * -1; Y += 1; break;
                case Direction.Right: X2 = Grid * -1; X += 1; break;
                case Direction.Left: X2 = Grid; X -= 1; break;
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
            if (!Keyboard.IsKeyPressed(Keyboard.Key.LControl) || !Graphicss.RenderWindow.HasFocus()) return;
            if (AttackTimer > 0) return;
            if (Panels.List["Trade"].Visible) return;
            if (Panels.List["Shop"].Visible) return;

            // Envia os dados para o servidor
            AttackTimer = Environment.TickCount;
            Send.PlayerAttack();
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
            if (Environment.TickCount <= _collectTimer + 250) return;

            // Coleta o item
            Send.CollectItem();
            _collectTimer = Environment.TickCount;
        }

        public void Leave()
        {
            // Reseta os dados
            List.Clear();
            Me = null;
        }
    }
}