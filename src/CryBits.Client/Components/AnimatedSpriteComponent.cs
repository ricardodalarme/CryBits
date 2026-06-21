namespace CryBits.Client.Components;

public sealed record class AnimatedSpriteComponent(int FrameWidth, int FrameHeight, int FrameCount, float TimePerFrame, float Timer, int CurrentFrameX, int CurrentFrameY, bool Playing);
