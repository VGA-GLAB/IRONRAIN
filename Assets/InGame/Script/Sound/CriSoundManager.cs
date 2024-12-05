#if CRI_SOUND_MANAGER
using System;
using CriWare;
using System.Runtime.InteropServices;

/// <summary>サウンドを管理するクラス</summary>
public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    /// <summary>CriAtomExPlaybackをラップしたオリジナル構造体</summary>
    public struct CriAtomCustomStruct
    {
        /// <summary>CriAtomExPlayback構造体</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>再生トラック情報</summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct TrackInfo
        {
            public uint id;

            /**< 再生ID **/
            public CriAtomEx.CueType sequenceType;

            /**< 親シーケンスタイプ **/
            public IntPtr playerHn;

            /**< プレーヤハンドル **/
            public ushort trackNo;

            /**< トラック番号 **/
            public ushort reserved; /**< 予約領域 **/
        }

        /// <summary>再生ステータスの取得</summary>
        /// <returns>再生中かどうか（false = 再生中、true = 再生終了）</returns>
        public bool CheckPlayingEnd()
        {
            if (GetStatus() == CriAtomExPlayback.Status.Removed)
            {
                return true;
            }

            return false;
        }

        /// <summary>再生ステータスの取得</summary>
        /// <returns>再生ステータス</returns>
        public CriAtomExPlayback.Status GetStatus() => Playback.GetStatus();
    }
}

#endif