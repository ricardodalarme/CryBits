using Objects;
using System;
using System.Collections.Generic;

class Lists
{
    // Armazenamento de dados
    public static Structures.Options Options = new Structures.Options();
    public static Structures.Character[] Characters;
    public static Structures.Weather[] Weather;

    // Obtém o ID de algum dado, caso ele não existir retorna um ID zerado
    public static string GetID(Data Object) => Object == null ? Guid.Empty.ToString() : Object.ID.ToString();

    // Estrutura dos itens em gerais
    public class Structures
    {
        [Serializable]
        public struct Options
        {
            public string Game_Name;
            public bool SaveUsername;
            public bool Sounds;
            public bool Musics;
            public bool Chat;
            public bool FPS;
            public bool Latency;
            public bool Party;
            public bool Trade;
            public string Username;
        }

        public class Character
        {
            public string Name;
            public short Level;
            public short Texture_Num;
        }

        public struct Weather
        {
            public bool Visible;
            public int x;
            public int y;
            public int Speed;
            public int Start;
            public bool Back;
        }
    }
}