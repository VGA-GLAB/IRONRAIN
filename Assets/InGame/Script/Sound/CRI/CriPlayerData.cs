using CriWare;
using System.Threading;
using UnityEngine;

public struct CriPlayerData
{
    /// <summary>再生中の音声のPlayback</summary>
    public CriAtomExPlayback Playback { get; set; }

    /// <summary>再生中のCueに関する情報</summary>
    public CriAtomEx.CueInfo CueInfo { get; set; }

    public CriAtomEx3dSource Source { get; set; }

    public float LastUpdateTime { get; set; }

    public bool IsLoop => CueInfo.length < 0;

    /// <summary>ポジションを更新する & 進行方向の予想ベクトルを返す</summary>
    /// <param name="nextPos">次のポジション</param>
    /// <returns>一秒間に進む予想ベクトル</returns>
    public void UpdateCurrentVector(Vector3 nextPos)
    {
        //前回のアップデートからの経過時間
        var elapsed = Playback.GetTime() - LastUpdateTime;

        //ポジションからベクトルを算出
        CriAtomEx.NativeVector nativePos = Source.GetPosition();
        Vector3 currentPos = new Vector3(nativePos.x, nativePos.y, nativePos.z);
        Vector3 movedVec = nextPos - currentPos;
        movedVec /= elapsed;

        LastUpdateTime = Playback.GetTime();
        Source.SetPosition(nextPos.x, nextPos.y, nextPos.z);
        Source.SetVelocity(movedVec.x, movedVec.y, movedVec.z);
        Source.Update();
    }
        
    public CancellationTokenSource CancellationTokenSource { get; set; }
}