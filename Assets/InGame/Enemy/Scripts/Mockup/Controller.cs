using Cysharp.Threading.Tasks;
using Enemy.Control;
using Enemy.DebugUse;
using System.Threading;
using UnityEngine;

namespace Enemy.Mockup
{
    public class Controller : MonoBehaviour, IDamageable
    {
        private PlayerChase _chase;
        private LookAt _lookAt;
        private Sensor _sensor;
        private LifePoint _lifePoint;
        private IAttack _attack;
        private CharacterAnimation _characterAnimation;

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
        // 待機位置から接近し、スロットに到達したか
        private bool _isArrivalAtSlot;
        // 一定間隔で攻撃中
        private bool _isFireTriggerPulling;
        // 死亡したフラグ
        private bool _isDeath;

        private void Awake()
        {
            _transform = transform;

            _chase = GetComponent<PlayerChase>();
            _lookAt = GetComponent<LookAt>();
            _sensor = GetComponent<Sensor>();
            _lifePoint = GetComponent<LifePoint>();
            _attack = GetComponent<IAttack>();
            _characterAnimation = GetComponent<CharacterAnimation>();
        }

        private void OnEnable()
        {
            _chase.OnSlotStay += ArrivalAtSlot;
            _sensor.OnCapture += PullFireTrigger;
            _sensor.OnUncapture += ReleaseFireTrigger;
            _lifePoint.OnDeath += DeathFlag;
        }

        private void OnDisable()
        {
            _chase.OnSlotStay -= ArrivalAtSlot;
            _sensor.OnCapture -= PullFireTrigger;
            _sensor.OnUncapture -= ReleaseFireTrigger;
            _lifePoint.OnDeath -= DeathFlag;
        }

        private void Start()
        {
            UpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid UpdateAsync(CancellationToken token)
        {
            Chase(false);

            await EntryAsync(token);

            Chase(true);
            ChaseSpeed(_approachSpeed);

            await IsArrivalAtSlotAsync(token);

            ChaseSpeed(_chaseSpeed);

            FireAsync(token).Forget();

            await IsDeathAsync(token);

            DeathFlag();

            await DeathEffectAsync(token);

            // とりあえず死亡時に破棄しておく
            Destroy(gameObject);
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
            if (_isDeath) return;

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
                    
                    if (_attack != null) _attack.Attack();

                    if (_characterAnimation != null)
                    {
                        // アニメーション再生を待たない
                        _characterAnimation.PlayAsync(AnimationKey.Attack, token).Forget();
                    }
                }

                await UniTask.Yield(token);
            }
        }

        // ダメージを受ける
        public void Damage(int value, string weapon)
        {
            // 勘違いで正の値を引数に渡すと回復するのを防ぐ
            int dmg = -Mathf.Abs(value);

            if (_lifePoint != null) _lifePoint.Change(dmg);
        }

        // 死亡したフラグを立てる
        private void DeathFlag()
        {
            _isDeath = true;
        }

        // 死亡したフラグが立つまで待つ
        private UniTask IsDeathAsync(CancellationToken token)
        {
            return UniTask.WaitUntil(() => _isDeath, cancellationToken: token);
        }

        // 死亡演出を待つ
        private async UniTask DeathEffectAsync(CancellationToken token)
        {
            await _characterAnimation.PlayAsync(AnimationKey.Broken, token);
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