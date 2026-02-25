namespace CryBits.Client.Components;

/// <summary>
/// Automatically cycles through frames on a sprite sheet.
/// </summary>
internal struct AnimatedSpriteComponent(int frameWidth, int frameHeight, float timePerFrame, int frameCount)
{
    public int FrameWidth = frameWidth;
    public int FrameHeight = frameHeight;
    public int FrameCount = frameCount;

    public float TimePerFrame = timePerFrame;
    public float Timer = 0f;

    public int CurrentFrameX = 0;
    public int CurrentFrameY = 0;

    public bool Playing = false;
}
