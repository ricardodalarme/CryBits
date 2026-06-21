using CryBits.Client.Framework.Graphics;
using CryBits.Definitions;
using CryBits.Definitions.Maps;
using CryBits.Editors.Entities;
using PropertyModels.ComponentModel;
using System.ComponentModel;

namespace CryBits.Editors.Forms.Maps.Properties;

internal class MapProperties(Map map) : MiniReactiveObject
{
    public readonly Map Base = map;

    /////////////
    // General //
    /////////////
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

    /////////
    // Fog //
    /////////
    [Category("Fog")]
    [DisplayName("Fog Texture")]
    [DefaultValue(0)]
    public byte FogTexture
    {
        get => Base.Fog.Texture;
        set => Base.Fog.Texture = Math.Min(value, (byte)(Textures.Fogs.Count - 1));
    }

    [Category("Fog")]
    [DisplayName("Fog Alpha")]
    [DefaultValue(255)]
    public byte FogAlpha
    {
        get => Base.Fog.Alpha;
        set => Base.Fog.Alpha = value;
    }

    [Category("Fog")]
    [DisplayName("Fog X Speed")]
    [DefaultValue(0)]
    public sbyte FogSpeedX
    {
        get => Base.Fog.SpeedX;
        set => Base.Fog.SpeedX = value;
    }

    [Category("Fog")]
    [DisplayName("Fog Y Speed")]
    [DefaultValue(0)]
    public sbyte FogSpeedY
    {
        get => Base.Fog.SpeedY;
        set => Base.Fog.SpeedY = value;
    }

    /////////////
    // Weather //
    /////////////

    [Category("Weather")]
    [DisplayName("Weather Intensity")]
    [DefaultValue(0)]
    public byte WeatherIntensity
    {
        get => Base.Weather.Intensity;
        set => Base.Weather.Intensity = Math.Min(value, Globals.MaxWeatherIntensity);
    }

    [Category("Weather")]
    [DisplayName("Weather Type")]
    [DefaultValue(0)]
    public Weather WeatherType
    {
        get => Base.Weather.Type;
        set
        {
            Base.Weather.Type = value;
            MapInstance.Instance.UpdateWeatherType();
        }
    }

    /////////
    // Hue //
    /////////

    [Category("Hue Overlay")]
    [DisplayName("Red Hue")]
    [DefaultValue(255)]
    public byte HueRed
    {
        get => (byte)(Base.ColorArgb >> 16);
        set => Base.ColorArgb = (Base.ColorArgb & ~(0xFF << 16)) | (value << 16);
    }

    [Category("Hue Overlay")]
    [DisplayName("Green Hue")]
    [DefaultValue(255)]
    public byte HueGreen
    {
        get => (byte)(Base.ColorArgb >> 8);
        set => Base.ColorArgb = (Base.ColorArgb & ~(0xFF << 8)) | (value << 8);
    }

    [Category("Hue Overlay")]
    [DisplayName("Blue Hue")]
    [DefaultValue(255)]
    public byte HueBlue
    {
        get => (byte)Base.ColorArgb;
        set => Base.ColorArgb = (Base.ColorArgb & ~0xFF) | value;
    }

    //////////
    // Misc //
    //////////
    [Category("Misc")]
    [DefaultValue(100)]
    public byte Lighting
    {
        get => Base.Lighting;
        set => Base.Lighting = value;
    }

    [Category("Misc")]
    [DefaultValue(0)]
    public byte Panorama
    {
        get => Base.Panorama;
        set => Base.Panorama = Math.Min(value, (byte)(Textures.Panoramas.Count - 1));
    }
}
