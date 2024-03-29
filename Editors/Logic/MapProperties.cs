﻿using System;
using System.ComponentModel;
using System.Drawing;
using CryBits.Client.Framework.Graphics;
using CryBits.Editors.Entities;
using CryBits.Entities.Map;
using CryBits.Enums;
using static CryBits.Globals;

namespace CryBits.Editors.Logic;

internal class MapProperties
{
    public MapProperties(Map map)
    {
        Base = map;
    }

    // Dados do NPC
    public readonly Map Base;

    /////////////
    // General //
    /////////////
    [Category("General")]
    public string Name
    {
        get => Base.Name;
        set => Base.Name = value;
    }

    [Category("General")][DefaultValue(0)]
    public Moral Moral
    {
        get => Base.Moral;
        set => Base.Moral = value;
    }

    /////////
    // Fog //
    /////////
    [Category("Fog")][DisplayName("Fog Texture")][DefaultValue(0)]
    public byte FogTexture
    {
        get => Base.Fog.Texture;
        set => Base.Fog.Texture = Math.Min(value, (byte)(Textures.Fogs.Count - 1));
    }

    [Category("Fog")][DisplayName("Fog Alpha")][DefaultValue(255)]
    public byte FogAlpha
    {
        get => Base.Fog.Alpha;
        set => Base.Fog.Alpha = value;
    }

    [Category("Fog")][DisplayName("Fog X Speed")][DefaultValue(0)]
    public sbyte FogSpeedX
    {
        get => Base.Fog.SpeedX;
        set => Base.Fog.SpeedX = value;
    }

    [Category("Fog")][DisplayName("Fog Y Speed")][DefaultValue(0)]
    public sbyte FogSpeedY
    {
        get => Base.Fog.SpeedY;
        set => Base.Fog.SpeedY = value;
    }

    /////////////
    // Weather //
    /////////////

    [Category("Weather")][DisplayName("Weather Intensity")][DefaultValue(0)]
    public byte WeatherSpeedY
    {
        get => Base.Weather.Intensity;
        set => Base.Weather.Intensity = Math.Min(value, MaxWeatherIntensity);
    }

    [Category("Weather")][DisplayName("Weather Type")][DefaultValue(0)]
    public Weather WeatherType
    {
        get => Base.Weather.Type;
        set
        {
            Base.Weather.Type = value;
            TempMap.UpdateWeatherType();
        }
    }

    /////////
    // Hue //
    /////////

    [Category("Hue Overlay")][DisplayName("Red Hue")][DefaultValue(255)]
    public byte HueRed
    {
        get => Base.Color.R;
        set => Base.Color = Color.FromArgb(Base.Color.A, value, Base.Color.G, Base.Color.B);
    }

    [Category("Hue Overlay")][DisplayName("Green Hue")][DefaultValue(255)]
    public byte HueGreen
    {
        get => Base.Color.G;
        set => Base.Color = Color.FromArgb(Base.Color.A, Base.Color.R, value, Base.Color.B);
    }

    [Category("Hue Overlay")][DisplayName("Blue Hue")][DefaultValue(255)]
    public byte HueBlue
    {
        get => Base.Color.B;
        set => Base.Color = Color.FromArgb(Base.Color.A, Base.Color.R, Base.Color.G, value);
    }

    //////////
    // Misc //
    //////////
    [Category("Misc")][DefaultValue(100)]
    public byte Lighting
    {
        get => Base.Lighting;
        set => Base.Lighting = value;
    }

    [Category("Misc")][DefaultValue(0)]
    public string Music
    {
        get => Base.Music;
        set => Base.Music = value;
    }

    [Category("Misc")][DefaultValue(0)]
    public byte Panorama
    {
        get => Base.Panorama;
        set => Base.Panorama = Math.Min(value, (byte)(Textures.Panoramas.Count - 1));
    }
}