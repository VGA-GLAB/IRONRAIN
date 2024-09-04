using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

namespace IronRain.Player
{
    public class PlayerBossMoveModel : IPlayerStateModel
    {
        public List<Vector3> LaneList => _laneList;
        public IReactiveProperty<int> CurrentRane => _currentLane;
        public Transform PointP => _pointP;

        [SerializeField] LeverController _leftController;
        [SerializeField] LeverController _rightController;
        [SerializeField] Rigidbody _rb;
        [SerializeField] private Transform _pointP;
        [SerializeField] private GameObject _cubeObj;

        private float _totalThrusterMove;
        private CancellationTokenSource _thrusterCancell;
        private PlayerEnvroment _playerEnvroment;
        private PlayerSetting.PlayerParams _params;
        private Transform _transform;
        private float _startTheta;
        private Transform _centerPoint;
        private List<Vector3> _laneList = new();
        private ReactiveProperty<int> _currentLane = new();

        public void SetUp(PlayerEnvroment env, CancellationToken token)
        {
            _playerEnvroment = env;
            _params = env.PlayerSetting.PlayerParamsData;
            _leftController.SetUp(env.PlayerSetting);
            _rightController.SetUp(env.PlayerSetting);
            _transform = _playerEnvroment.PlayerTransform;
            _centerPoint = _pointP;
            //SumTheta();
            CreateLane();
            //LanePosIns();
        }

