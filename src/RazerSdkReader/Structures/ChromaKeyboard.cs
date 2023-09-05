﻿using System;
using System.Runtime.InteropServices;
using RazerSdkReader.Enums;
using RazerSdkReader.Extensions;
using UnmanagedArrayGenerator;

namespace RazerSdkReader.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaKeyboard : IColorProvider
{
    public readonly int WriteIndex;
    private readonly int Padding;
    public readonly ChromaKeyboardData10 Data;
    public readonly ChromaDevice10 Device;

    public int Width => 22;

    public int Height => 6;

    public int Count => Width * Height;

    public ChromaColor GetColor(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var targetIndex = WriteIndex.ToReadIndex();

        var snapshot = Data.AsSpan()[targetIndex];

        if (snapshot.EffectType is not EffectType.Custom and not EffectType.CustomKey and not EffectType.Static)
            return default;

        ChromaColor clr = default;
        var staticColor = snapshot.Effect.Static.Color;

        if (snapshot.EffectType == EffectType.CustomKey)
        {
            clr = snapshot.Effect.Custom2.Key.AsSpan()[index];

            //this next part is required for some effects to work properly.
            //For example, the chroma example app ambient effect.
            if (clr == staticColor)
                clr = snapshot.Effect.Custom2.Color.AsSpan()[index];
        }
        else if (snapshot.EffectType is EffectType.Custom or EffectType.Static)
        {
            clr = snapshot.Effect.Custom.Color.AsSpan()[index];
        }

        return clr ^ staticColor;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaKeyboardData
{
    public readonly uint Flag;
    public readonly EffectType EffectType;
    public readonly KeyboardEffect Effect;
    private readonly uint Padding;
    public readonly ulong Timestamp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct KeyboardEffect
{
    public readonly Wave Wave;
    public readonly Breathing Breathing;
    public readonly Reactive Reactive;
    public readonly Starlight Starlight;
    public readonly Static Static;
    public readonly KeyboardCustom Custom;
    public readonly KeyboardCustom2 Custom2;
    public readonly KeyboardCustom3 Custom3;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct KeyboardCustom
{
    public readonly Color6X22 Color;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct KeyboardCustom2
{
    public readonly Color6X22 Color;
    public readonly Color6X22 Key;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct KeyboardCustom3
{
    public readonly Color8X24 Color;
    public readonly Color6X22 Key;
}

[UnmanagedArray(typeof(ChromaColor), 192)]
public readonly partial record struct Color8X24;

[UnmanagedArray(typeof(ChromaColor), 132)]
public readonly partial record struct Color6X22;

[UnmanagedArray(typeof(ChromaKeyboardData), 10)]
public readonly partial record struct ChromaKeyboardData10;