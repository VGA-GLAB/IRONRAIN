using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IronRain.SequenceSystem
{
    public class LoadSceneSequence : ISequence
    {
        [SerializeField] private string _loadScenenName;
        
        public void SetData(SequenceData data)
        {
            
        }

        public async UniTask PlayAsync(CancellationToken ct)
        {
            await SceneManager.LoadSceneAsync(_loadScenenName).ToUniTask(cancellationToken: ct);
        }

        public void Skip()
        {
            SceneManager.LoadScene(_loadScenenName);
        }
    }
}
