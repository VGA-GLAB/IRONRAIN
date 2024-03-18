using UnityEngine;
using System;

/// <summary>サウンドを管理するクラス</summary>
public class CriSoundManager : MonoBehaviour
{
    /// <summary>インスタンスを格納する変数</summary>
    private static CriSoundManager _instance = null;

    /// <summary>インスタンスのプロパティ</summary>
    public static CriSoundManager Instance { get { _instance ??= new CriSoundManager(); return _instance; } }

    /// <summary>コンストラクタ</summary>
    private CriSoundManager()
    {

    }

    /// <summary>ボリューム</summary>
    private Volume _masterVolume = default;

    /// <summary>ボリュームのインターフェース</summary>
    public interface IVolume
    {
        /// <summary>音量のプロパティ</summary>
        public float Value { get; set; }

        /// <summary>音量が変更された際のイベント</summary>
        public event Action<float> OnVolumeChanged;

        /// <summary>演算子のオーバーロード</summary>
        public static IVolume operator +(IVolume volume1, IVolume volume2) => volume1;
    }

    /// <summary>ボリュームのクラス</summary>
    private class Volume : IVolume
    {
        /// <summary>音量</summary>
        private float _value = 1.0f;

        /// <summary>音量のプロパティ</summary>
        public float Value
        {
            get => _value;
            set
            {
                value = Mathf.Clamp01(value);
                // 音量の変化量が閾値を超えていた場合
                if (_value + DIFF < value || _value - DIFF > value)
                {
                    _onVolumeChanged?.Invoke(value);
                    _value = value;
                }
            }
        }

        /// <summary>音量が変更された際のイベント</summary>
        private event Action<float> _onVolumeChanged = default;

        public event Action<float> OnVolumeChanged
        {
            add => OnVolumeChanged += value;
            remove => OnVolumeChanged -= value;
        }

        /// <summary>イベントが呼ばれる際の閾値</summary>
        private const float DIFF = 0.01F;

        /// <summary>暗黙的な演算子</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }
}