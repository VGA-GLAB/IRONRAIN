#if CRI_SOUND_MANAGER
using UnityEngine;
using System;
using CriWare;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Generic;

/// <summary>�T�E���h���Ǘ�����N���X</summary>
public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    private void Start()
    {
        _masterVolume = new Volume();
        _bgm = new CriSingleChannel(_masterVolume);
        _se = new CriMultiChannel(_masterVolume);
    }

    /// <summary>�}�X�^�[�{�����[��</summary>
    private Volume _masterVolume = default;

    /// <summary>BGM�𗬂��`�����l��</summary>
    private CriSingleChannel _bgm = default;

    /// <summary>SE�𗬂��`�����l��</summary>
    private CriMultiChannel _se = default;

    /// <summary>�}�X�^�[�̃{�����[��</summary>
    public IVolume MasterVolume => _masterVolume;

    /// <summary>BGM�̃`�����l��</summary>
    public ICustomChannel BGM => _bgm;

    /// <summary>SE�̃`�����l��</summary>
    public ICustomChannel SE => _se;

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
            add => _onVolumeChanged += value;
            remove => _onVolumeChanged -= value;
        }

        /// <summary>�ÖٓI�ȉ��Z�q</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }

    /// <summary>Player�̃f�[�^���܂Ƃ߂��\����</summary>
    private struct CriPlayerData
    {
        /// <summary>�Đ����ꂽ�����𐧌䂷�邽�߂̃I�u�W�F�N�g</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>�Đ����ꂽ�����𐧌䂷�邽�߂̃I�u�W�F�N�g</summary>
        public CriAtomCustomStruct CustomStruct { get; set; }

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

        /// <summary>�v���C���[���̃X���b�h�Z�[�t�ȃR���N�V����</summary>
        protected ConcurrentDictionary<int, CriPlayerData> _cueData = new ConcurrentDictionary<int, CriPlayerData>();

        /// <summary>�L���[�f�[�^�̃J�E���g�̍ő吔</summary>
        protected int _maxCueCount = 0;

        /// <summary>�폜���ꂽ�v���C���[�f�[�^�̃C���f�b�N�X���i�[����R���N�V����</summary>
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
        /// <param name="masterVolume">�}�X�^�[�{�����[���i���͐�p�j</param>
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

            // �v���C���[�̃f�[�^��j��
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

        /// <summary>�Đ����I������̂�҂��Ă���Playback���폜����</summary>
        /// <param name="index">�L���[�f�[�^�̃C���f�b�N�X</param>
        protected async void DestroyPlaybackAfterPlayEnd(int index)
        {
            // �L���[�����[�v���Ă����甲����
            if (_cueData[index].IsLoop)
            {
                return;
            }

            // �w�肵���~���b�i�L���[�̒����j��Ɋ�������L�����Z���\�ȃ^�X�N���쐬
            await Task.Delay((int)_cueData[index].CueInfo.length, _cueData[index].CancellationTokenSource.Token);

            while (true)
            {
                // �Đ������������ꍇ && �w�肵���L�[�����l������ɍ폜���ꂽ�ꍇ
                if (_cueData[index].Playback.GetStatus() == CriAtomExPlayback.Status.Removed && _cueData.TryRemove(index, out CriPlayerData deletedData))
                {
                    // �C���f�b�N�X���R���N�V�����ɒǉ�
                    _removedCueDataIndex.Add(index);

                    // �폜���ꂽ�v���C���[�̃f�[�^��j��
                    deletedData.Source?.Dispose();

                    return;
                }

                // �Đ����̏ꍇ
                else
                {
                    // �w�肵�����Ԃ̌�Ɋ�������L�����Z���\�ȃ^�X�N���쐬
                    await Task.Delay(TimeSpan.FromSeconds(0.05D), _cueData[index].CancellationTokenSource.Token);
                }
            }
        }

        /// <summary></summary>
        /// <param name="playerData">Player�̃f�[�^</param>
        protected int AddCueData(CriPlayerData playerData)
        {
            // �L���[�����[�v���Ă���ꍇ
            if (playerData.IsLoop)
            {
                // �폜���ꂽ�L���[�f�[�^��1�ȏ゠��ꍇ
                if (_removedCueDataIndex.Count > 0)
                {
                    // �ϐ���������
                    int tempIndex;

                    //  �R���N�V��������I�u�W�F�N�g���폜�ł���ꍇ
                    if (_removedCueDataIndex.TryTake(out tempIndex))
                    {
                        // �L�[��Player�f�[�^�̃y�A��ǉ�
                        _cueData.TryAdd(tempIndex, playerData);
                    }

                    return tempIndex;
                }

                // �폜���ꂽ�L���[�f�[�^�����݂��Ȃ��ꍇ
                else
                {
                    // �L���[�f�[�^�̃J�E���g�𑝉�
                    _maxCueCount++;

                    // �L�[��Player�f�[�^�̃y�A��ǉ�
                    _cueData.TryAdd(_maxCueCount, playerData);

                    return _maxCueCount;
                }
            }

            // �L���[�����[�v���Ă��Ȃ��ꍇ && �폜���ꂽ�L���[�f�[�^��1�ȏ゠��ꍇ
            if (_removedCueDataIndex.Count > 0)
            {
                // �ϐ���������
                int tempIndex;

                // �R���N�V��������I�u�W�F�N�g���폜�ł���ꍇ
                if (_removedCueDataIndex.TryTake(out tempIndex))
                {
                    // �L�[��Player�f�[�^�̃y�A��ǉ�
                    _cueData.TryAdd(tempIndex, playerData);
                }

                DestroyPlaybackAfterPlayEnd(tempIndex);
                return tempIndex;
            }
            // �L���[�����[�v���Ă��Ȃ��ꍇ && �폜���ꂽ�L���[�f�[�^�����݂��Ȃ��ꍇ
            else
            {
                // �L���[�f�[�^�̃J�E���g�𑝉�
                _maxCueCount++;

                // �L�[��Player�f�[�^�̃y�A��ǉ�
                _cueData.TryAdd(_maxCueCount, playerData);

                DestroyPlaybackAfterPlayEnd(_maxCueCount);
                return _maxCueCount;
            }
        }
    }

    /// <summary>���y���Ǘ����邽�߂̋@�\���������C���^�[�t�F�[�X</summary>
    public interface ICustomChannel
    {
        /// <summary>�{�����[��</summary>
        public IVolume Volume { get; }

        /// <summary>���y�𗬂��֐�</summary>
        /// <param name="cueSheetName">���������L���[�V�[�g�̖��O</param>
        /// <param name="cueName">���������L���[�̖��O</param>
        /// <param name="volume">�{�����[��</param>
        /// <returns>���삷��ۂɕK�v��Index</returns>
        public int Play(string cueSheetName, string cueName, float volume = 1.0F);

        /// <summary>���y�𗬂��֐�(3D)</summary>
        /// <param name="playSoundWorldPos">����Position��WorldSpace</param>
        /// <param name="cueSheetName">���������L���[�V�[�g�̖��O</param>
        /// <param name="cueName">���������L���[�̖��O</param>
        /// <param name="volume">�{�����[��</param>
        /// <returns>���삷��ۂɕK�v��Index</returns>
        public int Play3D(Vector3 playSoundWorldPos, string cueSheetName, string cueName, float volume = 1.0F);

        /// <summary>3D�̗���Position���X�V����</summary>
        /// <param name="playSoundWorldPos">�X�V����Position</param>
        /// <param name="index">�ύX���鉹����Play���̖߂�l(Index)</param>
        public void Update3DPos(Vector3 playSoundWorldPos, int index);

        /// <summary>������Pause������</summary>
        /// <param name="index">Pause��������������Play���̖߂�l(Index)</param>
        public void Pause(int index);

        /// <summary>Pause������������Resume������</summary>
        /// <param name="index">Resume��������������Play���̖߂�l(Index)</param>
        public void Resume(int index);

        /// <summary>������Stop������</summary>
        /// <param name="index">Stop��������������Play���̖߂�l(Index)</param>
        public void Stop(int index);

        /// <summary>���ׂẲ�����Stop������</summary>
        public void StopAll();

        /// <summary>���[�v���Ă��鉹�����ׂĂ�Stop������</summary>
        public void StopLoopCue();

        /// <summary>���ׂẴ��X�i�[��ݒ肷��</summary>
        /// <param name="listener">���X�i�[</param>
        public void SetListenerAll(CriAtomListener listener);

        /// <summary>���X�i�[��ݒ肷��</summary>
        /// <param name="listener">���X�i�[</param>
        /// <param name="index">���X�i�[��ύX������������Play���̖߂�l</param>
        public void SetListener(CriAtomListener listener, int index);
    }

    /// <summary>BGM�ȂǂɎg�p����A��̉��݂̂��o�͂���`�����l��</summary>
    private class CriSingleChannel : AbstractCriChannel, ICustomChannel
    {
        public CriAtomExPlayer _test = new CriAtomExPlayer();
        /// <summary>���ݍĐ�����Acb</summary>
        private CriAtomExAcb _currentAcb = null;

        /// <summary>���ݍĐ�����CueName</summary>
        private string _currentCueName = "";

        /// <summary>�R���X�g���N�^�|</summary>
        /// <param name="masterVolume">�}�X�^�[�{�����[��</param>
        public CriSingleChannel(Volume masterVolume) : base(masterVolume)
        {
            // TODO - Add�Ɏ��s�������ۂ̏�����ǉ�����
            _cueData.TryAdd(0, new CriPlayerData());
        }

        public IVolume Volume => _volume;

        public int Play(string cueSheetName, string cueName, float volume = 1.0F)
        {
            // CueSheet��������擾
            var tempAcb = CriAtom.GetAcb(cueSheetName);
            var tempPlayerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out CriAtomEx.CueInfo tempInfo);
            tempPlayerData.CueInfo = tempInfo;

            if (_currentAcb == tempAcb && _currentCueName == cueName
                                       && _player.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                // �C���f�b�N�X��Ԃ�
                return _cueData.Count - 1;
            }

            Stop(_cueData.Count - 1);

            // �����Z�b�g���čĐ�
            _player.SetCue(tempAcb, cueName);
            _player.SetVolume(_volume * _masterVolume * volume);
            _player.Set3dSource(null);
            _player.SetStartTime(0L);
            tempPlayerData.Playback = _player.Start();

            _cueData[_cueData.Count - 1] = tempPlayerData;

            return _cueData.Count - 1;
        }

        public int Play3D(Vector3 playSoundWorldPos, string cueSheetName, string cueName, float volume = 1.0F)
        {
            // CueSheet��������擾
            var tempAcb = CriAtom.GetAcb(cueSheetName);
            var tempPlayerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out CriAtomEx.CueInfo tempInfo);
            tempPlayerData.CueInfo = tempInfo;

            if (_currentAcb == tempAcb && _currentCueName == cueName
                                       && _player.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                return _cueData.Count - 1;
            }

            Stop(_cueData.Count - 1);

            // ���W�����Z�b�g���čĐ�
            var temp3dData = new CriAtomEx3dSource();

            temp3dData.SetPosition(playSoundWorldPos.x, playSoundWorldPos.y, playSoundWorldPos.z);
            // ���X�i�[�ƃ\�[�X��ݒ�
            _player.Set3dListener(_listener);
            _player.Set3dSource(temp3dData);
            tempPlayerData.Source = temp3dData;
            _player.SetCue(tempAcb, cueName);
            _player.SetVolume(_volume * _masterVolume * volume);
            _player.SetStartTime(0L);
            tempPlayerData.Playback = _player.Start();

            _cueData[_cueData.Count - 1] = tempPlayerData;

            return _cueData.Count - 1;
        }

        public void Update3DPos(Vector3 playSoundWorldPos, int index)
        {
            if (index <= -1 || _cueData[index].Source == null) return;

            _cueData[index].UpdateCurrentVector(playSoundWorldPos);
        }

        public void Pause(int index)
        {
            if (index <= -1) return;

            _player.Pause();
        }

        public void Resume(int index)
        {
            if (index <= -1) return;

            _player.Resume(CriAtomEx.ResumeMode.PausedPlayback);
        }

        public void Stop(int index)
        {
            if (index <= -1) return;

            _player.Stop(false);
        }

        public void StopAll()
        {
            _player.Stop(false);
        }

        public void StopLoopCue()
        {
            _player.Stop(false);
        }

        public void SetListenerAll(CriAtomListener listener)
        {
            _player.Set3dListener(listener.nativeListener);
            _player.UpdateAll();
        }

        public void SetListener(CriAtomListener listener, int index)
        {
            if (_cueData[index].Playback.GetStatus() == CriAtomExPlayback.Status.Removed || index <= -1) return;

            _player.Set3dListener(listener.nativeListener);
            _player.Update(_cueData[index].Playback);
        }

        /// <summary>�J�X�^���\���̂�Ԃ����\�b�h</summary>
        /// <param name="playerData">�J�X�^���\���̂�ێ����Ă���v���C���[���</param>
        public CriAtomCustomStruct GetCustomStruct(CriPlayerData playerData) => playerData.CustomStruct;

        /// <summary>�Đ��I����҂iUnitask�j</summary>
        public async UniTask WaitPlayingEnd(CancellationToken token, CriAtomCustomStruct customStruct)
        {
            // True��Ԃ��܂ő҂�
            await UniTask.WaitUntil(customStruct.CheckPlayingEnd, default, token, false);
        }

        /// <summary>�Đ��I����҂iCoroutine�j</summary>
        public System.Collections.IEnumerator WaitPlayingEndCor(CriAtomCustomStruct customStruct)
        {
            // True��Ԃ��܂ő҂�
            yield return new WaitUntil(customStruct.CheckPlayingEnd);
        }
    }

    /// <summary>SE�ȂǂɎg�p����A�����̉����o�͂���`�����l��</summary>
    private class CriMultiChannel : AbstractCriChannel, ICustomChannel
    {
        public CriMultiChannel(in Volume masterVolume) : base(in masterVolume)
        {

        }

        public IVolume Volume => _volume;

        public int Play(string cueSheetName, string cueName, float volume)
        {
            if (cueName == "") return -1;

            CriAtomEx.CueInfo cueInfo;
            CriPlayerData newAtomPlayer = new CriPlayerData();

            var tempAcb = CriAtom.GetAcb(cueSheetName);
            tempAcb.GetCueInfo(cueName, out cueInfo);

            newAtomPlayer.CueInfo = cueInfo;

            _player.SetCue(tempAcb, cueName);
            _player.Set3dSource(null);
            _player.SetVolume(volume * _volume * _masterVolume);
            newAtomPlayer.Playback = _player.Start();
            newAtomPlayer.CancellationTokenSource = new CancellationTokenSource();

            return AddCueData(newAtomPlayer);
        }

        public int Play3D(Vector3 playSoundWorldPos, string cueSheetName, string cueName, float volume)
        {
            // CueSheet��������擾
            var tempAcb = CriAtom.GetAcb(cueSheetName);
            var tempPlayerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out CriAtomEx.CueInfo tempInfo);
            tempPlayerData.CueInfo = tempInfo;

            // ���W�����Z�b�g���čĐ�
            var temp3dData = new CriAtomEx3dSource();

            temp3dData.SetPosition(playSoundWorldPos.x, playSoundWorldPos.y, playSoundWorldPos.z);
            // ���X�i�[�ƃ\�[�X��ݒ�
            _player.Set3dListener(_listener);
            _player.Set3dSource(temp3dData);
            tempPlayerData.Source = temp3dData;
            _player.SetCue(tempAcb, cueName);
            _player.SetVolume(_volume * _masterVolume * volume);
            _player.SetStartTime(0L);
            tempPlayerData.Playback = _player.Start();
            tempPlayerData.CancellationTokenSource = new CancellationTokenSource();

            return AddCueData(tempPlayerData);
        }

        public void Update3DPos(Vector3 playSoundWorldPos, int index)
        {
            if (index <= -1 || _cueData[index].Source == null) return;

            _cueData[index].UpdateCurrentVector(playSoundWorldPos);
        }

        public void Pause(int index)
        {
            if (index <= -1) return;

            _cueData[index].Playback.Pause();
        }

        public void Resume(int index)
        {
            if (index <= -1) return;

            _cueData[index].Playback.Resume(CriAtomEx.ResumeMode.AllPlayback);
        }

        public void Stop(int index)
        {
            if (index <= -1) return;

            _cueData[index].Playback.Stop(false);

            if (_cueData.Remove(index, out CriPlayerData outData))
            {
                _removedCueDataIndex.Add(index);
                outData.Playback.Stop(false);
                outData.Source?.Dispose();
                outData.CancellationTokenSource?.Cancel();
            }

        }

        public void StopAll()
        {
            _player.Stop(false);

            foreach (var VARIABLE in _cueData)
            {
                VARIABLE.Value.CancellationTokenSource?.Cancel();
                VARIABLE.Value.CancellationTokenSource?.Dispose();
            }

            _cueData.Clear();
            _removedCueDataIndex.Clear();
        }

        public void StopLoopCue()
        {
            var indexList = new List<int>();

            foreach (var n in _cueData)
            {
                if (n.Value.IsLoop)
                {
                    n.Value.Playback.Stop(false);
                }

                indexList.Add(n.Key);
            }

            foreach (var VARIABLE in indexList)
            {
                if (_cueData.Remove(VARIABLE, out CriPlayerData outData))
                {
                    _removedCueDataIndex.Add(VARIABLE);
                    outData.Source?.Dispose();
                }
            }
        }

        public void SetListenerAll(CriAtomListener listener)
        {
            _listener = listener.nativeListener;
            _player.Set3dListener(_listener);
            _player.UpdateAll();
        }

        public void SetListener(CriAtomListener listener, int index)
        {
            _listener = listener.nativeListener;
            _player.Set3dListener(_listener);
            _player.Update(_cueData[index].Playback);
        }

        /// <summary>�J�X�^���\���̂�Ԃ����\�b�h</summary>
        /// <param name="playerData">�J�X�^���\���̂�ێ����Ă���v���C���[���</param>
        public CriAtomCustomStruct GetCustomStruct(CriPlayerData playerData) => playerData.CustomStruct;

        /// <summary>�Đ��I����҂iUnitask�j</summary>
        public async UniTask WaitPlayingEnd(CancellationToken token, CriAtomCustomStruct customStruct)
        {
            // True��Ԃ��܂ő҂�
            await UniTask.WaitUntil(customStruct.CheckPlayingEnd, default, token, false);
        }

        /// <summary>�Đ��I����҂iCoroutine�j</summary>
        public System.Collections.IEnumerator WaitPlayingEndCor(CriAtomCustomStruct customStruct)
        {
            // True��Ԃ��܂ő҂�
            yield return new WaitUntil(customStruct.CheckPlayingEnd);
        }
    }

    /// <summary>CriAtomExPlayback�����b�v�����I���W�i���\����</summary>
    public struct CriAtomCustomStruct
    {
        /// <summary>CriAtomExPlayback�\����</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>�Đ��g���b�N���</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TrackInfo
        {
            public uint id;                         /**< �Đ�ID **/
            public CriAtomEx.CueType sequenceType;  /**< �e�V�[�P���X�^�C�v **/
            public IntPtr playerHn;                 /**< �v���[���n���h�� **/
            public ushort trackNo;                  /**< �g���b�N�ԍ� **/
            public ushort reserved;                 /**< �\��̈� **/
        }

        public CriAtomCustomStruct(uint id) : this()
        {
            this.id = id;
#if CRIWARE_ENABLE_HEADLESS_MODE
            this._dummyStatus = Status.Prep;
#endif
        }

        /// <summary>�Đ��X�e�[�^�X�̎擾</summary>
        /// <returns>�Đ������ǂ����ifalse = �Đ����Atrue = �Đ��I���j</returns>
        public bool CheckPlayingEnd()
        {
            if (GetStatus() == CriAtomExPlayback.Status.Removed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>�Đ����̒�~</summary>
        /// <param name = 'ignoresReleaseTime' > �����[�X���Ԃ𖳎����邩�ǂ���
        public void Stop(bool ignoresReleaseTime)
        {
            if (CriAtomPlugin.IsLibraryInitialized() == false) { return; }

            if (ignoresReleaseTime == false)
            {
                Playback.Stop();
            }
            else
            {
                Playback.StopWithoutReleaseTime();
            }
        }

        /// <summary>�Đ����̃|�[�Y</summary>
        public void Pause() => Playback.Pause(true);

        /// <summary>�Đ����̃|�[�Y����</summary>
        /// <param name="mode">�|�[�Y�����Ώ�</param>
        public void Resume(CriAtomEx.ResumeMode mode) => Playback.Resume(mode);

        /// <summary>�Đ����̃|�[�Y��Ԃ̎擾</summary>
        /// <returns>�|�[�Y�����ǂ����ifalse = �|�[�Y����Ă��Ȃ��Atrue = �|�[�Y���j</returns>
        public bool IsPaused() => Playback.IsPaused();

        /// <summary>�Đ����̃t�H�[�}�b�g���̎擾</summary>
        /// <param name='info'>�t�H�[�}�b�g���</param>
        /// <returns>��񂪎擾�ł������ǂ����i true = �擾�ł����A false = �擾�ł��Ȃ������j</returns>
        public bool GetFormatInfo(out CriAtomEx.FormatInfo info) => Playback.GetFormatInfo(out info);

        /// <summary>�Đ��X�e�[�^�X�̎擾</summary>
        /// <returns>�Đ��X�e�[�^�X</returns>
        public CriAtomExPlayback.Status GetStatus() => Playback.GetStatus();

        /// <summary>�Đ������̎擾</summary>
        /// <returns>�Đ������i�~���b�P�ʁj</returns>
        public long GetTime() => Playback.GetTime();

        /// <summary>�����ɓ��������Đ������̎擾</summary>
        /// <returns>�Đ������i�~���b�P�ʁj</returns>
        public long GetTimeSyncedWithAudio() => Playback.GetTimeSyncedWithAudio();

        /// <summary>�Đ��T���v�����̎擾</summary>
        /// <param name='numSamples'>�Đ��ς݃T���v����</param>
        /// <param name='samplingRate'>�T���v�����O���[�g</param>
        /// <returns>�T���v�������擾�ł������ǂ����i true = �擾�ł����A false = �擾�ł��Ȃ������j</returns>
        public bool GetNumPlayedSamples(out long numSamples, out int samplingRate) => Playback.GetNumPlayedSamples(out numSamples, out samplingRate);

        /// <summary>�V�[�P���X�Đ��ʒu�̎擾</summary>
        /// <returns>�V�[�P���X�Đ��ʒu�i�~���b�P�ʁj</returns>
        public long GetSequencePosition() => Playback.GetSequencePosition();

        /// <summary>�Đ����̃J�����g�u���b�N�C���f�b�N�X�̎擾</summary>
        /// <returns>�J�����g�u���b�N�C���f�b�N�X</returns>
        public int GetCurrentBlockIndex() => Playback.GetCurrentBlockIndex();

        /// <summary>�Đ��g���b�N���̎擾</summary>
        /// <param name='info'>�Đ��g���b�N���</param>
        /// <returns>�擾�ɐ���������</returns>
        public bool GetTrackInfo(out CriAtomExPlayback.TrackInfo info) => Playback.GetTrackInfo(out info);

        /// <summary>�r�[�g�������̎擾</summary>
        /// <param name='info'>�r�[�g�������</param>
        /// <returns>�擾�ɐ���������</returns>
        public bool GetBeatSyncInfo(out CriAtomExBeatSync.Info info) => Playback.GetBeatSyncInfo(out info);

        /// <summary>�Đ����̃u���b�N�J��</summary>
        /// <param name='index'>�u���b�N�C���f�b�N�X</param>
        public void SetNextBlockIndex(int index) => Playback.SetNextBlockIndex(index);

        /// <summary>�r�[�g�����I�t�Z�b�g�̐ݒ�</summary>
        /// <param name='timeMs'>�I�t�Z�b�g���ԁi�~���b�j</param>
        /// <returns>�I�t�Z�b�g�̐ݒ�ɐ���������</returns>

        public bool SetBeatSyncOffset(short timeMs) => Playback.SetBeatSyncOffset(timeMs);

        public uint id
        {
            get;
            private set;
        }

        public CriAtomExPlayback.Status status
        {
            get
            {
                return this.GetStatus();
            }
        }

        public long time
        {
            get
            {
                return this.GetTime();
            }
        }

        public long timeSyncedWithAudio
        {
            get
            {
                return this.GetTimeSyncedWithAudio();
            }
        }

        public const uint invalidId = 0xFFFFFFFF;

        public void Stop()
        {
            if (CriAtomPlugin.IsLibraryInitialized() == false) { return; }
            Playback.Stop();
        }
        public void StopWithoutReleaseTime()
        {
            if (CriAtomPlugin.IsLibraryInitialized() == false) { return; }
            Playback.StopWithoutReleaseTime();
        }
        public void Pause(bool sw) { Playback.Pause(sw); }
    }
}

#endif