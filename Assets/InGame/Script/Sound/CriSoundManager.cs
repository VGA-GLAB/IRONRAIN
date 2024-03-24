using UnityEngine;
using System;
using CriWare;
using System.Collections.Concurrent;
using System.Threading;

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
        _masterVolume = new Volume();
    }

    /// <summary>マスターボリューム</summary>
    private Volume _masterVolume = default;

    /// <summary>ボリュームのインターフェース</summary>
    public interface IVolume
    {
        /// <summary>音量のプロパティ</summary>
        public float Value { get; set; }

        /// <summary>音量が変更された際のイベント</summary>
        public event Action<float> OnVolumeChanged;
    }

    /// <summary>ボリュームのクラス</summary>
    private class Volume : IVolume
    {
        /// <summary>ボリューム</summary>
        private float _value = 1.0f;

        /// <summary>イベントが呼ばれる際の閾値</summary>
        private const float DIFF = 0.01f;

        /// <summary>ボリュームのプロパティ</summary>
        public float Value
        {
            get => _value;
            set
            {
                // 0から1の間に制限する
                value = Mathf.Clamp01(value);
                // ボリュームの変化量が閾値を超えていた場合
                if (_value + DIFF < value || _value - DIFF > value)
                {
                    // イベントの呼び出し
                    _onVolumeChanged?.Invoke(value);
                    _value = value;
                }
            }
        }

        /// <summary>ボリュームが変更された際のイベント</summary>
        private event Action<float> _onVolumeChanged = default;

        public event Action<float> OnVolumeChanged
        {
            add => OnVolumeChanged += value;
            remove => OnVolumeChanged -= value;
        }

        /// <summary>暗黙的な演算子</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }

    /// <summary>Playerのデータをまとめた構造体</summary>
    private struct CriPlayerData
    {
        /// <summary>再生された音声を制御するためのオブジェクト</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>キュー情報</summary>
        public CriAtomEx.CueInfo CueInfo { get; set; }

        /// <summary>3D音源を扱うためのオブジェクト</summary>
        public CriAtomEx3dSource Source { get; set; }

        /// <summary>最後にアップデートが行われた時間</summary>
        public float LastUpdateTime { get; set; }

        /// <summary>キューがループしているか</summary>
        public bool IsLoop => CueInfo.length < 0;

        /// <summary>キャンセル処理</summary>
        public CancellationTokenSource CancellationTokenSource { get; set; }

        /// <summary>進行方向のベクトルを計算・設定する</summary>
        /// <param name="nextPos">次のポジション</param>
        public void UpdateCurrentVector(Vector3 nextPos)
        {
            // 前回のアップデートからの経過時間
            var delta = Playback.GetTime() - LastUpdateTime;

            // 現在のポジションを取得する
            CriAtomEx.NativeVector nativePos = Source.GetPosition();

            // NativeVectorをVector3に変換する
            Vector3 currentPos = new Vector3(nativePos.x, nativePos.y, nativePos.z);

            // 進行方向のベクトルを計算する
            Vector3 moveVec = nextPos - currentPos;

            // 1秒あたり（CriAtomEx3dSource.SetVelocityの引数）の移動量
            moveVec /= delta;

            // アップデート時間を更新
            LastUpdateTime = Playback.GetTime();

            // ポジション・ベクトルを設定する
            Source.SetPosition(nextPos.x, nextPos.y, nextPos.z);
            Source.SetVelocity(moveVec.x, moveVec.y, moveVec.z);

            // ポジション・ベクトルを更新する
            Source.Update();
        }
    }

    /// <summary>チャンネルを作成するために必要な情報をまとめたクラス</summary>
    private abstract class AbstractCriChannel
    {
        /// <summary>音声を再生するためのプレイヤー</summary>
        protected CriAtomExPlayer _player = new CriAtomExPlayer();

        /// <summary>キューのデータ</summary>
        protected ConcurrentDictionary<int, CriPlayerData> _cueData = new ConcurrentDictionary<int, CriPlayerData>();

        /// <summary>キューデータのカウントの最大数</summary>
        protected int _maxCueCount = 0;

        /// <summary>削除されたキューデータのインデックスを格納するコレクション</summary>
        protected ConcurrentBag<int> _removedCueDataIndex = new ConcurrentBag<int>();

        /// <summary>3Dリスナーを扱うためのオブジェクト</summary>
        protected CriAtomEx3dListener _listener = default;

        /// <summary>ボリューム</summary>
        protected Volume _volume = new Volume();

        /// <summary>マスターボリューム</summary>
        protected Volume _masterVolume = null;

        /// <summary>キャンセル処理</summary>
        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>コンストラクタ</summary>
        protected AbstractCriChannel(in Volume masterVolume)
        {
            // マスターボリュームを設定
            _masterVolume = masterVolume;

            // ボリュームが変更された際のイベントを登録
            _volume.OnVolumeChanged += UpdateVolume;
            _masterVolume.OnVolumeChanged += UpdateMasterVolume;
        }

        /// <summary>デストラクタ</summary>
        ~AbstractCriChannel()
        {
            // キャンセル処理を実行
            _tokenSource.Cancel();

            // ボリュームが変更された際のイベントを削除
            _volume.OnVolumeChanged -= UpdateVolume;
            _masterVolume.OnVolumeChanged -= UpdateMasterVolume;

            // プレイヤーを破棄
            _player.Dispose();

            // キューのデータを破棄
            foreach (var data in _cueData)
            {
                data.Value.CancellationTokenSource.Cancel();
                data.Value.Source.Dispose();
            }
        }

        /// <summary>ボリュームを更新する</summary>
        /// <param name="volume">ボリュームのプロパティにセットした値</param>
        private void UpdateVolume(float volume)
        {
            // プレイヤーのボリュームを設定
            _player.SetVolume(volume * _masterVolume);

            // 設定したボリュームを再生中の音声にも反映
            foreach (var data in _cueData)
            {
                _player.Update(data.Value.Playback);
            }
        }

        /// <summary>マスターボリュームを更新する</summary>
        /// <param name="masterVolume">ボリュームのプロパティにセットした値</param>
        private void UpdateMasterVolume(float masterVolume)
        {
            // プレイヤーのボリュームを設定
            _player.SetVolume(_volume * masterVolume);

            // 設定したボリュームを再生中の音声にも反映
            foreach (var data in _cueData)
            {
                _player.Update(data.Value.Playback);
            }
        }

        /// <summary></summary>
        /// <param name="playerData">Playerのデータ</param>
        /// <returns></returns>
        protected int AddCueData(CriPlayerData playerData)
        {
            // キューがループしている場合
            if (playerData.IsLoop)
            {
                // 削除されたキューデータが1つ以上ある場合
                if (_removedCueDataIndex.Count > 0)
                {
                    // 
                    int tempIndex;

                    // 
                    if (_removedCueDataIndex.TryTake(out tempIndex))
                    {
                        // キーと値のペアを追加
                        _cueData.TryAdd(tempIndex, playerData);
                    }

                    return tempIndex;
                }

                // 削除されたキューデータが存在しない場合
                else
                {
                    //
                    _maxCueCount++;

                    // キーと値のペアを追加
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