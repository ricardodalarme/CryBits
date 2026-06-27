namespace CryBits.Client.Components;

public enum CharacterAnimation : byte
{
    Idle = 0,
    Walk = 1,
    Attack = 2
}

public sealed record class AnimationState(int FrameX, int FrameY, float Timer, CharacterAnimation Current);
