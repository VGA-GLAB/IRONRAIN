using System;
using UnityEngine;

/// <summary>音量変更の管理</summary>
public class Volume : IVolume
{
    private float _value = 1.0F;

    // 音量が変更された際の処理
    private event Action<float> _onVolumeChanged;

    /// <summary>外部で登録と解除を行うためのプロパティ</summary>
    public event Action<float> OnVolumeChanged
    {
        add => _onVolumeChanged += value;
        remove => _onVolumeChanged -= value;
    }

    // 量変更を通知する際に必要な最小の値の差
    private const float DIFF = 0.01F;

    /// <summary>音量を取得、設定をする</summary>
    public float Value
    {
        get => _value;
        set
        {
            // 0から1の値に制限する
            value = Mathf.Clamp01(value);

            // 音量変更が基準値よりも高ければvalueを更新する
            if (!(_value + DIFF < value) && !(_value - DIFF > value))
                return;
            
            _onVolumeChanged?.Invoke(value);
            _value = value;
        }
    }

    // クラスVolumeをfloat型に暗黙的に変換するオペレーター
    public static implicit operator float(Volume volume) => volume.Value;
}