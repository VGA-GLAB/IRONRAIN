// ���{��Ή�
using CriWare;
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class CRICustomStruct
{
    /// <summary>CriAtomExPlayback�����b�v�����I���W�i���\����</summary>
    public struct CriCustomStruct
    {
        /// <summary>CS1673�΍��Func�ϐ�</summary>
        public Func<bool> func;

        /// <summary>�Đ��X�e�[�^�X</summary>
        public enum Status
        {
            Prep = 1, /**< �Đ������� **/
            Playing = 2, /**< �Đ��� **/
            Removed = 3, /**< �폜���ꂽ **/
        }

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

        public CriCustomStruct(uint id): this()
        {
            this.id = id;
            #if CRIWARE_ENABLE_HEADLESS_MODE
		    this._dummyStatus = Status.Prep;
            #endif
        }

        /// <summary>�Đ��I����҂iUnitask�j</summary>
        public async UniTask WaitPlayingEnd()
        {
            await UniTask.WaitUntil(func = CheckPlayingEnd);
        }

        /// <summary>�Đ��I����҂iCoroutine�j</summary>
        public IEnumerator WaitPlayingEndCor()
        {
            yield return new WaitUntil(func = CheckPlayingEnd);
        }

        /// <summary>�Đ��X�e�[�^�X�̎擾</summary>
        /// <returns>�Đ������ǂ����ifalse = �Đ����Atrue = �Đ��I���j</returns>
        public bool CheckPlayingEnd()
        {
            if (GetStatus() == Status.Removed)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>�Đ����̒�~</summary>
        /// <param name='ignoresReleaseTime'>�����[�X���Ԃ𖳎����邩�ǂ���
        public void Stop(bool ignoresReleaseTime)
        {
            if (CriAtomPlugin.IsLibraryInitialized() == false) { return; }

            if (ignoresReleaseTime == false)
            {
                criAtomExPlayback_Stop(this.id);
            }
            else
            {
                criAtomExPlayback_StopWithoutReleaseTime(this.id);
            }
        }

        /// <summary>�Đ����̃|�[�Y</summary>
        public void Pause()
        {
            criAtomExPlayback_Pause(this.id, true);
        }
                
        /// <summary>�Đ����̃|�[�Y����</summary>
        /// <param name="mode">�|�[�Y�����Ώ�</param>
        public void Resume(CriAtomEx.ResumeMode mode)
        {
            criAtomExPlayback_Resume(this.id, mode);
        }

        /// <summary>�Đ����̃|�[�Y��Ԃ̎擾</summary>
        /// <returns>�|�[�Y�����ǂ����ifalse = �|�[�Y����Ă��Ȃ��Atrue = �|�[�Y���j</returns>
        public bool IsPaused()
        {
            return criAtomExPlayback_IsPaused(this.id);
        }

        /// <summary>�Đ����̃t�H�[�}�b�g���̎擾</summary>
        /// <param name='info'>�t�H�[�}�b�g���</param>
        /// <returns>��񂪎擾�ł������ǂ����i true = �擾�ł����A false = �擾�ł��Ȃ������j</returns>
        public bool GetFormatInfo(out CriAtomEx.FormatInfo info)
        {
            return criAtomExPlayback_GetFormatInfo(this.id, out info);
        }

        /// <summary>�Đ��X�e�[�^�X�̎擾</summary>
        /// <returns>�Đ��X�e�[�^�X</returns>
        public Status GetStatus()
        {
            return criAtomExPlayback_GetStatus(this.id);
        }

        /// <summary>�Đ������̎擾</summary>
        /// <returns>�Đ������i�~���b�P�ʁj</returns>
        public long GetTime()
        {
            return criAtomExPlayback_GetTime(this.id);
        }

        /// <summary>�����ɓ��������Đ������̎擾</summary>
        /// <returns>�Đ������i�~���b�P�ʁj</returns>
        public long GetTimeSyncedWithAudio()
        {
            return criAtomExPlayback_GetTimeSyncedWithAudio(this.id);
        }

        /// <summary>�Đ��T���v�����̎擾</summary>
        /// <param name='numSamples'>�Đ��ς݃T���v����</param>
        /// <param name='samplingRate'>�T���v�����O���[�g</param>
        /// <returns>�T���v�������擾�ł������ǂ����i true = �擾�ł����A false = �擾�ł��Ȃ������j</returns>
        public bool GetNumPlayedSamples(out long numSamples, out int samplingRate)
        {
            return criAtomExPlayback_GetNumPlayedSamples(this.id, out numSamples, out samplingRate);
        }

        /// <summary>�V�[�P���X�Đ��ʒu�̎擾</summary>
        /// <returns>�V�[�P���X�Đ��ʒu�i�~���b�P�ʁj</returns>
        public long GetSequencePosition()
        {
            return criAtomExPlayback_GetSequencePosition(this.id);
        }

        /// <summary>�Đ����̃J�����g�u���b�N�C���f�b�N�X�̎擾</summary>
        /// <returns>�J�����g�u���b�N�C���f�b�N�X</returns>
        public int GetCurrentBlockIndex()
        {
            return criAtomExPlayback_GetCurrentBlockIndex(this.id);
        }

        /// <summary>�Đ��g���b�N���̎擾</summary>
        /// <param name='info'>�Đ��g���b�N���</param>
        /// <returns>�擾�ɐ���������</returns>
        public bool GetTrackInfo(out TrackInfo info)
        {
            return criAtomExPlayback_GetPlaybackTrackInfo(this.id, out info);
        }

        /// <summary>�r�[�g�������̎擾</summary>
        /// <param name='info'>�r�[�g�������</param>
        /// <returns>�擾�ɐ���������</returns>
        public bool GetBeatSyncInfo(out CriAtomExBeatSync.Info info)
        {
            return criAtomExPlayback_GetBeatSyncInfo(this.id, out info);
        }

        /// <summary>�Đ����̃u���b�N�J��</summary>
        /// <param name='index'>�u���b�N�C���f�b�N�X</param>
        public void SetNextBlockIndex(int index)
        {
            criAtomExPlayback_SetNextBlockIndex(this.id, index);
        }

        /// <summary>�r�[�g�����I�t�Z�b�g�̐ݒ�</summary>
        /// <param name='timeMs'>�I�t�Z�b�g���ԁi�~���b�j</param>
        /// <returns>�I�t�Z�b�g�̐ݒ�ɐ���������</returns>
         
        public bool SetBeatSyncOffset(short timeMs)
        {
            return criAtomExPlayback_SetBeatSyncOffset(this.id, timeMs);
        }

        public uint id
        {
            get;
            private set;
        }

        public Status status
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
            criAtomExPlayback_Stop(this.id);
        }
        public void StopWithoutReleaseTime()
        {
            if (CriAtomPlugin.IsLibraryInitialized() == false) { return; }
            criAtomExPlayback_StopWithoutReleaseTime(this.id);
        }
        public void Pause(bool sw) { criAtomExPlayback_Pause(this.id, sw); }


        #region DLL Import
#if !CRIWARE_ENABLE_HEADLESS_MODE
        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern void criAtomExPlayback_Stop(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern void criAtomExPlayback_StopWithoutReleaseTime(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern void criAtomExPlayback_Pause(uint id, bool sw);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern void criAtomExPlayback_Resume(uint id, CriAtomEx.ResumeMode mode);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_IsPaused(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern Status criAtomExPlayback_GetStatus(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_GetFormatInfo(
            uint id, out CriAtomEx.FormatInfo info);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern long criAtomExPlayback_GetTime(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern long criAtomExPlayback_GetTimeSyncedWithAudio(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_GetNumPlayedSamples(
            uint id, out long num_samples, out int sampling_rate);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern long criAtomExPlayback_GetSequencePosition(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern void criAtomExPlayback_SetNextBlockIndex(uint id, int index);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern int criAtomExPlayback_GetCurrentBlockIndex(uint id);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_GetPlaybackTrackInfo(uint id, out TrackInfo info);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_GetBeatSyncInfo(uint id, out CriAtomExBeatSync.Info info);

        [DllImport(CriWare.Common.pluginName, CallingConvention = CriWare.Common.pluginCallingConvention)]
        private static extern bool criAtomExPlayback_SetBeatSyncOffset(uint id, short timeMs);
#else
	private Status _dummyStatus;
	private bool _dummyPaused;
	private void criAtomExPlayback_Stop(uint id) { _dummyStatus = Status.Removed; }
	private void criAtomExPlayback_StopWithoutReleaseTime(uint id) { _dummyStatus = Status.Removed; }
	private void criAtomExPlayback_Pause(uint id, bool sw) { _dummyPaused = sw; }
	private static void criAtomExPlayback_Resume(uint id, CriAtomEx.ResumeMode mode) { }
	private bool criAtomExPlayback_IsPaused(uint id) { return _dummyPaused; }
	private Status criAtomExPlayback_GetStatus(uint id)
	{
		if (_dummyStatus != Status.Removed) {
			_dummyStatus = _dummyStatus + 1;
		}
		return _dummyStatus;
	}
	private static bool criAtomExPlayback_GetFormatInfo(
		uint id, out CriAtomEx.FormatInfo info) { info = new CriAtomEx.FormatInfo(); return false; }
	private static long criAtomExPlayback_GetTime(uint id) { return 0; }
	private static long criAtomExPlayback_GetTimeSyncedWithAudio(uint id) { return 0; }
	private static bool criAtomExPlayback_GetNumPlayedSamples(
		uint id, out long num_samples, out int sampling_rate) { num_samples = sampling_rate = 0; return false; }
	private static long criAtomExPlayback_GetSequencePosition(uint id) { return 0; }
	private static void criAtomExPlayback_SetNextBlockIndex(uint id, int index) { }
	private static int criAtomExPlayback_GetCurrentBlockIndex(uint id) { return -1; }
	private static bool criAtomExPlayback_GetPlaybackTrackInfo(uint id, out TrackInfo info) { info = new TrackInfo(); return false; }
	private static bool criAtomExPlayback_GetBeatSyncInfo(uint id, out CriAtomExBeatSync.Info info) { info = new CriAtomExBeatSync.Info(); return false; }
	private static bool criAtomExPlayback_SetBeatSyncOffset(uint id, short timeMs) { return false; }
#endif
#endregion
    }
}