using CryBits.Definitions;

namespace CryBits.Editors.Maps;

public struct MapWeatherParticleInstance
{
    public bool Visible { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Speed { get; set; }
    public int Start { get; set; }
    public bool Back { get; set; }

    public void MoveRain()
    {
        // Move the particle for rain.
        X += Speed;
        Y += Speed;
    }

    public void SetRain()
    {
        // Initialize rain particle speed and start position.
        Speed = Random.Shared.Next(8, 13);

        if (Random.Shared.Next(2) == 0)
        {
            X = -32;
            Y = Random.Shared.Next(-32, Globals.ScreenHeight);
        }
        else
        {
            X = Random.Shared.Next(-32, Globals.ScreenWidth);
            Y = -32;
        }
    }

    public void SetSnow()
    {
        // Initialize snow particle.
        Speed = Random.Shared.Next(1, 3);
        Y = -32;
        X = Random.Shared.Next(-32, Globals.ScreenWidth);
        Start = X;
        Back = Random.Shared.Next(2) != 0;
    }

    public void MoveSnow(bool xAxis = true)
    {
        var difference = Random.Shared.Next(0, Globals.SnowMovement / 3);
        var x1 = Start + Globals.SnowMovement + difference;
        var x2 = Start - Globals.SnowMovement - difference;

        // Reverse horizontal direction when limits are reached
        if (x1 <= X)
            Back = true;
        else if (x2 >= X)
            Back = false;

        // Move particle vertically and optionally horizontally
        Y += Speed;

        if (xAxis)
            if (Back)
                X--;
            else
                X++;
    }
}
