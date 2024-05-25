#if CRI_CUSTOM_STRUCT

// 日本語対応
using CriWare;
using System;
using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Collections;

/// <summary>CriAtomExPlaybackをラップしたオリジナル構造体</summary>
public struct CriCustomStruct
{
    /// <summary>CriAtomExPlayback構造体</summary>
    public CriAtomExPlayback Playback { get; set; }

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

    public CriCustomStruct(uint id) : this()
    {
        this.id = id;
#if CRIWARE_ENABLE_HEADLESS_MODE
		this._dummyStatus = Status.Prep;
#endif
    }

    /// <summary>再生終了を待つ（Unitask）</summary>
    public async UniTask WaitPlayingEnd()
    {
        await UniTask.WaitUntil(CheckPlayingEnd);
    }

    /// <summary>再生終了を待つ（Coroutine）</summary>
    public IEnumerator WaitPlayingEndCor()
    {
        yield return new WaitUntil(CheckPlayingEnd);
    }

    /// <summary>再生ステータスの取得</summary>
    /// <returns>再生中かどうか（false = 再生中、true = 再生終了）</returns>
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

    /// <summary>再生音の停止</summary>
    /// <param name = 'ignoresReleaseTime' > リリース時間を無視するかどうか
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

    /// <summary>再生音のポーズ</summary>
    public void Pause() => Playback.Pause(true);

    /// <summary>再生音のポーズ解除</summary>
    /// <param name="mode">ポーズ解除対象</param>
    public void Resume(CriAtomEx.ResumeMode mode) => Playback.Resume(mode);

    /// <summary>再生音のポーズ状態の取得</summary>
    /// <returns>ポーズ中かどうか（false = ポーズされていない、true = ポーズ中）</returns>
    public bool IsPaused() => Playback.IsPaused();

    /// <summary>再生音のフォーマット情報の取得</summary>
    /// <param name='info'>フォーマット情報</param>
    /// <returns>情報が取得できたかどうか（ true = 取得できた、 false = 取得できなかった）</returns>
    public bool GetFormatInfo(out CriAtomEx.FormatInfo info) => Playback.GetFormatInfo(out info);

    /// <summary>再生ステータスの取得</summary>
    /// <returns>再生ステータス</returns>
    public CriAtomExPlayback.Status GetStatus() => Playback.GetStatus();

    /// <summary>再生時刻の取得</summary>
    /// <returns>再生時刻（ミリ秒単位）</returns>
    public long GetTime() => Playback.GetTime();

    /// <summary>音声に同期した再生時刻の取得</summary>
    /// <returns>再生時刻（ミリ秒単位）</returns>
    public long GetTimeSyncedWithAudio() => Playback.GetTimeSyncedWithAudio();

    /// <summary>再生サンプル数の取得</summary>
    /// <param name='numSamples'>再生済みサンプル数</param>
    /// <param name='samplingRate'>サンプリングレート</param>
    /// <returns>サンプル数が取得できたかどうか（ true = 取得できた、 false = 取得できなかった）</returns>
    public bool GetNumPlayedSamples(out long numSamples, out int samplingRate) => Playback.GetNumPlayedSamples(out numSamples, out samplingRate);

    /// <summary>シーケンス再生位置の取得</summary>
    /// <returns>シーケンス再生位置（ミリ秒単位）</returns>
    public long GetSequencePosition() => Playback.GetSequencePosition();

    /// <summary>再生音のカレントブロックインデックスの取得</summary>
    /// <returns>カレントブロックインデックス</returns>
    public int GetCurrentBlockIndex() => Playback.GetCurrentBlockIndex();

    /// <summary>再生トラック情報の取得</summary>
    /// <param name='info'>再生トラック情報</param>
    /// <returns>取得に成功したか</returns>
    public bool GetTrackInfo(out CriAtomExPlayback.TrackInfo info) => Playback.GetTrackInfo(out info);

    /// <summary>ビート同期情報の取得</summary>
    /// <param name='info'>ビート同期情報</param>
    /// <returns>取得に成功したか</returns>
    public bool GetBeatSyncInfo(out CriAtomExBeatSync.Info info) => Playback.GetBeatSyncInfo(out info);

    /// <summary>再生音のブロック遷移</summary>
    /// <param name='index'>ブロックインデックス</param>
    public void SetNextBlockIndex(int index) => Playback.SetNextBlockIndex(index);

    /// <summary>ビート同期オフセットの設定</summary>
    /// <param name='timeMs'>オフセット時間（ミリ秒）</param>
    /// <returns>オフセットの設定に成功したか</returns>

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