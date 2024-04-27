using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;
using System.Threading;
using Enemy.Control;

public class PlayerQTE : PlayerComponentBase
{
    protected override void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartQTE();
        }
    }

    private void StartQTE()
    {
        var startCts = new CancellationTokenSource();
        var startToken = startCts.Token;
        var endCts = new CancellationTokenSource();
        var endToken = endCts.Token;
        QTE(endCts, startToken).Forget();
        QTEFailureJudgment(startCts, endToken).Forget();
    }

    /// <summary>
    /// QTEの流れ
    /// </summary>
    /// <param name="endCts"></param>
    /// <param name="startToken"></param>
    /// <returns></returns>
    public async UniTask QTE(CancellationTokenSource endCts, CancellationToken startToken) 
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            _playerEnvroment.AddState(PlayerStateType.QTE);
            Debug.Log("QTEモード");
            ProvidePlayerInformation.TimeScale = 0.2f;
            ProvidePlayerInformation.StartQte.OnNext(UniRx.Unit.Default);
            //右レバーボタン1を押したまま右レバーを引く
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            Debug.Log("その１");
            //右レバーボタン1を押したまま右レバーを押す
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            Debug.Log("その2");
            //右レバーボタン2を押す
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton2), PlayerLoopTiming.Update, startToken);
            Debug.Log("その3");

            ProvidePlayerInformation.TimeScale = 1f;
            ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Success);
            _playerEnvroment.RemoveState(PlayerStateType.QTE);
            Debug.Log("QTEキャンセル");
            endCts.Cancel();
        }
    }

    /// <summary>
    /// QTEの失敗判定
    /// </summary>
    /// <returns></returns>
    private async UniTask QTEFailureJudgment(CancellationTokenSource startCts, CancellationToken endToken) 
    {
        await UniTask.WaitForSeconds(_playerParams.QteTimeLimit, true, PlayerLoopTiming.Update, endToken);
        Debug.Log("QTE終了");
        ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Failure);
        ProvidePlayerInformation.TimeScale = 1f;
        _playerEnvroment.RemoveState(PlayerStateType.QTE);
        startCts.Cancel();
    }


    public override void Dispose()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        var enemyTypeReader =  other.GetComponentsInParent<IEnemyTypeReader>();
        if (enemyTypeReader.Length == 0) return;
        //盾持ちの敵が入ってきたら
        if (enemyTypeReader[0].Type == EnemyType.Shield) 
        {
            Debug.Log("入った");
            StartQTE();
        }
    }
}
