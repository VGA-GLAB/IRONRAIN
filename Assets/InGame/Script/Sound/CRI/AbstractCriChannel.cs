using CriWare;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class AbstractCriChannel
{
    protected CriAtomExPlayer _player = new();

    /// <summary>キューのPlayback</summary>
    protected ConcurrentDictionary<int, CriPlayerData> _cueData = new();

    /// <summary>現在までの最大の_cuDataのカウント</summary>
    protected int _currentMaxCount;

    /// <summary>_cueDataのリムーブされたインデックス</summary>
    protected ConcurrentBag<int> _removedCueDataIndex = new();

    /// <summary>リスナー</summary>
    protected CriAtomEx3dListener _listener;

    /// <summary>ボリューム</summary>
    protected Volume _volume = new();

    /// <summary>マスターボリューム</summary>
    protected Volume _masterVolume;

    /// <summary>CancellationTokenSource</summary>
    private CancellationTokenSource _tokenSource = new();

    /// <summary>3DSource</summary>
    protected List<CriAtomEx3dSource> _3dSources = new();

    protected AbstractCriChannel(in Volume masterVolume)
    {
        _masterVolume = masterVolume;

        _volume.OnVolumeChanged += UpdateVolume;
        _masterVolume.OnVolumeChanged += UpdateMasterVolume;
    }

    ~AbstractCriChannel()
    {
        _tokenSource.Cancel();
        _volume.OnVolumeChanged -= UpdateVolume;
        _masterVolume.OnVolumeChanged -= UpdateMasterVolume;
        _player.Dispose();

        foreach (var source in _3dSources)
        {
            source.Dispose();
        }

        foreach (var VARIABLE in _cueData)
        {
            VARIABLE.Value.CancellationTokenSource.Cancel();
            VARIABLE.Value.Source.Dispose();
        }
    }

    private void UpdateVolume(float volume)
    {
        _player.SetVolume(volume * _masterVolume);

        foreach (var data in _cueData)
        {
            _player.Update(data.Value.Playback);
        }
    }

    private void UpdateMasterVolume(float masterVolume)
    {
        _player.SetVolume(_volume * masterVolume);

        foreach (var data in _cueData)
        {
            _player.Update(data.Value.Playback);
        }
    }

    protected int CueDataAdd(CriPlayerData playerData)
    {
        if (playerData.IsLoop)
        {
            if (_removedCueDataIndex.Count > 0)
            {
                int tempIndex;
                if (_removedCueDataIndex.TryTake(out tempIndex))
                {
                    _cueData.TryAdd(tempIndex, playerData);
                }

                return tempIndex;
            }
            else
            {
                _currentMaxCount++;
                _cueData.TryAdd(_currentMaxCount, playerData);
                return _currentMaxCount;
            }
        }
        else if (_removedCueDataIndex.Count > 0)
        {
            int tempIndex;
            if (_removedCueDataIndex.TryTake(out tempIndex))
            {
                _cueData.TryAdd(tempIndex, playerData);
            }

            PlaybackDestroyWaitForPlayEnd(tempIndex, playerData.CancellationTokenSource.Token).Forget();
            return tempIndex;
        }
        else
        {
            _currentMaxCount++;
            _cueData.TryAdd(_currentMaxCount, playerData);

            PlaybackDestroyWaitForPlayEnd(_currentMaxCount, playerData.CancellationTokenSource.Token).Forget();
            return _currentMaxCount;
        }
    }

    protected async UniTaskVoid PlaybackDestroyWaitForPlayEnd(int index, CancellationToken cancellationToken)
    {
        // ループしていたら抜ける
        if (_cueData[index].IsLoop)
        {
            return;
        }

        while (_cueData[index].Playback.GetStatus() == CriAtomExPlayback.Status.Playing)
        {
            await UniTask.WaitForSeconds(_cueData[index].CueInfo.length / 1000F,
                cancellationToken: _cueData[index].CancellationTokenSource.Token);
        }

        if (_cueData.TryRemove(index, out CriPlayerData outData) && !outData.IsLoop)
        {
            _removedCueDataIndex.Add(index);
        }
    }
}