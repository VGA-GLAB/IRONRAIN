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
            StartQTE().Forget();
        }
    }

    public async UniTask<QTEResultType> StartQTE()
    {
        QTEResultType qteResult = QTEResultType.Failure;
        var startCts = new CancellationTokenSource();
        var startToken = startCts.Token;
        var endCts = new CancellationTokenSource();
        var endToken = endCts.Token;
        qteResult = await QTE(endCts, startToken);
        qteResult = await QTEFailureJudgment(startCts, endToken);
        return qteResult;
    }

    /// <summary>
    /// QTEの流れ
    /// </summary>
    /// <param name="endCts"></param>
    /// <param name="startToken"></param>
    /// <returns></returns>
    public async UniTask<QTEResultType> QTE(CancellationTokenSource endCts, CancellationToken startToken)
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            _playerEnvroment.AddState(PlayerStateType.QTE);

            ProvidePlayerInformation.TimeScale = 0.2f;
            ProvidePlayerInformation.StartQte.OnNext(UniRx.Unit.Default);
            var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

            _qteType.Value = QTEState.QTE1;
            await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, startToken);
            await tutorialTextBoxController.DoTextChangeAsync("Qボタンを押したまま右レバーまたはマウスホイールを手前に引いた状態にしろ", 0.5f, startToken);
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            await tutorialTextBoxController.DoTextChangeAsync("Qボタンを押したまま右レバーまたはマウスホイールを奥に押し出すように傾けろ", 0.5f, startToken);
            _qteType.Value = QTEState.QTE2;
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            await tutorialTextBoxController.DoTextChangeAsync("左レバーの後ろのボタンか右クリックを押せ", 0.5f, startToken); 
            _qteType.Value = QTEState.QTE3;
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, startToken);
            _qteType.Value = QTEState.QTENone;

            ProvidePlayerInformation.TimeScale = 1f;
            ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Success);
            _playerEnvroment.RemoveState(PlayerStateType.QTE);
            endCts.Cancel();
            await tutorialTextBoxController.DoTextChangeAsync("成功です", 0.5f, startToken);
            tutorialTextBoxController.DoCloseTextBoxAsync(0.5f, startToken).Forget();
            return QTEResultType.Success;
        }
        return QTEResultType.Failure;
    }

    /// <summary>
    /// つばぜり合い
    /// </summary>
    /// <param name="endCts"></param>
    /// <param name="startToken"></param>
    /// <returns></returns>
    public async UniTask BossQTE1(CancellationTokenSource endCts, CancellationToken startToken)
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            _playerEnvroment.AddState(PlayerStateType.QTE);

            ProvidePlayerInformation.TimeScale = 0.2f;
            ProvidePlayerInformation.StartQte.OnNext(UniRx.Unit.Default);
            var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

            _qteType.Value = QTEState.QTE1;
            await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, startToken);
            await tutorialTextBoxController.DoTextChangeAsync("Qボタンを押したまま右レバーまたはマウスホイールを奥に押した状態にする", 0.5f, startToken);
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            await tutorialTextBoxController.DoTextChangeAsync("Qボタンを押したまま右レバーまたはマウスホイールを思いっきり手前に引く", 0.5f, startToken);
            _qteType.Value = QTEState.QTE2;
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            ProvidePlayerInformation.TimeScale = 1f;
            ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Success);
            _playerEnvroment.RemoveState(PlayerStateType.QTE);
            Debug.Log("QTEキャンセル");
            endCts.Cancel();
        }
    }

    public async UniTask BossQTE2(CancellationTokenSource endCts, CancellationToken startToken)
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            _playerEnvroment.AddState(PlayerStateType.QTE);

            ProvidePlayerInformation.TimeScale = 0.2f;
            ProvidePlayerInformation.StartQte.OnNext(UniRx.Unit.Default);
            var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

            _qteType.Value = QTEState.QTE1;
            await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, startToken);
            await tutorialTextBoxController.DoTextChangeAsync("Qボタンを押したまま右レバーまたはマウスホイールを奥に押した状態にする", 0.5f, startToken);
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            await tutorialTextBoxController.DoTextChangeAsync("左クリックを押したまま右レバーまたはマウスホイールを思いっきり手前に引く", 0.5f, startToken);
            _qteType.Value = QTEState.QTE2;
            await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeButton), PlayerLoopTiming.Update, startToken);

            await tutorialTextBoxController.DoTextChangeAsync("左レバーの後ろのボタンか右クリックを押せ", 0.5f, startToken);
            _qteType.Value = QTEState.QTE3;
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, startToken);
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
    private async UniTask<QTEResultType> QTEFailureJudgment(CancellationTokenSource startCts, CancellationToken endToken)
    {
        //失敗までの時間を計測
        await UniTask.WaitForSeconds(_playerParams.QteTimeLimit, true, PlayerLoopTiming.Update, endToken);
        Debug.Log("QTE終了");
        ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Failure);
        ProvidePlayerInformation.TimeScale = 1f;
        _qteType.Value = QTEState.QTENone;
        _playerEnvroment.RemoveState(PlayerStateType.QTE);
        startCts.Cancel();
        return QTEResultType.Failure;
    }
}
