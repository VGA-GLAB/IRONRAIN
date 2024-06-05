using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerStoryEvent : PlayerComponentBase
{
    [SerializeField] private TutorialTextBoxController _tutorialTextBoxController;
    [SerializeField] private Transform _bossBattleStartPos;

    /// <summary>
    /// ジェットパックをパージ
    /// </summary>
    /// <returns></returns>
    public async UniTask StartJetPackPurge()
    {
        var token = this.GetCancellationTokenOnDestroy();
        _playerEnvroment.ClearState();
        _playerEnvroment.AddState(PlayerStateType.Inoperable);
        await _tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, token);
        await _tutorialTextBoxController.DoTextChangeAsync("toggle3を押せ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle3), PlayerLoopTiming.Update, token);
        await _tutorialTextBoxController.DoTextChangeAsync("toggle4を押せ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), PlayerLoopTiming.Update, token);
        //右レバーボタン1を押したまま右レバーを押す
        await _tutorialTextBoxController.DoTextChangeAsync("両方のレバーを前に上げろ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeLever), PlayerLoopTiming.Update, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourLever), PlayerLoopTiming.Update, token);
        await _tutorialTextBoxController.DoCloseTextBoxAsync(0.5f, token);
        Debug.Log("パージ成功");
        //await _playerAnimation.JetpackPurgeAnim();
    }

    /// <summary>
    /// 落下をスタートする
    /// </summary>
    public async UniTask StartFall()
    {
        _playerEnvroment.AddState(PlayerStateType.Inoperable);
        await _playerEnvroment.PlayerAnimation.FallAnim();
    }

    /// <summary>
    /// ボス戦をスタートする
    /// </summary>
    public void BossStart() 
    {
        _playerEnvroment.SeachState<PlayerTrackingPhaseMove>().enabled = false;
        _playerEnvroment.SeachState<PlayerMove>().enabled = true;
        _playerEnvroment.ClearState();
        _playerEnvroment.PlayerTransform.position = _bossBattleStartPos.position;
    }
}
