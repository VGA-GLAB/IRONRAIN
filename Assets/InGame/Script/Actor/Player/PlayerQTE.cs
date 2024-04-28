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
    /// QTE�̗���
    /// </summary>
    /// <param name="endCts"></param>
    /// <param name="startToken"></param>
    /// <returns></returns>
    public async UniTask QTE(CancellationTokenSource endCts, CancellationToken startToken) 
    {
        if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
        {
            _playerEnvroment.AddState(PlayerStateType.QTE);
            Debug.Log("QTE���[�h");
            ProvidePlayerInformation.TimeScale = 0.2f;
            ProvidePlayerInformation.StartQte.OnNext(UniRx.Unit.Default);
            //�E���o�[�{�^��1���������܂܉E���o�[������
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == -1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            Debug.Log("���̂P");
            //�E���o�[�{�^��1���������܂܉E���o�[������
            await UniTask.WaitUntil(() => InputProvider.Instance.RightLeverDir.z == 1
            && InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton1), PlayerLoopTiming.Update, startToken);
            Debug.Log("����2");
            //�E���o�[�{�^��2������
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.RightButton2), PlayerLoopTiming.Update, startToken);
            Debug.Log("����3");

            ProvidePlayerInformation.TimeScale = 1f;
            ProvidePlayerInformation.EndQte.OnNext(QTEResultType.Success);
            _playerEnvroment.RemoveState(PlayerStateType.QTE);
            Debug.Log("QTE�L�����Z��");
            endCts.Cancel();
        }
    }

    /// <summary>
    /// QTE�̎��s����
    /// </summary>
    /// <returns></returns>
    private async UniTask QTEFailureJudgment(CancellationTokenSource startCts, CancellationToken endToken) 
    {
        await UniTask.WaitForSeconds(_playerParams.QteTimeLimit, true, PlayerLoopTiming.Update, endToken);
        Debug.Log("QTE�I��");
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
        //�������̓G�������Ă�����
        if (enemyTypeReader[0].Type == EnemyType.Shield) 
        {
            Debug.Log("������");
            StartQTE();
        }
    }
}
