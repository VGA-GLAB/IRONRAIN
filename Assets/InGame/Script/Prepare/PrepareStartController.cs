using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PrepareStartController : MonoBehaviour
{
    [SerializeField] private float _firstDelayTime = 10;
    
    public async UniTask PrepareStartAsync(CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(_firstDelayTime, cancellationToken: cancellationToken);
        
    }
}
