namespace Objects
{
    class Character
    {
        // Geral
        public short[] Vital = new short[(byte)Game.Vitals.Count];
        public byte X;
        public byte Y;
        public Game.Directions Direction;
        public Game.Movements Movement;
        public short X2;
        public short Y2;
        public byte Animation;
        public bool Attacking;
        public int Hurt;
        public int Attack_Timer;

        // Posição exata em que o personagem está
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
}