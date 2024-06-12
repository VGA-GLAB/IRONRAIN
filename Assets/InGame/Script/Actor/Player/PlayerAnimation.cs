using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _fallObj;
    [SerializeField] private Transform _fallInsPos;
    private CancellationToken _token;


    private void Awake()
    {
        _token = this.GetCancellationTokenOnDestroy();
    }

    public async UniTask JetpackPurgeAnim() 
    {
        _anim.Play("Purge");
        await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
        PlayerLoopTiming.Update, _token);
    }

    public async UniTask FallAnim() 
    {
        var obj = Instantiate(_fallObj, _fallInsPos);
        Animator anim = obj.GetComponent<Animator>();
        await UniTask.WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
        PlayerLoopTiming.Update, _token);
        obj.SetActive(false);
    }
}
