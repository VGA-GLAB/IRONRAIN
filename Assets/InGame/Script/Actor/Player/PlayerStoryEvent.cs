using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using IronRain.Player;

namespace IronRain.Player
{
    public class PlayerStoryEvent : PlayerComponentBase
    {
        [SerializeField] private Transform _bossBattleStartPos;
        [SerializeField] private Transform _centerPoint;
        [SerializeField] private List<LeverController> _leverCon;
        [Header("パージして着地するまでの時間")]
        [SerializeField] private float _purgeDuration;

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
            await tutorialTextBoxController.DoTextChangeAsync("[F3]を押してください。", 0.05f, token);
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle3), PlayerLoopTiming.Update, token);
            CriAudioManager.Instance.SE.Play("SE", "SE_Purge");
            await tutorialTextBoxController.DoTextChangeAsync("[F4]を押してください。", 0.05f, token);
            await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), PlayerLoopTiming.Update, token);
            CriAudioManager.Instance.SE.Play("SE", "SE_Purge");
            //右レバーボタン1を押したまま右レバーを押す
            await tutorialTextBoxController.DoTextChangeAsync("[s]と[z]を同時に押してください！", 0.05f, token);
            await UniTask.WaitUntil(() => InputProvider.Instance.ThreeLeverDir.y == -1 && InputProvider.Instance.FourLeverDir.y == -1, PlayerLoopTiming.Update, token);
            // UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourLever), PlayerLoopTiming.Update, token);
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
            //ボス戦の中心点をPlayerの真下に移動させる
            _bossBattleStartPos.transform.position = new Vector3
                (_playerEnvroment.PlayerTransform.position.x,
                _bossBattleStartPos.position.y, 
                _playerEnvroment.PlayerTransform.position.z + 20);

            CriAudioManager.Instance.SE.Play("SE", "SE_Fall");
            _playerEnvroment.AddState(PlayerStateType.Inoperable);
            _playerEnvroment.PlayerTransform.SetParent(_bossBattleStartPos.transform);
            var endPos = new Vector3(0, 0, -20);
            await _playerEnvroment.PlayerTransform.DOLocalMove(endPos, _purgeDuration).SetLink(this.gameObject);
            CriAudioManager.Instance.SE.Play("SE", "SE_Landing");
            //await _playerEnvroment.PlayerAnimation.FallAnim();
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
}
