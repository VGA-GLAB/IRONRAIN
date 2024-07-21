using Enemy.Control;
using Enemy.Extensions;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// デバッグ用のプレイヤー制御。
    /// </summary>
    public class DebugPlayerController : MonoBehaviour, IDamageable
    {
        [Header("操作設定")]
        [SerializeField] private float _moveSpeed = 10.0f;
        [SerializeField] private float _rotationSpeed = 10.0f;
        [Header("攻撃設定")]
        [SerializeField] private float _forwardOffset;
        [SerializeField] private int _damage = 1;
        [SerializeField] private int _range = 10;
        [Header("カメラ制御を行う")]
        [SerializeField] private Transform _camera;

        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }

        private void Update()
        {
            // 前後左右の移動
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) move += _transform.forward;
            if (Input.GetKey(KeyCode.S)) move -= _transform.forward;
            if (Input.GetKey(KeyCode.A)) move -= _transform.right;
            if (Input.GetKey(KeyCode.D)) move += _transform.right;
            // 上下の移動
            if (Input.GetKey(KeyCode.Q)) move += Vector3.up;
            if (Input.GetKey(KeyCode.E)) move += Vector3.down;

            // 左右に回転
            float rot = 0;
            if (Input.GetKey(KeyCode.LeftArrow)) rot--;
            if (Input.GetKey(KeyCode.RightArrow)) rot++;

            // 高速化
            float mag = 1;
            if (Input.GetKey(KeyCode.LeftShift)) mag++;

            // 移動
            Vector3 deltaMove = move.normalized * mag * _moveSpeed * Time.deltaTime * ProvidePlayerInformation.TimeScale;
            _transform.position += deltaMove;

            // 回転
            float deltaRot = rot * mag * _rotationSpeed * Time.deltaTime * ProvidePlayerInformation.TimeScale;
            _transform.Rotate(new Vector3(0, deltaRot, 0));

            // 遠距離攻撃
            if (Input.GetKeyDown(KeyCode.Space)) Attack(Const.PlayerAssaultRifleWeaponName);
            // 近接攻撃
            if (Input.GetKeyDown(KeyCode.Return)) Attack(Const.PlayerMeleeWeaponName);
        }

        // レイキャストで攻撃
        private void Attack(string weapon)
        {
            (Vector3 origin, Vector3 end) p = AttackRange();
            if(Physics.Raycast(p.origin, _transform.forward, out RaycastHit hit, _range))
            {
                // 当たり判定が別のオブジェクトになっている。
                IDamageable damage = hit.collider.GetComponentInParent<IDamageable>();
                if (damage == null) return;

                damage.Damage(_damage, weapon);

                // 攻撃のヒットをギズモに描画
                Debug.DrawLine(p.origin, p.end, Color.red);
            }
        }

        // 攻撃範囲
        private (Vector3, Vector3) AttackRange()
        {
            Transform t = _transform != null ? _transform : transform;

            Vector3 p = t.position + t.forward * _forwardOffset;
            Vector3 q = p + t.forward * _range;
            return (p, q);
        }

        // カメラをプレイヤーの正面を捉えるように制御する
        private void LateUpdate()
        {
            if (_camera == null) return;
            
            _camera.position = _transform.position;
            _camera.forward = _transform.forward;
        }

        private void OnDrawGizmos()
        {
            DrawAttackRange();
        }

        // 攻撃範囲
        private void DrawAttackRange()
        {
            (Vector3, Vector3) p = AttackRange();
            GizmosUtils.Line(p.Item1, p.Item2, ColorExtensions.ThinRed);
        }

        public void Damage(int value, string weapon = "")
        {
            // ダメージ処理
        }
    }
}
