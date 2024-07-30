using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronRain.SequenceSystem
{
    public class LoadSceneSequence : ISequence
    {
        [Header("読み込むシーンの名前"), SerializeField] private string _loadSceneName;
        
        public void SetData(SequenceData data)
        {
            
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            await SceneManager.LoadSceneAsync(_loadSceneName).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            SceneManager.LoadScene(_loadSceneName);
        }
    }
}
