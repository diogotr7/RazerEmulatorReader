using RazerSdkReader.Enums;
using RazerSdkReader.Extensions;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace RazerSdkReader.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaKeyboard : IColorProvider
{
    // ReSharper disable InconsistentNaming
    private const int WIDTH = 22;
    private const int HEIGHT = 6;
    private const int COUNT = WIDTH * HEIGHT;
    // ReSharper restore InconsistentNaming

    public readonly uint WriteIndex;
    private readonly int Padding;
    public readonly ChromaKeyboardData10 Data;
    public readonly ChromaDevice10 Device;

    public int Width => WIDTH;

    public int Height => HEIGHT;

    public int Count => COUNT;

    public readonly ChromaColor GetColor(int index)
    {
        if (index is < 0 or >= COUNT)
            throw new ArgumentOutOfRangeException(nameof(index));

        var targetIndex = WriteIndex.ToReadIndex();

        var snapshot = Data[targetIndex];
        
        var clr2 = snapshot.Effect.Custom2.Color[index];

        return ChromaEncryption.Decrypt(clr2, snapshot.Timestamp);

        if (snapshot.EffectType is not EffectType.Custom and not EffectType.CustomKey and not EffectType.Static)
            return default;

        ChromaColor clr = default;
        var staticColor = snapshot.Effect.Static.Color;

        if (snapshot.EffectType == EffectType.CustomKey)
        {
            clr = snapshot.Effect.Custom2.Key[index];

            //this next part is required for some effects to work properly.
            //For example, the chroma example app ambient effect.
            if (clr == staticColor)
                clr = snapshot.Effect.Custom2.Color[index];
        }
        else if (snapshot.EffectType is EffectType.Custom or EffectType.Static)
        {
            clr = snapshot.Effect.Custom.Color[index];
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

[InlineArray(192)]
public struct Color8X24
{
    public ChromaColor _field;
}

[InlineArray(132)]
public struct Color6X22
{
    public ChromaColor _field;
}

[InlineArray(10)]
public struct ChromaKeyboardData10
{
    public ChromaKeyboardData _field;
}