using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UniRx;

public class PlayerQTEModel : IPlayerStateModel
{
    public IReactiveProperty<QTEState> QTEType => _qteType;

    private PlayerEnvroment _playerEnvroment;
    private PlayerSetting.PlayerParams _playerParams;
    private ReactiveProperty<QTEState> _qteType = new();

    public void Dispose()
    {
        _qteType.Dispose();   
    }

    public void FixedUpdate()
    {
        
    }

    public void SetUp(PlayerEnvroment env, CancellationToken token)
    {
        _playerEnvroment = env;
        _playerParams = _playerEnvroment.PlayerSetting.PlayerParamsData;
    }

    public void Start()
    {

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            StartQTE();
        }
    }

    public void StartQTE()
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
            _qteType.Value = QTEState.QTE1;
            //右レバーボタン1を押したまま右レバーを引く
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            _qteType.Value = QTEState.QTE2;
            //右レバーボタン1を押したまま右レバーを押す
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            _qteType.Value = QTEState.QTE3;
            //右レバーボタン2を押す
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton2), PlayerLoopTiming.Update, startToken);
            _qteType.Value = QTEState.QTENone;

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
        //失敗までの時間を計測
        await UniTask.WaitForSeconds(_playerParams.QteTimeLimit, true, PlayerLoopTiming.Update, endToken);
        Debug.Log("QTE終了");
        ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Failure);
        ProvidePlayerInformation.TimeScale = 1f;
        _playerEnvroment.RemoveState(PlayerStateType.QTE);
        startCts.Cancel();
    }
}
