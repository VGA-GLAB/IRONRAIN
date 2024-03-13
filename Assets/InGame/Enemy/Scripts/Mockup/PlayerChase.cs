using Enemy.Control;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Mockup
{
    /// <summary>
    /// プレイヤーを追跡する移動を行う。
    /// プレイヤーがY軸以外で回転すると破綻する可能性がある。
    /// </summary>
    public class PlayerChase : MonoBehaviour
    {
        /// <summary>
        /// スロットと同じ位置にいる間呼び出される。
        /// </summary>
        public event UnityAction OnSlotStay;

        [SerializeField] private SurroundingPool _pool;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _forward;
        [Header("接近する距離の設定")]
        [Tooltip("キャラクターのエリアの半径")]
        [Min(1.0f)]
        [SerializeField] private float _areaRadius = 1.0f;
        [Tooltip("プレイヤーのエリアの半径")]
        [SerializeField] private float _playerAreaRadius = 1.0f;
        [Header("接近速度の設定")]
        [Tooltip("ホーミングしてプレイヤーを追跡する移動の重み")]
        [SerializeField] private float _homingWeight = 1.0f;
        [Tooltip("上下移動の重み")]
        [SerializeField] private float _verticalWeight = 1.0f;
        [Tooltip("移動速度の制限")]
        [SerializeField] private float _velocityLimit = 10.0f;

        private Transform _transform;
        private Slot _slot;
        private CircleArea _area;
        private CircleArea _playerArea;

        // 現在位置と移動先を補間する値
        private float _homingPower;
        private float _verticalPower;

        private void Awake()
        {
            _transform = transform;
            _area = new CircleArea(_transform.position, _areaRadius);
        }

        private void Start()
        {
            if (_player != null)
            {
                _playerArea = new CircleArea(_player.position, _playerAreaRadius);
            }

            if (_pool != null && !_pool.TryRent(out _slot))
            {
                Debug.LogError($"敵を配置するスロットの確保に失敗: {name}");
            }
        }

        private void OnEnable()
        {
            // 無効化中に移動している可能性があるため。
            // プレイヤーのエリア内に移動していた場合は境界線上にワープしてしまう。
            _area.Point = _transform.position;
        }

        private void OnDestroy()
        {
            if (_pool != null) _pool.Return(_slot);
        }

        private void Update()
        {
            if (_player == null || _slot == null) return;

            HorizontalMove();
            VerticalMove();
            ExecuteEvent();
        }

        /// <summary>
        /// 外部からホーミングの強さを変更する。
        /// </summary>
        public void SetHomingWeight(float weight)
        {
            _homingWeight = weight;
        }

        // XZ平面上の移動
        private void HorizontalMove()
        {
            // プレイヤーの位置にエリアを配置
            _playerArea.Point = _player.position;

            // 速度の計算
            Vector3 velocity = Vector3.zero;
            velocity += HomingAcceleration(_homingPower);

            // 速度制限を付け、移動量を計算
            Vector3 delta = Vector3.ClampMagnitude(velocity, _velocityLimit) * Time.deltaTime;

            // 移動量がスロットまでの距離より大きい場合は、エリアをスロットと同じ位置に配置する。
            if (delta.sqrMagnitude >= _slot.SqrMagnitude(_area))
            {
                _area.Point = _slot.Point;
                // スロットの位置から離れた際にホーミングするようにリセットする。
                _homingPower = 0;
            }
            else
            {
                _area.Point += delta;
                // 必ずスロット位置に到達するようにホーミングの強さを徐々に増す。
                _homingPower += Time.deltaTime;
            }

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_area.Collision(_playerArea)) _area.Point = _area.TouchPoint(_playerArea);

            // エリアの中心に自身を配置する
            _transform.position = _area.MatchPoint(_transform.position);
        }

        // Y軸上の移動
        private void VerticalMove()
        {
            // 速度の計算
            Vector3 velocity = Vector3.zero;
            velocity += VerticalAcceleration(_verticalPower);
            
            // 移動量を計算
            Vector3 delta = velocity * Time.deltaTime;

            // 移動量がプレイヤーまでの距離より大きい場合は、プレイヤーと同じ高さに配置する。
            float dist = Mathf.Abs(_player.position.y - _transform.position.y);
            if (delta.sqrMagnitude >= dist * dist)
            {
                // 高さをプレイヤーに合わせる。
                Vector3 p = _transform.position;
                p.y = _player.position.y;
                _transform.position = p;
                // 高さが同じ間は移動しないので補間の値を0にしておく。
                _verticalPower = 0;
            }
            else
            {
                _transform.position += delta;
                // 補間の値を増加させ、移動速度を徐々に上げていく。
                _verticalPower += Time.deltaTime;
            }
        }

        // 状態に応じたイベントを呼び出す。
        private void ExecuteEvent()
        {
            // スロットと同じ位置にいる場合
            if (_transform.position == _slot.Point) OnSlotStay?.Invoke();
        }

        // スロットに向けたホーミングの加速度
        // 引数のpowerの値が高いほど強く曲がる
        private Vector3 HomingAcceleration(float power)
        {
            if (_slot == null || _forward == null) return Vector3.zero;

            // Lerpで角度を計算しているので01にクランプ
            power = Mathf.Clamp01(power);

            return Acceleration.Homing(_area.Point, _slot.Point, _forward.forward, power, _homingWeight);
        }

        // プレイヤーの高さに合わせた上下移動の加速度
        // 引数のpowerの値が高いほど慣性を無視した動きになる
        private Vector3 VerticalAcceleration(float power)
        {
            if (_player == null) return Vector3.zero;

            // Lerpで角度を計算しているので01にクランプ
            power = Mathf.Clamp01(power);

            return Acceleration.Vertical(_transform.position, _player.position, power, _verticalWeight);
        }

        private void OnDrawGizmos()
        {
            _slot?.DrawOnGizmos();
            _playerArea?.DrawOnGizmos();
            _area?.DrawOnGizmos();
        }
    }
}
