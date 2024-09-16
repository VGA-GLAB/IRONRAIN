using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UniRx;
using DG.Tweening;

namespace IronRain.Player
{
    public class PlayerQTEModel : IPlayerStateModel
    {
        public IReactiveProperty<QTEState> QTEType => _qteType;
        
        [SerializeField] private Transform _qteStartPos;

        private PlayerEnvroment _playerEnvroment;
        private PlayerSetting.PlayerParams _playerParams;
        private ReactiveProperty<QTEState> _qteType = new();
        private QTEResultType _qteResultType;
        private Guid _enemyId;
        private PlayerSetting.PlayerParams _params;

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
            _params = _playerEnvroment.PlayerSetting.PlayerParamsData;
            _qteType.Value = QTEState.QTENone;
        }

        public void Start()
        {

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                //StartQTE().Forget();
            }
        }

        public async UniTask<QTEResultType> StartQTE(System.Guid enemyId, QteType qteType)
        {
            _enemyId = enemyId;
            QTEResultType qteResult = QTEResultType.Failure;
            var startCts = new CancellationTokenSource();
            var startToken = startCts.Token;
            var endCts = new CancellationTokenSource();
            var endToken = endCts.Token;

            //QTEFailureJudgment(startCts, endToken).Forget();

            try
            {
                if (qteType == QteType.NormalQte)
                {
                    qteResult = await QTE(startToken);
                }
                else if (qteType == QteType.BossQte1)
                {
                    qteResult = await BossQTE1(startToken);
                }
                else if (qteType == QteType.BossQte2)
                {
                    qteResult = await BossQTE2(startToken);
                }

            }
            catch
            {
                return qteResult;
            }

            endCts.Cancel();
            return qteResult;
        }

        /// <summary>
        /// QTEの流れ
        /// </summary>
        /// <param name="endCts"></param>
        /// <param name="startToken"></param>
        /// <returns></returns>
        public async UniTask<QTEResultType> QTE(CancellationToken startToken)
        {
            _qteResultType = QTEResultType.Failure;

            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
            {
                _playerEnvroment.AddState(PlayerStateType.QTE);

                //ProvidePlayerInformation.TimeScale = 0.2f;
                ProvidePlayerInformation.StartQte.OnNext(_enemyId);
                var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

                _qteType.Value = QTEState.QTE1;
                await tutorialTextBoxController.DoOpenTextBoxAsync(0.05f, startToken);
                await tutorialTextBoxController.DoTextChangeAsync("左レバーを引いてください。", 0.05f, startToken);
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, startToken);
                tutorialTextBoxController.ClearText();
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();

                _playerEnvroment.PlayerAnimation.QteAttack(startToken).Forget();

                await tutorialTextBoxController.DoOpenTextBoxAsync(0.05f, startToken);
                await tutorialTextBoxController.DoTextChangeAsync("左レバーを前に押し出してください。", 0.05f, startToken);
                _qteType.Value = QTEState.QTE2;
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, startToken);

                //次のアニメーションを再生して待機
                _playerEnvroment.PlayerAnimation.NextStopAnim(0.9f, startToken).Forget();

                tutorialTextBoxController.ClearText();
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();

                //Qte2の際に移動する
                await _playerEnvroment.PlayerTransform
                    .DOMoveZ(_playerEnvroment.PlayerTransform.position.z + _params.QteGoDistance, _params.QteGoDistanceTime)
                    .ToUniTask(cancellationToken: startToken);

                await tutorialTextBoxController.DoOpenTextBoxAsync(0.05f, startToken);
                await tutorialTextBoxController.DoTextChangeAsync("左レバーの[R2]を押してください。", 0.05f, startToken);

                _qteType.Value = QTEState.QTE3;
                await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, startToken);
                _qteType.Value = QTEState.QTENone;
                _playerEnvroment.PlayerAnimation.AnimationSpeedReset();
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();
                tutorialTextBoxController.ClearText();

                await _playerEnvroment.PlayerAnimation.PileFire(startToken);
                await _playerEnvroment.PlayerAnimation.PileFinish(startToken);

                ProvidePlayerInformation.TimeScale = 1f;
                ProvidePlayerInformation.EndQte.OnNext(new QteResultData(QTEResultType.Success, _enemyId));
                _playerEnvroment.RemoveState(PlayerStateType.QTE);
                await tutorialTextBoxController.DoTextChangeAsync("成功です", 0.05f, startToken);
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();

                _qteResultType = QTEResultType.Success;
                return _qteResultType;
            }
            return _qteResultType;
        }

        public async UniTask TutorialQteCallseparately(QTEState qteProgressType, CancellationToken token) 
        {

            switch (qteProgressType)
            {
                case QTEState.QTE1:
                    {
                        if (_qteType.Value != QTEState.QTENone) Debug.LogError("意図しないQteの呼び出しがされています");
                        ProvidePlayerInformation.StartQte.OnNext(_enemyId);

                        _qteType.Value = QTEState.QTE1;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, token);
                        await _playerEnvroment.PlayerAnimation.QteAttack(token);
                        break;
                    }
                case QTEState.QTE2:
                    {
                        if (_qteType.Value != QTEState.QTE1) Debug.LogError("意図しないQteの呼び出しがされています");

                        _qteType.Value = QTEState.QTE2;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, token);

                        //次のアニメーションを再生して待機
                        _playerEnvroment.PlayerAnimation.NextStopAnim(0.9f, token).Forget();

                        //Qte2の際に移動する
                        await _playerEnvroment.PlayerTransform
                            .DOMoveZ(_playerEnvroment.PlayerTransform.position.z + _params.QteGoDistance, _params.QteGoDistanceTime)
                            .ToUniTask(cancellationToken: token);
                        break;
                    }
                case QTEState.QTE3:
                    {
                        if (_qteType.Value != QTEState.QTE2) Debug.LogError("意図しないQteの呼び出しがされています");
                        _qteType.Value = QTEState.QTE3;
                        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, token);
                        _qteType.Value = QTEState.QTENone;
                        _playerEnvroment.PlayerAnimation.AnimationSpeedReset();

                        await _playerEnvroment.PlayerAnimation.PileFire(token);
                        await _playerEnvroment.PlayerAnimation.PileFinish(token);

                        ProvidePlayerInformation.EndQte.OnNext(new QteResultData(QTEResultType.Success, _enemyId));
                        _playerEnvroment.RemoveState(PlayerStateType.QTE);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// つばぜり合い
        /// </summary>
        /// <param name="endCts"></param>
        /// <param name="startToken"></param>
        /// <returns></returns>
        public async UniTask<QTEResultType> BossQTE1(CancellationToken startToken)
        {
            _playerEnvroment.AddState(PlayerStateType.EnterBossQte);
            _playerEnvroment.PlayerTransform.parent = null;

            _qteResultType = QTEResultType.Failure;
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
            {
                _playerEnvroment.AddState(PlayerStateType.QTE);

                ProvidePlayerInformation.TimeScale = 0.2f;
                ProvidePlayerInformation.StartQte.OnNext(_enemyId);
                var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

                _qteType.Value = QTEState.QTE1;
                await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, startToken);
                await tutorialTextBoxController.DoTextChangeAsync("左レバーを引いてください。", 0.05f, startToken);
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, startToken);
                await _playerEnvroment.PlayerAnimation.QteGuard(startToken);
                tutorialTextBoxController.ClearText();

                await tutorialTextBoxController.DoTextChangeAsync("左レバーを前に押し出してください。", 0.05f, startToken);
                _qteType.Value = QTEState.QTE2;
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, startToken);
                tutorialTextBoxController.ClearText();
                await _playerEnvroment.PlayerAnimation.QteGuard(startToken);

                ProvidePlayerInformation.TimeScale = 1f;
                ProvidePlayerInformation.EndQte.OnNext(new QteResultData(QTEResultType.Success, _enemyId));
                _playerEnvroment.RemoveState(PlayerStateType.QTE);
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();

                _qteResultType = QTEResultType.Success;
                return _qteResultType;
            }
            return _qteResultType;
        }


        public async UniTask BossQte1Callseparately(QTEState qteProgressType, CancellationToken token)
        {
            switch (qteProgressType)
            {
                //case QTEState.QtePreparation:
                //    {
                //        if (_qteType.Value != QTEState.QTENone) Debug.LogError("意図しないQteの呼び出しがされています");
                //        await _playerEnvroment.PlayerTransform.DOLocalMove(_qteStartPos.position, 2f)
                //            .ToUniTask(TweenCancelBehaviour.Kill, token);
                //        break;
                //    }
                case QTEState.QTE1:
                    {
                        if (_qteType.Value != QTEState.QTENone) Debug.LogError("意図しないQteの呼び出しがされています");
                        _playerEnvroment.AddState(PlayerStateType.EnterBossQte);
                        _qteType.Value = QTEState.QTE1;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, token);
                        break;
                    }
                case QTEState.QTE2:
                    {
                        if (_qteType.Value != QTEState.QTE1) Debug.LogError("意図しないQteの呼び出しがされています");
                        _qteType.Value = QTEState.QTE2;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, token);
                        _qteResultType = QTEResultType.Success;
                        _qteType.Value = QTEState.QTENone;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public async UniTask BossQte2Callseparately(QTEState qteProgressType, CancellationToken token) 
        {
            switch (qteProgressType)
            {
                case QTEState.QTE1:
                    {
                        if (_qteType.Value != QTEState.QTENone) Debug.LogError("意図しないQteの呼び出しがされています");
                        _qteType.Value = QTEState.QTE1;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, token);
                        _playerEnvroment.PlayerAnimation.QteAttack(token).Forget();
                        break;
                    }
                case QTEState.QTE2:
                    {
                        if (_qteType.Value != QTEState.QTE1) Debug.LogError("意図しないQteの呼び出しがされています");
                        _qteType.Value = QTEState.QTE2;
                        await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, token);
                        _playerEnvroment.PlayerAnimation.NextStopAnim(0.9f, token).Forget();
                        break;
                    }
                case QTEState.QTE3:
                    {
                        if (_qteType.Value != QTEState.QTE2) Debug.LogError("意図しないQteの呼び出しがされています");
                        _qteType.Value = QTEState.QTE3;
                        await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, token);
                        _qteResultType = QTEResultType.Success;
                        _qteType.Value = QTEState.QTENone;
                        await _playerEnvroment.PlayerAnimation.EndPileFire(token);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        public async UniTask<QTEResultType> BossQTE2(CancellationToken startToken)
        {
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE))
            {
                _playerEnvroment.AddState(PlayerStateType.QTE);

                ProvidePlayerInformation.TimeScale = 0.2f;
                ProvidePlayerInformation.StartQte.OnNext(_enemyId);
                var tutorialTextBoxController = _playerEnvroment.TutorialTextBoxCon;

                _qteType.Value = QTEState.QTE1;
                await tutorialTextBoxController.DoOpenTextBoxAsync(0.5f, startToken);
                await tutorialTextBoxController.DoTextChangeAsync("左レバーを奥に押し出してください。", 0.05f, startToken);
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == 1, PlayerLoopTiming.Update, startToken);
                tutorialTextBoxController.ClearText();

                await tutorialTextBoxController.DoTextChangeAsync("左レバーを手前に引いてください。", 0.05f, startToken);
                _qteType.Value = QTEState.QTE2;
                await UniTask.WaitUntil(() => InputProvider.Instance.LeftLeverDir.z == -1, PlayerLoopTiming.Update, startToken);
                tutorialTextBoxController.ClearText();

                await tutorialTextBoxController.DoTextChangeAsync("左レバーの[R2]を押してください。", 0.05f, startToken);
                _qteType.Value = QTEState.QTE3;
                await UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.FourButton), PlayerLoopTiming.Update, startToken);
                _qteType.Value = QTEState.QTENone;
                tutorialTextBoxController.ClearText();

                ProvidePlayerInformation.TimeScale = 1f;
                ProvidePlayerInformation.EndQte.OnNext(new QteResultData(QTEResultType.Success, _enemyId));
                _playerEnvroment.RemoveState(PlayerStateType.QTE);
                tutorialTextBoxController.DoCloseTextBoxAsync(0.05f, startToken).Forget();

                _qteResultType = QTEResultType.Success;
                return _qteResultType;
            }
            return _qteResultType;
        }

        /// <summary>
        /// QTEの失敗判定
        /// </summary>
        /// <returns></returns>
        public async UniTask QTEFailureJudgment(CancellationTokenSource startCts, CancellationToken endToken)
        {
            //失敗までの時間を計測
            await UniTask.WaitForSeconds(_playerParams.QteTimeLimit, true, PlayerLoopTiming.Update, endToken);
            ProvidePlayerInformation.EndQte.OnNext(new QteResultData(QTEResultType.Failure, _enemyId));
            ProvidePlayerInformation.TimeScale = 1f;
            _qteType.Value = QTEState.QTENone;
            _playerEnvroment.RemoveState(PlayerStateType.QTE);
            _qteResultType = QTEResultType.Failure;
            startCts.Cancel();
        }
    }
}
