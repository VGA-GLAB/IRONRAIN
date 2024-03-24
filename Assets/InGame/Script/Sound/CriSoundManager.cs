using UnityEngine;
using System;
using CriWare;
using System.Collections.Concurrent;
using System.Threading;

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
        _masterVolume = new Volume();
    }

    /// <summary>�}�X�^�[�{�����[��</summary>
    private Volume _masterVolume = default;

    /// <summary>�{�����[���̃C���^�[�t�F�[�X</summary>
    public interface IVolume
    {
        /// <summary>���ʂ̃v���p�e�B</summary>
        public float Value { get; set; }

        /// <summary>���ʂ��ύX���ꂽ�ۂ̃C�x���g</summary>
        public event Action<float> OnVolumeChanged;
    }

    /// <summary>�{�����[���̃N���X</summary>
    private class Volume : IVolume
    {
        /// <summary>�{�����[��</summary>
        private float _value = 1.0f;

        /// <summary>�C�x���g���Ă΂��ۂ�臒l</summary>
        private const float DIFF = 0.01f;

        /// <summary>�{�����[���̃v���p�e�B</summary>
        public float Value
        {
            get => _value;
            set
            {
                // 0����1�̊Ԃɐ�������
                value = Mathf.Clamp01(value);
                // �{�����[���̕ω��ʂ�臒l�𒴂��Ă����ꍇ
                if (_value + DIFF < value || _value - DIFF > value)
                {
                    // �C�x���g�̌Ăяo��
                    _onVolumeChanged?.Invoke(value);
                    _value = value;
                }
            }
        }

        /// <summary>�{�����[�����ύX���ꂽ�ۂ̃C�x���g</summary>
        private event Action<float> _onVolumeChanged = default;

        public event Action<float> OnVolumeChanged
        {
            add => OnVolumeChanged += value;
            remove => OnVolumeChanged -= value;
        }

        /// <summary>�ÖٓI�ȉ��Z�q</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }

    /// <summary>Player�̃f�[�^���܂Ƃ߂��\����</summary>
    private struct CriPlayerData
    {
        /// <summary>�Đ����ꂽ�����𐧌䂷�邽�߂̃I�u�W�F�N�g</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>�L���[���</summary>
        public CriAtomEx.CueInfo CueInfo { get; set; }

        /// <summary>3D�������������߂̃I�u�W�F�N�g</summary>
        public CriAtomEx3dSource Source { get; set; }

        /// <summary>�Ō�ɃA�b�v�f�[�g���s��ꂽ����</summary>
        public float LastUpdateTime { get; set; }

        /// <summary>�L���[�����[�v���Ă��邩</summary>
        public bool IsLoop => CueInfo.length < 0;

        /// <summary>�L�����Z������</summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>�i�s�����̃x�N�g�����v�Z�E�ݒ肷��</summary>
        /// <param name="nextPos">���̃|�W�V����</param>
        public void UpdateCurrentVector(Vector3 nextPos)
        {
            // �O��̃A�b�v�f�[�g����̌o�ߎ���
            var delta = Playback.GetTime() - LastUpdateTime;

            // ���݂̃|�W�V�������擾����
            CriAtomEx.NativeVector nativePos = Source.GetPosition();

            // NativeVector��Vector3�ɕϊ�����
            Vector3 currentPos = new Vector3(nativePos.x, nativePos.y, nativePos.z);

            // �i�s�����̃x�N�g�����v�Z����
            Vector3 moveVec = nextPos - currentPos;

            // 1�b������iCriAtomEx3dSource.SetVelocity�̈����j�̈ړ���
            moveVec /= delta;

            // �A�b�v�f�[�g���Ԃ��X�V
            LastUpdateTime = Playback.GetTime();

            // �|�W�V�����E�x�N�g����ݒ肷��
            Source.SetPosition(nextPos.x, nextPos.y, nextPos.z);
            Source.SetVelocity(moveVec.x, moveVec.y, moveVec.z);

            // �|�W�V�����E�x�N�g�����X�V����
            Source.Update();
        }
    }

    /// <summary>�`�����l�����쐬���邽�߂ɕK�v�ȏ����܂Ƃ߂��N���X</summary>
    private abstract class AbstractCriChannel
    {
        /// <summary>�������Đ����邽�߂̃v���C���[</summary>
        protected CriAtomExPlayer _player = new CriAtomExPlayer();

        /// <summary>�L���[�̃f�[�^</summary>
        protected ConcurrentDictionary<int, CriPlayerData> _cueData = new ConcurrentDictionary<int, CriPlayerData>();

        /// <summary>�L���[�f�[�^�̃J�E���g�̍ő吔</summary>
        protected int _maxCueCount = 0;

        /// <summary>�폜���ꂽ�L���[�f�[�^�̃C���f�b�N�X���i�[����R���N�V����</summary>
        protected ConcurrentBag<int> _removedCueDataIndex = new ConcurrentBag<int>();

        /// <summary>3D���X�i�[���������߂̃I�u�W�F�N�g</summary>
        protected CriAtomEx3dListener _listener = default;

        /// <summary>�{�����[��</summary>
        protected Volume _volume = new Volume();

        /// <summary>�}�X�^�[�{�����[��</summary>
        protected Volume _masterVolume = null;

        /// <summary>�L�����Z������</summary>
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>�R���X�g���N�^</summary>
        protected AbstractCriChannel(in Volume masterVolume)
        {
            // �}�X�^�[�{�����[����ݒ�
            _masterVolume = masterVolume;

            // �{�����[�����ύX���ꂽ�ۂ̃C�x���g��o�^
            _volume.OnVolumeChanged += UpdateVolume;
            _masterVolume.OnVolumeChanged += UpdateMasterVolume;
        }

        /// <summary>�f�X�g���N�^</summary>
        ~AbstractCriChannel()
        {
            // �L�����Z�����������s
            _tokenSource.Cancel();

            // �{�����[�����ύX���ꂽ�ۂ̃C�x���g���폜
            _volume.OnVolumeChanged -= UpdateVolume;
            _masterVolume.OnVolumeChanged -= UpdateMasterVolume;

            // �v���C���[��j��
            _player.Dispose();

            // �L���[�̃f�[�^��j��
            foreach (var data in _cueData)
            {
                data.Value.CancellationTokenSource.Cancel();
                data.Value.Source.Dispose();
            }
        }

        /// <summary>�{�����[�����X�V����</summary>
        /// <param name="volume">�{�����[���̃v���p�e�B�ɃZ�b�g�����l</param>
        private void UpdateVolume(float volume)
        {
            // �v���C���[�̃{�����[����ݒ�
            _player.SetVolume(volume * _masterVolume);

            // �ݒ肵���{�����[�����Đ����̉����ɂ����f
            foreach (var data in _cueData)
            {
                _player.Update(data.Value.Playback);
            }
        }

        /// <summary>�}�X�^�[�{�����[�����X�V����</summary>
        /// <param name="masterVolume">�{�����[���̃v���p�e�B�ɃZ�b�g�����l</param>
        private void UpdateMasterVolume(float masterVolume)
        {
            // �v���C���[�̃{�����[����ݒ�
            _player.SetVolume(_volume * masterVolume);

            // �ݒ肵���{�����[�����Đ����̉����ɂ����f
            foreach (var data in _cueData)
            {
                _player.Update(data.Value.Playback);
            }
        }

        /// <summary></summary>
        /// <param name="playerData">Player�̃f�[�^</param>
        /// <returns></returns>
        protected int AddCueData(CriPlayerData playerData)
        {
            // �L���[�����[�v���Ă���ꍇ
            if (playerData.IsLoop)
            {
                // �폜���ꂽ�L���[�f�[�^��1�ȏ゠��ꍇ
                if (_removedCueDataIndex.Count > 0)
                {
                    // 
                    int tempIndex;

                    // 
                    if (_removedCueDataIndex.TryTake(out tempIndex))
                    {
                        // �L�[�ƒl�̃y�A��ǉ�
                        _cueData.TryAdd(tempIndex, playerData);
                    }

                    return tempIndex;
                }

                // �폜���ꂽ�L���[�f�[�^�����݂��Ȃ��ꍇ
                else
                {
                    //
                    _maxCueCount++;

                    // �L�[�ƒl�̃y�A��ǉ�
                    _cueData.TryAdd(_maxCueCount, playerData);
                    return _maxCueCount;
                }
            }

            //
            else if (_removedCueDataIndex.Count > 0)
            {
                int tempIndex;
                if (_removedCueDataIndex.TryTake(out tempIndex))
                {
                    _cueData.TryAdd(tempIndex, playerData);
                }

                PlaybackDestroyWaitForPlayEnd(tempIndex, playerData.CancellationTokenSource.Token);
                return tempIndex;
            }
            else
            {
                _currentMaxCount++;
                _cueData.TryAdd(_currentMaxCount, playerData);

                PlaybackDestroyWaitForPlayEnd(_currentMaxCount, playerData.CancellationTokenSource.Token);
                return _currentMaxCount;
            }
        }
    }
}