using CriWare;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>音楽を管理するための機能を持ったInterface</summary>
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
        
        #if AUDIO_MANAGER_UNITASK_SUPPORT

        public UniTask PlayAsync(string cueSheetName, string cueName, CancellationToken cancellationToken, float volume = 1.0F);

        public UniTask Play3DAsync(Vector3 playSoundWorldPos, string cueSheetName, string cueName, CancellationToken cancellationToken,float volume = 1.0F);
        
        #endif

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

        /// <summary>AISACの値を設定する</summary>
        /// <param name="value"></param>
        public void SetAisac(string controlName, float value);
    }