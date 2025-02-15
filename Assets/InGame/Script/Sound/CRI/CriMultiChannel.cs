using CriWare;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>SEなどに使用する、複数の音を出力するチャンネル</summary>
public class CriMultiChannel : AbstractCriChannel, ICustomChannel
{
    public CriMultiChannel(in Volume masterVolume) : base(in masterVolume) { }

        public IVolume Volume => _volume;

        public int Play(string cueSheetName, string cueName, float volume)
        {
            if (cueName == "") return -1;

            CriAtomEx.CueInfo cueInfo;
            CriPlayerData newAtomPlayer = new();

            var tempAcb = CriAtom.GetAcb(cueSheetName);
            tempAcb.GetCueInfo(cueName, out cueInfo);

            newAtomPlayer.CueInfo = cueInfo;

            _player.SetCue(tempAcb, cueName);
            _player.Set3dSource(null);
            _player.SetVolume(volume * _volume * _masterVolume);
            newAtomPlayer.Playback = _player.Start();
            newAtomPlayer.CancellationTokenSource = new CancellationTokenSource();

            return CueDataAdd(newAtomPlayer);
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
            temp3dData.Update();
            // リスナーとソースを設定
            _player.Set3dListener(_listener);
            _player.Set3dSource(temp3dData);
            tempPlayerData.Source = temp3dData;
            _3dSources.Add(temp3dData);
            
            _player.SetCue(tempAcb, cueName);
            _player.SetVolume(_volume * _masterVolume * volume);
            _player.SetStartTime(0L);
            tempPlayerData.Playback = _player.Start();
            tempPlayerData.CancellationTokenSource = new CancellationTokenSource();

            return CueDataAdd(tempPlayerData);
        }
        
        #if AUDIO_MANAGER_UNITASK_SUPPORT

        public async UniTask PlayAsync(string cueSheetName, string cueName, CancellationToken cancellationToken, float volume = 1)
        {
            int index = Play(cueSheetName, cueName, volume);
            float waitSec = _cueData[index].CueInfo.length / 1000F;
            await UniTask.WaitForSeconds(waitSec, cancellationToken: cancellationToken);
        }

        public async UniTask Play3DAsync(Vector3 playSoundWorldPos, string cueSheetName, string cueName, CancellationToken cancellationToken, float volume = 1)
        {
            int index = Play3D(playSoundWorldPos, cueSheetName, cueName, volume);
            float waitSec = _cueData[index].CueInfo.length / 1000f;
            await UniTask.WaitForSeconds(waitSec, cancellationToken: cancellationToken);
        }
        
        #endif

        public void Update3DPos(Vector3 playSoundWorldPos, int index)
        {
            if (!_cueData.ContainsKey(index))
            {
                Debug.LogError($"Index : {index} に対応する音が_cueData内に存在しません(MultiChannel)");
                return;
            }
            if (_player.GetStatus() == CriAtomExPlayer.Status.PlayEnd)
            {
                Debug.LogWarning($"{_cueData[index].CueInfo.name} の再生が終了しているため、音源の位置更新を停止しています(MultiChannel)");
                return;
            }
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
                //outData.Source?.Dispose();
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
                    //outData.Source?.Dispose();
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
        
        public void SetAisac(string controlName, float value)
        {
            _player.SetAisacControl(controlName, value);
            _player.UpdateAll();
        }
        
        public void Reset3DPlayer()
        {
            StopAll();
            _player.Set3dSource(null);
        }
}