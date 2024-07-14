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

/// <summary>サウンドを管理するクラス</summary>
public class CriSoundManager : SingletonMonoBehaviour<CriSoundManager>
{
    private void Awake()
    {
        _masterVolume = new Volume();
        _bgm = new CriSingleChannel(_masterVolume);
        _se = new CriMultiChannel(_masterVolume);
    }

    /// <summary>マスターボリューム</summary>
    private Volume _masterVolume = default;

    /// <summary>BGMを流すチャンネル</summary>
    private CriSingleChannel _bgm = default;

    /// <summary>SEを流すチャンネル</summary>
    private CriMultiChannel _se = default;

    /// <summary>マスターのボリューム</summary>
    public IVolume MasterVolume => _masterVolume;

    /// <summary>BGMのチャンネル</summary>
    public ICustomChannel BGM => _bgm;

    /// <summary>SEのチャンネル</summary>
    public ICustomChannel SE => _se;

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
            add => _onVolumeChanged += value;
            remove => _onVolumeChanged -= value;
        }

        /// <summary>暗黙的な演算子</summary>
        public static implicit operator float(Volume volume) => volume.Value;
    }

    /// <summary>Playerのデータをまとめた構造体</summary>
    private struct CriPlayerData
    {
        /// <summary>再生された音声を制御するためのオブジェクト</summary>
        public CriAtomExPlayback Playback { get; set; }

        /// <summary>再生された音声を制御するためのオブジェクト</summary>
        public CriAtomCustomStruct CustomStruct { get; set; }

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

        /// <summary>プレイヤー情報のスレッドセーフなコレクション</summary>
        protected ConcurrentDictionary<int, CriPlayerData> _cueData = new ConcurrentDictionary<int, CriPlayerData>();

        /// <summary>キューデータのカウントの最大数</summary>
        protected int _maxCueCount = 0;

        /// <summary>削除されたプレイヤーデータのインデックスを格納するコレクション</summary>
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
        /// <param name="masterVolume">マスターボリューム（入力専用）</param>
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

            // プレイヤーのデータを破棄
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

        /// <summary>再生が終了するのを待ってからPlaybackを削除する</summary>
        /// <param name="index">キューデータのインデックス</param>
        protected async void DestroyPlaybackAfterPlayEnd(int index)
        {
            // キューがループしていたら抜ける
            if (_cueData[index].IsLoop)
            {
                return;
            }

            // 指定したミリ秒（キューの長さ）後に完了するキャンセル可能なタスクを作成
            await Task.Delay((int)_cueData[index].CueInfo.length, _cueData[index].CancellationTokenSource.Token);

            while (true)
            {
                // 再生が完了した場合 && 指定したキーを持つ値が正常に削除された場合
                if (_cueData[index].Playback.GetStatus() == CriAtomExPlayback.Status.Removed && _cueData.TryRemove(index, out CriPlayerData deletedData))
                {
                    // インデックスをコレクションに追加
                    _removedCueDataIndex.Add(index);

                    // 削除されたプレイヤーのデータを破棄
                    deletedData.Source?.Dispose();

                    return;
                }

                // 再生中の場合
                else
                {
                    // 指定した期間の後に完了するキャンセル可能なタスクを作成
                    await Task.Delay(TimeSpan.FromSeconds(0.05D), _cueData[index].CancellationTokenSource.Token);
                }
            }
        }

        /// <summary></summary>
        /// <param name="playerData">Playerのデータ</param>
        protected int AddCueData(CriPlayerData playerData)
        {
            // キューがループしている場合
            if (playerData.IsLoop)
            {
                // 削除されたキューデータが1つ以上ある場合
                if (_removedCueDataIndex.Count > 0)
                {
                    // 変数を初期化
                    int tempIndex;

                    //  コレクションからオブジェクトを削除できる場合
                    if (_removedCueDataIndex.TryTake(out tempIndex))
                    {
                        // キーとPlayerデータのペアを追加
                        _cueData.TryAdd(tempIndex, playerData);
                    }

                    return tempIndex;
                }

                // 削除されたキューデータが存在しない場合
                else
                {
                    // キューデータのカウントを増加
                    _maxCueCount++;

                    // キーとPlayerデータのペアを追加
                    _cueData.TryAdd(_maxCueCount, playerData);

                    return _maxCueCount;
                }
            }

            // キューがループしていない場合 && 削除されたキューデータが1つ以上ある場合
            if (_removedCueDataIndex.Count > 0)
            {
                // 変数を初期化
                int tempIndex;

                // コレクションからオブジェクトを削除できる場合
                if (_removedCueDataIndex.TryTake(out tempIndex))
                {
                    // キーとPlayerデータのペアを追加
                    _cueData.TryAdd(tempIndex, playerData);
                }

                DestroyPlaybackAfterPlayEnd(tempIndex);
                return tempIndex;
            }
            // キューがループしていない場合 && 削除されたキューデータが存在しない場合
            else
            {
                // キューデータのカウントを増加
                _maxCueCount++;

                // キーとPlayerデータのペアを追加
                _cueData.TryAdd(_maxCueCount, playerData);

                DestroyPlaybackAfterPlayEnd(_maxCueCount);
                return _maxCueCount;
            }
        }
    }

    /// <summary>音楽を管理するための機能を持ったインターフェース</summary>
    public interface ICustomChannel
    {
        /// <summary>ボリューム</summary>
        public IVolume Volume { get; }

        /// <summary>音楽を流す関数</summary>
        /// <param name="cueSheetName">流したいキューシートの名前</param>
        /// <param name="cueName">流したいキューの名前</param>
        /// <param name="volume">ボリューム</param>
        /// <returns>操作する際に必要なIndex</returns>
        public int Play(string cueSheetName, string cueName, float volume = 1.0F);

        /// <summary>音楽を流す関数(3D)</summary>
        /// <param name="playSoundWorldPos">流すPositionのWorldSpace</param>
        /// <param name="cueSheetName">流したいキューシートの名前</param>
        /// <param name="cueName">流したいキューの名前</param>
        /// <param name="volume">ボリューム</param>
        /// <returns>操作する際に必要なIndex</returns>
        public int Play3D(Vector3 playSoundWorldPos, string cueSheetName, string cueName, float volume = 1.0F);

        /// <summary>3Dの流すPositionを更新する</summary>
        /// <param name="playSoundWorldPos">更新するPosition</param>
        /// <param name="index">変更する音声のPlay時の戻り値(Index)</param>
        public void Update3DPos(Vector3 playSoundWorldPos, int index);

        /// <summary>音声をPauseさせる</summary>
        /// <param name="index">Pauseさせたい音声のPlay時の戻り値(Index)</param>
        public void Pause(int index);

        /// <summary>Pauseさせた音声をResumeさせる</summary>
        /// <param name="index">Resumeさせたい音声のPlay時の戻り値(Index)</param>
        public void Resume(int index);

        /// <summary>音声をStopさせる</summary>
        /// <param name="index">Stopさせたい音声のPlay時の戻り値(Index)</param>
        public void Stop(int index);

        /// <summary>すべての音声をStopさせる</summary>
        public void StopAll();

        /// <summary>ループしている音声すべてをStopさせる</summary>
        public void StopLoopCue();

        /// <summary>すべてのリスナーを設定する</summary>
        /// <param name="listener">リスナー</param>
        public void SetListenerAll(CriAtomListener listener);

        /// <summary>リスナーを設定する</summary>
        /// <param name="listener">リスナー</param>
        /// <param name="index">リスナーを変更したい音声のPlay時の戻り値</param>
        public void SetListener(CriAtomListener listener, int index);
    }

    /// <summary>BGMなどに使用する、一つの音のみを出力するチャンネル</summary>
    private class CriSingleChannel : AbstractCriChannel, ICustomChannel
    {
        public CriAtomExPlayer _test = new CriAtomExPlayer();
        /// <summary>現在再生中のAcb</summary>
        private CriAtomExAcb _currentAcb = null;

        /// <summary>現在再生中のCueName</summary>
        private string _currentCueName = "";

        /// <summary>コンストラクタ－</summary>
        /// <param name="masterVolume">マスターボリューム</param>
        public CriSingleChannel(Volume masterVolume) : base(masterVolume)
        {
            // TODO - Addに失敗したい際の処理を追加する
            _cueData.TryAdd(0, new CriPlayerData());
        }

        public IVolume Volume => _volume;

        public int Play(string cueSheetName, string cueName, float volume = 1.0F)
        {
            // CueSheetから情報を取得
            var tempAcb = CriAtom.GetAcb(cueSheetName);
            var tempPlayerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out CriAtomEx.CueInfo tempInfo);
            tempPlayerData.CueInfo = tempInfo;

            if (_currentAcb == tempAcb && _currentCueName == cueName
                                       && _player.GetStatus() == CriAtomExPlayer.Status.Playing)
            {
                // インデックスを返す
                return _cueData.Count - 1;
            }

            Stop(_cueData.Count - 1);

            // 情報をセットして再生
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
            // CueSheetから情報を取得
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

            // 座標情報をセットして再生
            var temp3dData = new CriAtomEx3dSource();

            temp3dData.SetPosition(playSoundWorldPos.x, playSoundWorldPos.y, playSoundWorldPos.z);
            // リスナーとソースを設定
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

        /// <summary>カスタム構造体を返すメソッド</summary>
        /// <param name="playerData">カスタム構造体を保持しているプレイヤー情報</param>
        public CriAtomCustomStruct GetCustomStruct(CriPlayerData playerData) => playerData.CustomStruct;

        /// <summary>再生終了を待つ（Unitask）</summary>
        public async UniTask WaitPlayingEnd(CancellationToken token, CriAtomCustomStruct customStruct)
        {
            // Trueを返すまで待つ
            await UniTask.WaitUntil(customStruct.CheckPlayingEnd, default, token, false);
        }

        /// <summary>再生終了を待つ（Coroutine）</summary>
        public System.Collections.IEnumerator WaitPlayingEndCor(CriAtomCustomStruct customStruct)
        {
            // Trueを返すまで待つ
            yield return new WaitUntil(customStruct.CheckPlayingEnd);
        }
    }

    /// <summary>SEなどに使用する、複数の音を出力するチャンネル</summary>
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
            // CueSheetから情報を取得
            var tempAcb = CriAtom.GetAcb(cueSheetName);
            var tempPlayerData = new CriPlayerData();
            tempAcb.GetCueInfo(cueName, out CriAtomEx.CueInfo tempInfo);
            tempPlayerData.CueInfo = tempInfo;

            // 座標情報をセットして再生
            var temp3dData = new CriAtomEx3dSource();

            temp3dData.SetPosition(playSoundWorldPos.x, playSoundWorldPos.y, playSoundWorldPos.z);
            // リスナーとソースを設定
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

        /// <summary>カスタム構造体を返すメソッド</summary>
        /// <param name="playerData">カスタム構造体を保持しているプレイヤー情報</param>
        public CriAtomCustomStruct GetCustomStruct(CriPlayerData playerData) => playerData.CustomStruct;

        /// <summary>再生終了を待つ（Unitask）</summary>
        public async UniTask WaitPlayingEnd(CancellationToken token, CriAtomCustomStruct customStruct)
        {
            // Trueを返すまで待つ
            await UniTask.WaitUntil(customStruct.CheckPlayingEnd, default, token, false);
        }

        /// <summary>再生終了を待つ（Coroutine）</summary>
        public System.Collections.IEnumerator WaitPlayingEndCor(CriAtomCustomStruct customStruct)
        {
            // Trueを返すまで待つ
            yield return new WaitUntil(customStruct.CheckPlayingEnd);
        }
    }

    /// <summary>CriAtomExPlaybackをラップしたオリジナル構造体</summary>
    public struct CriAtomCustomStruct
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

        public CriAtomCustomStruct(uint id) : this()
        {
            this.id = id;
#if CRIWARE_ENABLE_HEADLESS_MODE
            this._dummyStatus = Status.Prep;
#endif
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
}

#endif