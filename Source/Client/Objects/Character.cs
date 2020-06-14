namespace Objects
{
    class Character
    {
        // Geral
        public short[] Vital = new short[(byte)Utils.Vitals.Count];
        public byte X;
        public byte Y;
        public Utils.Directions Direction;
        public Utils.Movements Movement;
        public short X2;
        public short Y2;
        public byte Animation;
        public bool Attacking;
        public int Hurt;
        public int Attack_Timer;

        // Posição exata em que o personagem está
        public int Pixel_X => X * Utils.Grid + X2;
        public int Pixel_Y => Y * Utils.Grid + Y2;

        public void ProcessMovement()
        {
            byte Speed = 0;
            short x = X2, y = Y2;

            // Reseta a animação se necessário
            if (Animation == Utils.Animation_Stopped) Animation = Utils.Animation_Right;

            // Define a velocidade que o jogador se move
            switch (Movement)
            {
                case Utils.Movements.Walking: Speed = 2; break;
                case Utils.Movements.Moving: Speed = 3; break;
                case Utils.Movements.Stopped:
                    // Reseta os dados
                    X2 = 0;
                    Y2 = 0;
                    return;
            }

            // Define a Posição exata do jogador
            switch (Direction)
            {
                case Utils.Directions.Up: Y2 -= Speed; break;
                case Utils.Directions.Down: Y2 += Speed; break;
                case Utils.Directions.Right: X2 += Speed; break;
                case Utils.Directions.Left: X2 -= Speed; break;
            }

            // Verifica se não passou do limite
            if (x > 0 && X2 < 0) X2 = 0;
            if (x < 0 && X2 > 0) X2 = 0;
            if (y > 0 && Y2 < 0) Y2 = 0;
            if (y < 0 && Y2 > 0) Y2 = 0;

            // Alterar as animações somente quando necessário
            if (Direction == Utils.Directions.Right || Direction == Utils.Directions.Down)
            {
                if (X2 < 0 || Y2 < 0)
                    return;
            }
            else if (X2 > 0 || Y2 > 0)
                return;

            // Define as animações
            Movement = Utils.Movements.Stopped;
            if (Animation == Utils.Animation_Left)
                Animation = Utils.Animation_Right;
            else
                Animation = Utils.Animation_Left;
        }
    }
}