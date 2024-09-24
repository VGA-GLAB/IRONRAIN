using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronRain.SequenceSystem
{
    public class LoadSceneSequence : ISequence
    {
        [OpenScriptButton(typeof(LoadSceneSequence))]
        [Description("次のシーンを読み込むためのシーケンス")]
        [Header("読み込むシーンの名前"), SerializeField] private string _loadSceneName;
        
        public void SetData(SequenceData data)
        {
            
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 音の停止処理
            CriAudioManager.Instance.SE.StopAll();
            CriAudioManager.Instance.CockpitSE.StopAll();
            CriAudioManager.Instance.BGM.StopAll();
            CriAudioManager.Instance.AmbientBGM.StopAll();
            CriAudioManager.Instance.Voice.StopAll();

            await SceneManager.LoadSceneAsync(_loadSceneName).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            SceneManager.LoadScene(_loadSceneName);
        }
    }
}