        public void Start()
        {
            _thrusterCancell = new CancellationTokenSource();
            _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
            _playerEnvroment.OnStateChange += ThrusterCancell;
        }
        public void FixedUpdate()
        {
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
                && !_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable))
            {
                Move();
            }
            else
            {
                _playerEnvroment.PlayerTransform.parent = null;
                _rb.velocity = Vector2.zero;
            }
        }

        public void LanePosIns() 
        {
            for (int i = 0; i < _laneList.Count; i++) 
            {
                GameObject.Instantiate(_cubeObj, _laneList[i] + _centerPoint.position, Quaternion.identity);
            }
        }

        public void Update()
        {
            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.QTE)
                && !_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Inoperable))
            {
                _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
            }
        }

        public void Dispose()
        {
            _currentLane.Dispose();
        }

        public void ResetPos()
        {

        }

        private void Move()
        {
            //左スラスター
            if (_rightController.ControllerDir.x == -1)
            {
                //スラスター中ではなかった場合
                if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
                {
                    _playerEnvroment.AddState(PlayerStateType.Thruster);
                    //var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum * -1);
                    var nextPoint = NextLane(-1);

                    UniTask.Create(async () =>
                    {
                        await _playerEnvroment.PlayerTransform
                        .DOLocalMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                        .OnComplete(() => _playerEnvroment.PlayerTransform.localPosition = nextPoint)
                        .OnKill(() => _playerEnvroment.PlayerTransform.localPosition = nextPoint)
                        .ToUniTask(cancellationToken: _thrusterCancell.Token);
                        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                        _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                    });
                }
            }
            //右スラスター
            else if (_rightController.ControllerDir.x == 1)
            {
                //スラスター中ではなかった場合
                if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.Thruster))
                {
                    _playerEnvroment.AddState(PlayerStateType.Thruster);
                    //var nextPoint = NextThrusterMovePoint(_params.ThrusterMoveNum);
                    var nextPoint = NextLane(1);
                    UniTask.Create(async () =>
                    {
                        await _playerEnvroment.PlayerTransform
                        .DOLocalMove(nextPoint, _params.ThrusterMoveTime * ProvidePlayerInformation.TimeScale)
                        .OnComplete(() => _playerEnvroment.PlayerTransform.localPosition = nextPoint)
                        .OnKill(() => _playerEnvroment.PlayerTransform.localPosition = nextPoint)
                        .ToUniTask(cancellationToken: _thrusterCancell.Token);
                        _playerEnvroment.PlayerTransform.LookAt(_centerPoint);
                        _playerEnvroment.RemoveState(PlayerStateType.Thruster);
                    });
                }
            }
            //２ギア
            else
            {
            }
        }

        public void  ThrusterCancell(PlayerStateType playerState) 
        {
            if (playerState.HasFlag(PlayerStateType.EnterBossQte)) 
            {
                _thrusterCancell.Cancel();
            } 
        }


        /// <summary>
        /// 次にスラスターで移動するポイント
        /// </summary>
        private Vector3 NextThrusterMovePoint(float moveDistance)
        {
            var playerPos = _playerEnvroment.PlayerTransform.localPosition;
            playerPos.y = 0;
            var centerPosition = Vector3.zero;
            centerPosition.y = 0;
            var r = Vector3.Distance(centerPosition, playerPos);
            var theta = (moveDistance / r);
            Debug.Log($"θ:{theta}r:{r}");

            _totalThrusterMove += theta;
            var cos = Mathf.Cos(_totalThrusterMove + _startTheta);
            var x = cos * r;
            var z = Mathf.Sin(_totalThrusterMove + _startTheta) * r;

            var position = new Vector3(x + centerPosition.x, _playerEnvroment.PlayerTransform.localPosition.y, z + centerPosition.z);
            //Debug.Log($"移動しましたX:{position.x}:Z{position.z}r:{r}");
            return position;
        }

        private Vector3 NextThrusterMovePoint(float moveDistance, Vector3 pos) 
        {
            pos.y = 0;
            var centerPosition = Vector3.zero;
            centerPosition.y = 0;
            var r = Vector3.Distance(centerPosition, pos);
            var theta = (moveDistance / r);

            _totalThrusterMove += moveDistance;
            Debug.Log($"θ:{theta}r:{r}total{_totalThrusterMove}start{_startTheta}");
            var cos = Mathf.Cos((_totalThrusterMove + 270) * Mathf.Deg2Rad);
            var x = cos * r;
            var z = Mathf.Sin((_totalThrusterMove + 270) * Mathf.Deg2Rad) * r;

            var position = new Vector3(x + centerPosition.x, _playerEnvroment.PlayerTransform.localPosition.y, z + centerPosition.z);
            //Debug.Log($"移動しましたX:{position.x}:Z{position.z}r:{r}");
            return position;
        }


        private void CreateLane()
        {
            int idx = Mathf.FloorToInt(360 / _params.ThrusterMoveNum);
            var nextPos = _playerEnvroment.PlayerTransform.localPosition;

            nextPos = NextThrusterMovePoint(0, nextPos);
            _laneList.Add(nextPos);
            for (int i = 1; i < idx; i++) 
            {
                nextPos = NextThrusterMovePoint(_params.ThrusterMoveNum, nextPos);
                _laneList.Add(nextPos);
            }
        }

        private Vector3 NextLane(int isRight) 
        {
            var nextLane = _currentLane.Value + isRight;
            if (nextLane < 0)
            {
                _currentLane.Value = _laneList.Count - 1;
            }
            else if (_laneList.Count - 1 < nextLane)
            {
                _currentLane.Value = 0;
            }
            else 
            {
                _currentLane.Value += isRight;
            }

            return _laneList[_currentLane.Value];
        }

        private void SumTheta()
        {
            var playerPos = _playerEnvroment.PlayerTransform.localPosition;
            playerPos.y = 0;
            var centerPosition = Vector3.zero;
            centerPosition.y = 0;

            var r = Vector3.Distance(centerPosition, playerPos);
            var aDir = (playerPos - centerPosition).normalized;
            var bDir = (new Vector3(centerPosition.x, 0, r) - centerPosition).normalized;
            Debug.Log($"aDir:{aDir}bDir:{bDir}");
            _startTheta = Vector3.Angle(aDir, bDir);
        }
    }
}
