using CriWare;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>BGMなどに使用する、一つの音のみを出力するチャンネル</summary>
public class CriSingleChannel : AbstractCriChannel, ICustomChannel
{
    /// <summary>現在再生中のAcb</summary>
    private CriAtomExAcb _currentAcb = null;

    /// <summary>現在再生中のCueSheetName</summary>
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
        _3dSources.Add(temp3dData);

        _player.SetCue(tempAcb, cueName);
        _player.SetVolume(_volume * _masterVolume * volume);
        _player.SetStartTime(0L);
        tempPlayerData.Playback = _player.Start();

        _cueData[_cueData.Count - 1] = tempPlayerData;

        return _cueData.Count - 1;
    }

#if AUDIO_MANAGER_UNITASK_SUPPORT

    public async UniTask PlayAsync(string cueSheetName, string cueName, CancellationToken cancellationToken,
        float volume = 1)
    {
        int index = Play(cueSheetName, cueName, volume);
        float waitSec = _cueData[index].CueInfo.length / 1000F;
        await UniTask.WaitForSeconds(waitSec, cancellationToken: cancellationToken);
    }

    public async UniTask Play3DAsync(Vector3 playSoundWorldPos, string cueSheetName, string cueName,
        CancellationToken cancellationToken, float volume = 1)
    {
        int index = Play3D(playSoundWorldPos, cueSheetName, cueName, volume);
        float waitSec = _cueData[index].CueInfo.length / 1000F;
        await UniTask.WaitForSeconds(waitSec, cancellationToken: cancellationToken);
    }

#endif

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

    public void SetAisac(string controlName, float value)
    {
        _player.SetAisacControl(controlName, value);
        _player.UpdateAll();
    }
}