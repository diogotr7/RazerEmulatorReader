﻿using System;
using System.Runtime.InteropServices;
using RazerSdkReader.Enums;
using RazerSdkReader.Extensions;
using UnmanagedArrayGenerator;

namespace RazerSdkReader.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaHeadset : IColorProvider
{
    public readonly uint WriteIndex;
    private readonly int Padding;
    public readonly ChromaHeadsetData10 Data;
    public readonly ChromaDevice10 Device;

    public int Width => 5;

    public int Height => 1;

    public int Count => Width * Height;

    public ChromaColor GetColor(int index)
    {
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        var targetIndex = WriteIndex.ToReadIndex();
        var snapshot = Data.AsSpan()[targetIndex];

        if (snapshot.EffectType is not EffectType.Custom and not EffectType.Static)
            return default;

        var clr = snapshot.Effect.Custom.AsSpan()[index];
        var staticColor = snapshot.Effect.Static.Color;

        return clr ^ staticColor;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaHeadsetData
{
    public readonly uint Flag;
    public readonly EffectType EffectType;
    public readonly HeadsetEffect Effect;
    private readonly uint Padding;
    public readonly ulong Timestamp;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct HeadsetEffect
{
    public readonly Breathing Breathing;
    public readonly HeadsetCustom Custom;
    public readonly Static Static;
}

[UnmanagedArray(typeof(ChromaColor), 5)]
public readonly partial record struct HeadsetCustom;

[UnmanagedArray(typeof(ChromaHeadsetData), 10)]
public readonly partial record struct ChromaHeadsetData10;