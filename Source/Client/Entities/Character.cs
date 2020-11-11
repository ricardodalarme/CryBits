using CryBits;
using static CryBits.Client.Logic.Game;

namespace CryBits.Client.Entities
{
    class Character
    {
        // Geral
        public short[] Vital = new short[(byte)Vitals.Count];
        public byte X;
        public byte Y;
        public Directions Direction;
        public Movements Movement;
        public short X2;
        public short Y2;
        public byte Animation;
        public bool Attacking;
        public int Hurt;
        public int Attack_Timer;

        // Posição exata em que o personagem está
        public int Pixel_X => X * Grid + X2;
        public int Pixel_Y => Y * Grid + Y2;

        public void ProcessMovement()
        {
            byte Speed = 0;
            short x = X2, y = Y2;

            // Reseta a animação se necessário
            if (Animation == Animation_Stopped) Animation = Animation_Right;

            // Define a velocidade que o jogador se move
            switch (Movement)
            {
                case Movements.Walking: Speed = 2; break;
                case Movements.Moving: Speed = 3; break;
                case Movements.Stopped:
                    // Reseta os dados
                    X2 = 0;
                    Y2 = 0;
                    return;
            }

            // Define a Posição exata do jogador
            switch (Direction)
            {
                case Directions.Up: Y2 -= Speed; break;
                case Directions.Down: Y2 += Speed; break;
                case Directions.Right: X2 += Speed; break;
                case Directions.Left: X2 -= Speed; break;
            }

            // Verifica se não passou do limite
            if (x > 0 && X2 < 0) X2 = 0;
            if (x < 0 && X2 > 0) X2 = 0;
            if (y > 0 && Y2 < 0) Y2 = 0;
            if (y < 0 && Y2 > 0) Y2 = 0;

            // Alterar as animações somente quando necessário
            if (Direction == Directions.Right || Direction == Directions.Down)
            {
                if (X2 < 0 || Y2 < 0)
                    return;
            }
            else if (X2 > 0 || Y2 > 0)
                return;

            // Define as animações
            Movement = Movements.Stopped;
            if (Animation == Animation_Left)
                Animation = Animation_Right;
            else
                Animation = Animation_Left;
        }
    }
}