using CryBits.Client.Framework.Graphics;
using CryBits.Definitions.Maps;
using CryBits.Editors.Entities;
using PropertyModels.ComponentModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CryBits.Editors.Forms.Maps.Models;

internal class MapProperties(Map map) : MiniReactiveObject
{
    public readonly Map Base = map;

    [Category("General")]
    public string Name
    {
        get => Base.Name;
        set => Base.Name = value;
    }

    [Category("General")]
    [DefaultValue(0)]
    public Moral Moral
    {
        get => Base.Moral;
        set => Base.Moral = value;
    }

    [Category("General")]
    [DefaultValue("")]
    public string Music
    {
        get => Base.Music;
        set => Base.Music = value;
    }

    [Category("Fog")]
    [DisplayName("Fog Texture")]
    [DefaultValue(0)]
    public byte FogTexture
    {
        get => Base.DefaultFog?.Texture ?? 0;
        set
        {
            Base.DefaultFog ??= new FogConfig();
            Base.DefaultFog = Base.DefaultFog with { Texture = Math.Min(value, (byte)(Textures.Fogs.Count - 1)) };
        }
    }

    [Category("Fog")]
    [DisplayName("Fog Alpha")]
    [DefaultValue(255)]
    [Trackable(0, 255)]
    public byte FogAlpha
    {
        get => Base.DefaultFog?.Alpha ?? 255;
        set
        {
            Base.DefaultFog ??= new FogConfig();
            Base.DefaultFog = Base.DefaultFog with { Alpha = value };
        }
    }

    [Category("Fog")]
    [DisplayName("Fog X Speed")]
    [DefaultValue(0)]
    [Trackable(-128, 127)]
    public sbyte FogSpeedX
    {
        get => Base.DefaultFog?.SpeedX ?? 0;
        set
        {
            Base.DefaultFog ??= new FogConfig();
            Base.DefaultFog = Base.DefaultFog with { SpeedX = value };
        }
    }

    [Category("Fog")]
    [DisplayName("Fog Y Speed")]
    [DefaultValue(0)]
    [Trackable(-128, 127)]
    public sbyte FogSpeedY
    {
        get => Base.DefaultFog?.SpeedY ?? 0;
        set
        {
            Base.DefaultFog ??= new FogConfig();
            Base.DefaultFog = Base.DefaultFog with { SpeedY = value };
        }
    }

    [Browsable(false)]
    public byte WeatherIntensity
    {
        get => 0;
        set { }
    }

    [Category("Weather")]
    [DisplayName("Weather Type")]
    [DefaultValue(0)]
    public WeatherType WeatherTypeProp
    {
        get => Base.DefaultWeather;
        set
        {
            Base.DefaultWeather = value;
            MapInstance.Instance?.UpdateWeatherType();
        }
    }

    [Category("Hue Overlay")]
    [DisplayName("Red Hue")]
    [DefaultValue(255)]
    [Trackable(0, 255)]
    public byte HueRed
    {
        get => (byte)(Base.ColorArgb >> 16);
        set => Base.ColorArgb = (Base.ColorArgb & ~(0xFF << 16)) | (value << 16);
    }

    [Category("Hue Overlay")]
    [DisplayName("Green Hue")]
    [DefaultValue(255)]
    [Trackable(0, 255)]
    public byte HueGreen
    {
        get => (byte)(Base.ColorArgb >> 8);
        set => Base.ColorArgb = (Base.ColorArgb & ~(0xFF << 8)) | (value << 8);
    }

    [Category("Hue Overlay")]
    [DisplayName("Blue Hue")]
    [DefaultValue(255)]
    [Trackable(0, 255)]
    public byte HueBlue
    {
        get => (byte)Base.ColorArgb;
        set => Base.ColorArgb = (Base.ColorArgb & ~0xFF) | value;
    }

    [Category("Misc")]
    [DefaultValue(100)]
    [Trackable(0, 255)]
    [Range(0, 255)]
    public byte Lighting
    {
        get => Base.DefaultLighting;
        set => Base.DefaultLighting = value;
    }

    [Category("Misc")]
    [DefaultValue(0)]
    public byte Panorama
    {
        get => Base.Panorama;
        set => Base.Panorama = Math.Min(value, (byte)(Textures.Panoramas.Count - 1));
    }
}
