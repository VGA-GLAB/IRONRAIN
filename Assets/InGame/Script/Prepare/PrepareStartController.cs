using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PrepareStartController : MonoBehaviour
{
    [SerializeField] private float _firstDelayTime = 10;
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Missile_Hit";
    
    public async UniTask PrepareStartAsync(CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(_firstDelayTime, cancellationToken: cancellationToken);

        CriAudioManager.Instance.SE.Play(_announceCueSheetName, _announceCueName);
    }
}
