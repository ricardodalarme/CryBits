using CryBits.Client.Framework.Graphics;
using CryBits.Entities.Map;

namespace CryBits.Client.Framework.Entities.TempMap;

public class TempMapFog(MapFog map)
{
    private MapFog Data { get; } = map;
    public int X { get; set; }
    public int Y { get; set; }

    private int _xTimer;
    private int _yTimer;

    public void Update()
    {
        if (Data.Texture == 0) return;

        // Update fog offsets.
        UpdateX();
        UpdateY();
    }

    private void UpdateX()
    {
        var size = Textures.Fogs[Data.Texture].ToSize();
        int speedX = Data.SpeedX;

        // Return early if not time or no horizontal speed.
        if (_xTimer >= Environment.TickCount) return;
        if (speedX == 0) return;

        // Move left when speed is negative.
        if (speedX < 0)
        {
            X--;
            if (X < -size.Width) X = 0;
        }
        // Move right when speed is positive.
        else
        {
            X++;
            if (X > size.Width) X = 0;
        }

        // Reset timer using absolute speed.
        if (speedX < 0) speedX *= -1;
        _xTimer = Environment.TickCount + 50 - speedX;
    }

    private void UpdateY()
    {
        var size = Textures.Fogs[Data.Texture].ToSize();
        int speedY = Data.SpeedY;

        // Return early if not time or no vertical speed.
        if (_yTimer >= Environment.TickCount) return;
        if (speedY == 0) return;

        // Move up when speed is negative.
        if (speedY < 0)
        {
            Y--;
            if (Y < -size.Height) Y = 0;
        }
        // Move down when speed is positive.
        else
        {
            Y++;
            if (Y > size.Height) Y = 0;
        }

        // Reset timer using absolute speed.
        if (speedY < 0) speedY *= -1;
        _yTimer = Environment.TickCount + 50 - speedY;
    }
}
