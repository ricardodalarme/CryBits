using Entities;
using System;
using System.ComponentModel;
using System.Drawing;

namespace Logic
{
    class MapProperties
    {
        public MapProperties(Map Map)
        {
            Base = Map;
        }

        // Dados do NPC
        public Map Base;

        /////////////
        // General //
        /////////////
        [Category("General")]
        public string Name
        {
            get => Base.Name;
            set => Base.Name = value;
        }

        [Category("General"), DefaultValue(0)]
        public Map_Morals Moral
        {
            get => Base.Moral;
            set => Base.Moral = value;
        }

        /////////
        // Fog //
        /////////
        [Category("Fog"), DisplayName("Fog Texture"), DefaultValue(0)]
        public byte Fog_Texture
        {
            get => Base.Fog.Texture;
            set => Base.Fog.Texture = Math.Min(value, (byte)(Graphics.Tex_Fog.Length - 1));
        }

        [Category("Fog"), DisplayName("Fog Alpha"), DefaultValue(255)]
        public byte Fog_Alpha
        {
            get => Base.Fog.Alpha;
            set => Base.Fog.Alpha = value;
        }

        [Category("Fog"), DisplayName("Fog X Speed"), DefaultValue(0)]
        public sbyte Fog_SpeedX
        {
            get => Base.Fog.Speed_X;
            set => Base.Fog.Speed_X = value;
        }

        [Category("Fog"), DisplayName("Fog Y Speed"), DefaultValue(0)]
        public sbyte Fog_SpeedY
        {
            get => Base.Fog.Speed_Y;
            set => Base.Fog.Speed_Y = value;
        }

        /////////////
        // Weather //
        /////////////

        [Category("Weather"), DisplayName("Weather Intensity"), DefaultValue(0)]
        public byte Weather_SpeedY
        {
            get => Base.Weather.Intensity;
            set => Base.Weather.Intensity = Math.Max(value, Map.Max_Weather_Intensity);
        }

        [Category("Weather"), DisplayName("Weather Type"), DefaultValue(0)]
        public Weathers Weather_Type
        {
            get => Base.Weather.Type;
            set
            {
                Base.Weather.Type = value;
                Map.UpdateWeather();
            }
        }

        /////////
        // Hue //
        /////////

        [Category("Hue Overlay"), DisplayName("Red Hue"), DefaultValue(255)]
        public byte Hue_Red
        {
            get => Base.Color.R;
            set => Base.Color = Color.FromArgb(Base.Color.A, value, Base.Color.G, Base.Color.B);
        }

        [Category("Hue Overlay"), DisplayName("Green Hue"), DefaultValue(255)]
        public byte Hue_Green
        {
            get => Base.Color.G;
            set => Base.Color = Color.FromArgb(Base.Color.A, Base.Color.R, value, Base.Color.B);
        }

        [Category("Hue Overlay"), DisplayName("Blue Hue"), DefaultValue(255)]
        public byte Hue_Blue
        {
            get => Base.Color.B;
            set => Base.Color = Color.FromArgb(Base.Color.A, Base.Color.R, Base.Color.G, value);
        }

        //////////
        // Misc //
        //////////
        [Category("Misc"), DefaultValue(100)]
        public byte Lighting
        {
            get => Base.Lighting;
            set => Base.Lighting = value;
        }

        [Category("Misc"), DefaultValue(0)]
        public Audio.Musics Music
        {
            get => Base.Music;
            set => Base.Music = value;
        }

        [Category("Misc"), DefaultValue(0)]
        public byte Panorama
        {
            get => Base.Panorama;
            set => Base.Panorama = Math.Min(value, (byte)(Graphics.Tex_Panorama.Length - 1));
        }
    }
}