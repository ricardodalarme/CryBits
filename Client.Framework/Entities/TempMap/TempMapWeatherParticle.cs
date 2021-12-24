namespace CryBits.Client.Framework.Entities.TempMap;

public struct TempMapWeatherParticle
{
    public bool Visible { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Speed { get; set; }
    public int Start { get; set; }
    public bool Back { get; set; }

    public void MoveRain()
    {
        // Movimenta a partícula
        X += Speed;
        Y += Speed;
    }

    public void SetRain()
    {
        // Define a velocidade e a posição da partícula
        Speed = Utils.MyRandom.Next(8, 13);

        if (Utils.MyRandom.Next(2) == 0)
        {
            X = -32;
            Y = Utils.MyRandom.Next(-32, Globals.ScreenHeight);
        }
        else
        {
            X = Utils.MyRandom.Next(-32, Globals.ScreenWidth);
            Y = -32;
        }
    }

    public void SetSnow()
    {
        // Define a velocidade e a posição da partícula
        Speed = Utils.MyRandom.Next(1, 3);
        Y = -32;
        X = Utils.MyRandom.Next(-32, Globals.ScreenWidth);
        Start = X;
        Back = Utils.MyRandom.Next(2) != 0;
    }

    public void MoveSnow(bool xAxis = true)
    {
        var difference = Utils.MyRandom.Next(0, Globals.SnowMovement / 3);
        var x1 = Start + Globals.SnowMovement + difference;
        var x2 = Start - Globals.SnowMovement - difference;

        // Faz com que a partícula volte
        if (x1 <= X)
            Back = true;
        else if (x2 >= X)
            Back = false;

        // Movimenta a partícula
        Y += Speed;

        if (xAxis)
            if (Back)
                X--;
            else
                X++;
    }
}