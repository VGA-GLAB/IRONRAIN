#if CRI_CUSTOM_STRUCT

// ���{��Ή�
using CriWare;
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;

/// <summary>CriAtomExPlayback�����b�v�����I���W�i���\����</summary>
public struct CriCustomStruct
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

    public CriCustomStruct(uint id) : this()
    {
        this.id = id;
#if CRIWARE_ENABLE_HEADLESS_MODE
		this._dummyStatus = Status.Prep;
#endif
    }

    /// <summary>�Đ��I����҂iUnitask�j</summary>
    public async UniTask WaitPlayingEnd()
    {
        await UniTask.WaitUntil(CheckPlayingEnd);
    }

    /// <summary>�Đ��I����҂iCoroutine�j</summary>
    public IEnumerator WaitPlayingEndCor()
    {
        yield return new WaitUntil(CheckPlayingEnd);
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
#endif