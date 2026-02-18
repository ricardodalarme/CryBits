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
        
        // Faz a movimentação
        UpdateX();
        UpdateY();
    }

    private void UpdateX()
    {
        var size = Textures.Fogs[Data.Texture].ToSize();
        int speedX = Data.SpeedX;

        // Apenas se necessário
        if (_xTimer >= Environment.TickCount) return;
        if (speedX == 0) return;

        // Movimento para trás
        if (speedX < 0)
        {
            X--;
            if (X < -size.Width) X = 0;
        }
        // Movimento para frente
        else
        {
            X++;
            if (X > size.Width) X = 0;
        }

        // Contagem
        if (speedX < 0) speedX *= -1;
        _xTimer = Environment.TickCount + 50 - speedX;
    }

    private void UpdateY()
    {
        var size = Textures.Fogs[Data.Texture].ToSize();
        int speedY = Data.SpeedY;

        // Apenas se necessário
        if (_yTimer >= Environment.TickCount) return;
        if (speedY == 0) return;

        // Movimento para trás
        if (speedY < 0)
        {
            Y--;
            if (Y < -size.Height) Y = 0;
        }
        // Movimento para frente
        else
        {
            Y++;
            if (Y > size.Height) Y = 0;
        }

        // Contagem
        if (speedY < 0) speedY *= -1;
        _yTimer = Environment.TickCount + 50 - speedY;
    }
}