using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class PlayerStoryEvent : PlayerComponentBase
{
    [SerializeField] private Transform _bossBattleStartPos;
    [SerializeField] private Transform _centerPoint;
    [SerializeField] private List<LeverController> _leverCon;

    /// <summary>
    /// ジェットパックをパージ
    /// </summary>
    /// <returns></returns>
    public async UniTask StartJetPackPurge()
    {
        var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;
        var token = this.GetCancellationTokenOnDestroy();
        _playerEnvroment.ClearState();
        _playerEnvroment.AddState(PlayerStateType.Inoperable);
        await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, token);
        await tutorialTextBoxController.DoTextChangeAsync("F3を押せ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle3), PlayerLoopTiming.Update, token);
        CriAudioManager.Instance.SE.Play("SE", "SE_Purge");
        await tutorialTextBoxController.DoTextChangeAsync("F4を押せ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), PlayerLoopTiming.Update, token);
        CriAudioManager.Instance.SE.Play("SE", "SE_Purge");
        //右レバーボタン1を押したまま右レバーを押す
        await tutorialTextBoxController.DoTextChangeAsync("WボタンとShiftボタンを押せ", 0.5f, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.ThreeLever), PlayerLoopTiming.Update, token);
        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourLever), PlayerLoopTiming.Update, token);
        CriAudioManager.Instance.SE.Play("SE", "SE_Purge");
        await tutorialTextBoxController.DoCloseTextBoxAsync(0.5f, token);
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
    /// チェイスシーン始める
    /// </summary>
    public void StartChaseScene()
    {
        _playerEnvroment.SeachState<PlayerTrackingPhaseMove>().enabled = true;
        _playerEnvroment.SeachState<PlayerQTE>().enabled = true;
        _playerEnvroment.SeachState<PlayerWeaponController>().enabled = true;
        for (int i = 0; i < _leverCon.Count; i++)  
        {
            _leverCon[i].enabled = true;
        }
    }

    /// <summary>
    /// ボス戦をスタートする
    /// </summary>
    public void BossStart() 
    {
        _playerEnvroment.SeachState<PlayerTrackingPhaseMove>().enabled = false;
        _playerEnvroment.SeachState<PlayerBossMove>().enabled = true;
        _playerEnvroment.ClearState();
    }

    /// <summary>
    /// 真ん中についたかどうか
    /// </summary>
    public bool GoalCenterPoint()
    {
        if (_playerEnvroment.PlayerTransform.position.z - 20 < _centerPoint.position.z
            && _playerEnvroment.PlayerTransform.position.z + 30 > _centerPoint.position.z)
        {
            Debug.Log("ついた");
            return true;
        }
        return false;
    }
}
