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
        /// <summary>音量が変更された際の処理</summary>
        public event Action<float> OnVolumeChanged;

        /// <summary>演算子のオーバーロード</summary>
        public static IVolume operator +(IVolume volume1, IVolume volume2) => volume1;
    }

    /// <summary>ボリュームのクラス</summary>
    private class Volume : IVolume
    {
        /// <summary>音量</summary>
        private float _value = 1.0f;

        /// <summary>音量が変更された際の処理</summary>
        private event Action<float> _onVolumeChanged = default;

        public event Action<float> OnVolumeChanged
        {
            add => OnVolumeChanged += value;
            remove => OnVolumeChanged -= value;
        }

        /// <summary>暗黙的な演算子</summary>
        public float Value
        {
            get => _value;
            set
            {
                value = Mathf.Clamp01(value);


            }
        }

        /// <summary>暗黙的な演算子</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }
}