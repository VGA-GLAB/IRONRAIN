// 日本語対応
using CriWare;
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;

public class CRICustomStruct
{
    /// <summary>CriAtomExPlaybackをラップしたオリジナル構造体</summary>
    public struct CriCustomStruct
    {
        /// <summary>CS1673対策のFunc変数</summary>
        public Func<bool> func;

        /// <summary>再生ステータス</summary>
        public enum Status
        {
            Prep = 1, /**< 再生準備中 **/
            Playing = 2, /**< 再生中 **/
            Removed = 3, /**< 削除された **/
        }

        /// <summary>再生トラック情報</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TrackInfo
        {
            public uint id;                         /**< 再生ID **/
            public CriAtomEx.CueType sequenceType;  /**< 親シーケンスタイプ **/
            public IntPtr playerHn;                 /**< プレーヤハンドル **/
            public ushort trackNo;                  /**< トラック番号 **/
            public ushort reserved;                 /**< 予約領域 **/
        }

        public CriCustomStruct(uint id): this()
        {
            this.id = id;
            #if CRIWARE_ENABLE_HEADLESS_MODE
		    this._dummyStatus = Status.Prep;
            #endif
        }

        /// <summary>再生終了を待つ（Unitask）</summary>
        public async UniTask WaitPlayingEnd()
        {
            await UniTask.WaitUntil(func = CheckPlayingEnd);
        }

        /// <summary>再生終了を待つ（Coroutine）</summary>
        public IEnumerator WaitPlayingEndCor()
        {
            yield return new WaitUntil(func = CheckPlayingEnd);
        }

        /// <summary>再生ステータスの取得</summary>
        /// <returns>再生中かどうか（false = 再生中、true = 再生終了）</returns>
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

        /// <summary>再生音の停止</summary>
        /// <param name='ignoresReleaseTime'>リリース時間を無視するかどうか
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

        /// <summary>再生音のポーズ</summary>
        public void Pause()
        {
            criAtomExPlayback_Pause(this.id, true);
        }
                
        /// <summary>再生音のポーズ解除</summary>
        /// <param name="mode">ポーズ解除対象</param>
        public void Resume(CriAtomEx.ResumeMode mode)
        {
            criAtomExPlayback_Resume(this.id, mode);
        }

        /// <summary>再生音のポーズ状態の取得</summary>
        /// <returns>ポーズ中かどうか（false = ポーズされていない、true = ポーズ中）</returns>
        public bool IsPaused()
        {
            return criAtomExPlayback_IsPaused(this.id);
        }

        /// <summary>再生音のフォーマット情報の取得</summary>
        /// <param name='info'>フォーマット情報</param>
        /// <returns>情報が取得できたかどうか（ true = 取得できた、 false = 取得できなかった）</returns>
        public bool GetFormatInfo(out CriAtomEx.FormatInfo info)
        {
            return criAtomExPlayback_GetFormatInfo(this.id, out info);
        }

        /// <summary>再生ステータスの取得</summary>
        /// <returns>再生ステータス</returns>
        public Status GetStatus()
        {
            return criAtomExPlayback_GetStatus(this.id);
        }

        /// <summary>再生時刻の取得</summary>
        /// <returns>再生時刻（ミリ秒単位）</returns>
        public long GetTime()
        {
            return criAtomExPlayback_GetTime(this.id);
        }

        /// <summary>音声に同期した再生時刻の取得</summary>
        /// <returns>再生時刻（ミリ秒単位）</returns>
        public long GetTimeSyncedWithAudio()
        {
            return criAtomExPlayback_GetTimeSyncedWithAudio(this.id);
        }

        /// <summary>再生サンプル数の取得</summary>
        /// <param name='numSamples'>再生済みサンプル数</param>
        /// <param name='samplingRate'>サンプリングレート</param>
        /// <returns>サンプル数が取得できたかどうか（ true = 取得できた、 false = 取得できなかった）</returns>
        public bool GetNumPlayedSamples(out long numSamples, out int samplingRate)
        {
            return criAtomExPlayback_GetNumPlayedSamples(this.id, out numSamples, out samplingRate);
        }

        /// <summary>シーケンス再生位置の取得</summary>
        /// <returns>シーケンス再生位置（ミリ秒単位）</returns>
        public long GetSequencePosition()
        {
            return criAtomExPlayback_GetSequencePosition(this.id);
        }

        /// <summary>再生音のカレントブロックインデックスの取得</summary>
        /// <returns>カレントブロックインデックス</returns>
        public int GetCurrentBlockIndex()
        {
            return criAtomExPlayback_GetCurrentBlockIndex(this.id);
        }

        /// <summary>再生トラック情報の取得</summary>
        /// <param name='info'>再生トラック情報</param>
        /// <returns>取得に成功したか</returns>
        public bool GetTrackInfo(out TrackInfo info)
        {
            return criAtomExPlayback_GetPlaybackTrackInfo(this.id, out info);
        }

        /// <summary>ビート同期情報の取得</summary>
        /// <param name='info'>ビート同期情報</param>
        /// <returns>取得に成功したか</returns>
        public bool GetBeatSyncInfo(out CriAtomExBeatSync.Info info)
        {
            return criAtomExPlayback_GetBeatSyncInfo(this.id, out info);
        }

        /// <summary>再生音のブロック遷移</summary>
        /// <param name='index'>ブロックインデックス</param>
        public void SetNextBlockIndex(int index)
        {
            criAtomExPlayback_SetNextBlockIndex(this.id, index);
        }

        /// <summary>ビート同期オフセットの設定</summary>
        /// <param name='timeMs'>オフセット時間（ミリ秒）</param>
        /// <returns>オフセットの設定に成功したか</returns>
         
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