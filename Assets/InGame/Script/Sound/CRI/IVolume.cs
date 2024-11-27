using System;

/// <summary>音量変更プロパティ</summary>
public interface IVolume
{
    public event Action<float> OnVolumeChanged;

    public float Value { get; set; }

    public static IVolume operator +(IVolume volume, IVolume volume2) => volume;
}