using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Enemy.DebugUse;

namespace Enemy.Control
{
    public class Controller : MonoBehaviour
    {
        private enum State
        {
            Entry,
            Approach,
            Chase,
        }

        private PlayerChase _chase;
        private LookAt _lookAt;
        private Sensor _sensor;
        private LifePoint _lifePoint;
        private IAttack _attack;

        [SerializeField] private Transform _forward;
        [Header("生成位置から待機位置への移動設定")]
        [SerializeField] private float _entrySpeed = 5.0f;
        [SerializeField] private Transform[] _entryPath;
        [Header("待機位置からスロットへの移動設定")]
        [SerializeField] private float _approachSpeed = 10.0f;
        [Header("スロット到着後の移動設定")]
        [SerializeField] private float _chaseSpeed = 5.0f;
        [Header("攻撃設定")]
        [SerializeField] private float _fireRate = 0.5f;

        private Transform _transform;
        private State _currentState;
        // 待機位置から接近し、スロットに到達したか
        private bool _isArrivalAtSlot;
        // 一定間隔で攻撃中
        private bool _isFireTriggerPulling;

        private void Awake()
        {
            _transform = transform;

            _chase = GetComponent<PlayerChase>();
            _lookAt = GetComponent<LookAt>();
            _sensor = GetComponent<Sensor>();
            _lifePoint = GetComponent<LifePoint>();
            _attack = GetComponent<IAttack>();
        }

        private void OnEnable()
        {
            _chase.OnSlotStay += ArrivalAtSlot;
            _sensor.OnDamaged += Damage;
            _sensor.OnCapture += PullFireTrigger;
            _sensor.OnUncapture += ReleaseFireTrigger;
        }

        private void OnDisable()
        {
            _chase.OnSlotStay -= ArrivalAtSlot;
            _sensor.OnDamaged -= Damage;
            _sensor.OnCapture -= PullFireTrigger;
            _sensor.OnUncapture -= ReleaseFireTrigger;
        }

        private void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            //if (Input.GetKeyDown(KeyCode.Space)) _attack.Attack();

            Chase(false);

            await EntryAsync(token);

            Chase(true);
            ChaseSpeed(_approachSpeed);

            await IsArrivalAtSlotAsync(token);

            ChaseSpeed(_chaseSpeed);

            FireAsync(token).Forget();

            // 一定間隔で攻撃つくるところから

            // 登場シーケンス、接近シーケンス、追跡シーケンス、死亡シーケンス を左から右に処理していく
            // 終了条件があるステートマシン。

            // イベント駆動型、任意のイベンﾄに遷移をトリガーして外部からイベント発行し、ステート内では遷移条件を調べない

            // 一定間隔で攻撃？
            // 撃破されたら死ぬ

            //switch (_currentState)
            //{
            //    case State.Entry:
            //    break;
            //    case State.Approach:
            //    break;
            //    case State.Chase:
            //    break;
            //}
        }

        // プレイヤーを追いかける。
        private void Chase(bool active)
        {
            _chase.enabled = active;
            _lookAt.enabled = active;
        }

        // プレイヤーを追いかける速度を設定。
        private void ChaseSpeed(float speed)
        {
            _chase.SetHomingWeight(speed);
        }

        // スロットに到着したフラグを立てる。
        private void ArrivalAtSlot()
        {
            _isArrivalAtSlot = true;
        }

        // スロットに到着するまで待つ。
        private UniTask IsArrivalAtSlotAsync(CancellationToken token)
        {
            return UniTask.WaitUntil(() => _isArrivalAtSlot, cancellationToken: token);
        }

        // ダメージを受ける
        private void Damage(Collision collision)
        {
            _lifePoint.Change(-1);
        }

        // 経路の始点に配置し、経路の末端まで補間で移動。
        private async UniTask EntryAsync(CancellationToken token)
        {
            if (_entryPath == null || _entryPath.Length < 2) return;

            for (int i = 1; i < _entryPath.Length; i++)
            {
                Vector3 s = _entryPath[i - 1].position;
                Vector3 g = _entryPath[i].position;

                if (_forward != null) _forward.forward = g - s;

                for (float t = 0; t <= 1.0f; t += Time.deltaTime * _entrySpeed)
                {
                    _transform.position = Vector3.Lerp(s, g, t);
                    await UniTask.Yield(token);
                }

                _transform.position = g;
            }
        }

        // 対象がプレイヤーの場合は、攻撃フラグを立てることで一定間隔で攻撃する
        private void PullFireTrigger(Collider collider)
        {
            if (collider.CompareTag(Const.PlayerTag))
            {
                _isFireTriggerPulling = true;
            }
        }

        // 攻撃フラグを折ることで一定間隔の攻撃を中断
        private void ReleaseFireTrigger()
        {
            _isFireTriggerPulling = false;
        }

        // 一定間隔で攻撃
        private async UniTask FireAsync(CancellationToken token)
        {
            float elapsed = 0;
            while (!token.IsCancellationRequested)
            {
                if (_isFireTriggerPulling)
                {
                    elapsed += Time.deltaTime;
                }

                if (elapsed > _fireRate)
                {
                    elapsed = 0;
                    _attack.Attack();
                }

                await UniTask.Yield(token);
            }
        }

        private void OnDrawGizmos()
        {
            if (_entryPath == null || _entryPath.Length < 2) return;

            for (int i = 1; i < _entryPath.Length; i++)
            {
                Vector3 s = _entryPath[i - 1].position;
                Vector3 g = _entryPath[i].position;
                // 経路の頂点同士を結ぶ
                GizmosUtils.Sphere(s, 0.5f, Color.cyan);
                GizmosUtils.Sphere(g, 0.5f, Color.cyan);
                GizmosUtils.Arrow(s, g, Color.cyan);
            }

            // シーン上の位置から生成位置への線
            GizmosUtils.Line(transform.position, _entryPath[0].position, new Color(0, 1, 1, 0.1f));
        }
    }
}

// 1.生成位置から中継地点まで飛んでくる(アニメーション等、これは無くても良い)
// 2.中継地点からスロットまで飛んでくる(PlayerChaseのパラメータ変更)
// 3.スロットに追従する(PlayerChaseのパラメータ変更)

// 攻撃と防御について
//  攻撃は武器毎にダメージも範囲も間隔も異なる
//  防御は特定条件下でダメージを受けない、例えば盾を持っているなど
//  アニメーションの開始から攻撃まではディレイがある
//  必ずプレイヤーめがけて攻撃するわけではない(向いている方向など)

// 近接攻撃はデバッグした。後はインターフェース待ち、誰か作るはず。
// 遠距離攻撃は弾のマズルと、飛んでいく弾、プールへの登録してデバッグする。
// 弾には当たり判定のあるものとないものがある。