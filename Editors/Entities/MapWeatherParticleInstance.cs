using static CryBits.Utils.RandomUtils;

namespace CryBits.Editors.Entities;

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
        Speed = MyRandom.Next(8, 13);

        if (MyRandom.Next(2) == 0)
        {
            X = -32;
            Y = MyRandom.Next(-32, Globals.ScreenHeight);
        }
        else
        {
            X = MyRandom.Next(-32, Globals.ScreenWidth);
            Y = -32;
        }
    }

    public void SetSnow()
    {
        // Initialize snow particle.
        Speed = MyRandom.Next(1, 3);
        Y = -32;
        X = MyRandom.Next(-32, Globals.ScreenWidth);
        Start = X;
        Back = MyRandom.Next(2) != 0;
    }

    public void MoveSnow(bool xAxis = true)
    {
        var difference = MyRandom.Next(0, Globals.SnowMovement / 3);
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
