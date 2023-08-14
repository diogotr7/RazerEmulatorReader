﻿using System.Runtime.InteropServices;

namespace RazerSdkReader.Structures;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaAppData
{
    //Note: This can be interpreted as 51 ChromaAppData structs,
    // but the first one seems to only contain the PID of the current app, with the name being empty.
    
    //I'm not sure what the padding here or in ChromaAppInfo is.
    
    public readonly uint AppCount;
    public readonly ChromaString UnusedAppName;
    public readonly uint CurrentAppId;
    public readonly uint Padding;
    
    public readonly ChromaAppInfo50 AppInfo;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly record struct ChromaAppInfo
{
    public readonly ChromaString AppName;
    public readonly uint AppId;
    public readonly uint Padding;
}

[UnmanagedArray(typeof(ChromaAppInfo), 50)]
public readonly partial record struct ChromaAppInfo50;