using UnityEngine;
using System;

/// <summary>�T�E���h���Ǘ�����N���X</summary>
public class CriSoundManager : MonoBehaviour
{
    /// <summary>�C���X�^���X���i�[����ϐ�</summary>
    private static CriSoundManager _instance = null;

    /// <summary>�C���X�^���X�̃v���p�e�B</summary>
    public static CriSoundManager Instance { get { _instance ??= new CriSoundManager(); return _instance; } }

    /// <summary>�R���X�g���N�^</summary>
    private CriSoundManager()
    {

    }

    /// <summary>�{�����[��</summary>
    private Volume _masterVolume = default;

    /// <summary>�{�����[���̃C���^�[�t�F�[�X</summary>
    public interface IVolume
    {
        /// <summary>���ʂ̃v���p�e�B</summary>
        public float Value { get; set; }

        /// <summary>���ʂ��ύX���ꂽ�ۂ̃C�x���g</summary>
        public event Action<float> OnVolumeChanged;

        /// <summary>���Z�q�̃I�[�o�[���[�h</summary>
        public static IVolume operator +(IVolume volume1, IVolume volume2) => volume1;
    }

    /// <summary>�{�����[���̃N���X</summary>
    private class Volume : IVolume
    {
        /// <summary>����</summary>
        private float _value = 1.0f;

        /// <summary>���ʂ̃v���p�e�B</summary>
        public float Value
        {
            get => _value;
            set
            {
                value = Mathf.Clamp01(value);
                // ���ʂ̕ω��ʂ�臒l�𒴂��Ă����ꍇ
                if (_value + DIFF < value || _value - DIFF > value)
                {
                    _onVolumeChanged?.Invoke(value);
                    _value = value;
                }
            }
        }

        /// <summary>���ʂ��ύX���ꂽ�ۂ̃C�x���g</summary>
        private event Action<float> _onVolumeChanged = default;

        public event Action<float> OnVolumeChanged
        {
            add => OnVolumeChanged += value;
            remove => OnVolumeChanged -= value;
        }

        /// <summary>�C�x���g���Ă΂��ۂ�臒l</summary>
        private const float DIFF = 0.01F;

        /// <summary>�ÖٓI�ȉ��Z�q</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }
}