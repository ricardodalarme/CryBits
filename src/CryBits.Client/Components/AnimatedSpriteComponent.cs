namespace CryBits.Client.Components;

public sealed class AnimatedSpriteComponent
{
    public int FrameWidth { get; set; }
    public int FrameHeight { get; set; }
    public int FrameCount { get; set; }
    public float TimePerFrame { get; set; }
    public float Timer { get; set; }
    public int CurrentFrameX { get; set; }
    public int CurrentFrameY { get; set; }
    public bool Playing { get; set; }
}
