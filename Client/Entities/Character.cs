using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Client.Entities;

internal abstract class Character
{
    // Geral
    public short[] Vital = new short[(byte)Enums.Vital.Count];
    public byte X;
    public byte Y;
    public Direction Direction;
    public Movement Movement;
    public short X2;
    public short Y2;
    public byte Animation;
    public bool Attacking;
    public int Hurt;
    public int AttackTimer;

    // Posição exata em que o personagem está
    public int PixelX => X * Grid + X2;
    public int PixelY => Y * Grid + Y2;

    protected void ProcessMovement()
    {
        byte speed = 0;
        short x = X2, y = Y2;

        // Reseta a animação se necessário
        if (Animation == AnimationStopped) Animation = AnimationRight;

        // Define a velocidade que o jogador se move
        switch (Movement)
        {
            case Movement.Walking: speed = 2; break;
            case Movement.Moving: speed = 3; break;
            case Movement.Stopped:
                // Reseta os dados
                X2 = 0;
                Y2 = 0;
                return;
        }

        // Define a Posição exata do jogador
        switch (Direction)
        {
            case Direction.Up: Y2 -= speed; break;
            case Direction.Down: Y2 += speed; break;
            case Direction.Right: X2 += speed; break;
            case Direction.Left: X2 -= speed; break;
        }

        // Verifica se não passou do limite
        if (x > 0 && X2 < 0) X2 = 0;
        if (x < 0 && X2 > 0) X2 = 0;
        if (y > 0 && Y2 < 0) Y2 = 0;
        if (y < 0 && Y2 > 0) Y2 = 0;

        // Alterar as animações somente quando necessário
        if (Direction == Direction.Right || Direction == Direction.Down)
        {
            if (X2 < 0 || Y2 < 0)
                return;
        }
        else if (X2 > 0 || Y2 > 0)
            return;

        // Define as animações
        Movement = Movement.Stopped;
        if (Animation == AnimationLeft)
            Animation = AnimationRight;
        else
            Animation = AnimationLeft;
    }
}