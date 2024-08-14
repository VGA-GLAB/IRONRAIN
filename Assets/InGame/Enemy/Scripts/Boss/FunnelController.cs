using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Extensions;

namespace Enemy.Boss
{
    [RequireComponent(typeof(FunnelParams))]
    public class FunnelController : Character, IDamageable
    {
        // 状態
        enum State { Unexpanded, Expanded, Defeated }

        // 展開モード
        enum Mode { Default, Right, Left }

        [SerializeField] private Transform _model;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Collider _hitBox;
        [Header("エフェクトの設定")]
        [SerializeField] private Effect _destroyedEffect;
        [SerializeField] private Effect _trailEffect;
        [Header("展開モード")]
        [SerializeField] private Mode _mode;

        private Transform _transform;
        private Transform _offset;
        private Transform _rotate;
        private Transform _player;
        private BossController _boss;
        private FunnelParams _params;

        // ボス本体を基準として展開するので、この値にボス本体の位置を足す。
        private Vector3 _expandOffset;
        // 速度
        private Vector3 _velocity;
        // 現在の状態
        private State _state = State.Unexpanded;
        // 攻撃までの時間
        private float _elapsed;
        // 現在の体力
        private int _currentHp;

        private void Awake()
        {
            _transform = transform;
            _offset = FindOffset();
            _rotate = FindRotate();
            _player = FindPlayer();
            _params = GetComponent<FunnelParams>();
        }

        private void Start()
        {
            ResetStatus();
            Hide();
        }

        private void Update()
        {
            // 展開中以外は画面に表示されていないので処理しない。
            if (_state != State.Expanded) return;

            Move();
            LookAtPlayer();

            if (FireTrigger())
            {
                Fire();
                AudioWrapper.PlaySE("SE_Funnel_Fire");
            }
        }

        /// <summary>
        /// タグで全ファンネルを検索、ボスを登録し、引数のリストに全ファンネルを詰めて返す。
        /// </summary>
        public static void RegisterOwner(BossController boss, List<FunnelController> funnels)
        {
            foreach (GameObject g in GameObject.FindGameObjectsWithTag(Const.EnemyTag))
            {
                if (g.TryGetComponent(out FunnelController f))
                {
                    f._boss = boss;
                    funnels.Add(f);
                }
            }
        }

        /// <summary>
        /// ファンネルを展開。
        /// </summary>
        public void Expand()
        {
            // ドローンを倒しきらずに2回目の展開をした場合は弾く。
            if (_state == State.Expanded) return;

            View();
            ResetStatus();
            SetExpandOffset();
            RadarMap(true);
            TrailEffect(true);
            DestroyedEffect(false);

            // ファンネルが飛んでいる音(ループしなくて良い？)
            AudioWrapper.PlaySE("SE_Funnel_Fly");

            // 展開中に状態を変更
            _state = State.Expanded;
        }

        // 展開する度にリセットされる。
        private void ResetStatus()
        {
            _currentHp = _params.MaxHp;
            if (_boss != null) _transform.position = _boss.transform.position;
            // ファンネル毎に射撃タイミングをずらすために経過時間の初期値を変える。
            _elapsed = Random.value;
        }

        // ボス本体の左右もしくは周囲に展開する。
        private void SetExpandOffset()
        {
            if (_mode == Mode.Default)
            {
                const float MaxHeight = 8.0f;
                const float MinHeight = 6.0f;
                const float MaxSide = 6.0f;
                const float MinSide = 4.0f;

                float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
                float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
                float dist = Random.Range(MinSide, MaxSide);
                float h = Random.Range(MinHeight, MaxHeight);
                int lr = Random.value <= 0.5f ? 1 : -1;
                _expandOffset = new Vector3(cos * dist * lr, h, sin * dist * lr);
            }
            else
            {
                const float Height = 10.0f;
                const float Side = 5.0f;

                if (_mode == Mode.Right) _expandOffset = new Vector3(Side, Height, 0);
                else if (_mode == Mode.Left) _expandOffset = new Vector3(-Side, Height, 0);
            }
        }

        // ボスの周囲を浮遊するような動き。
        private void Move()
        {
            Vector3 expandedPoint = _boss.transform.position + _expandOffset;
            Vector3 dir = expandedPoint - _transform.position;
            Vector3 velo = dir.normalized * _boss.BlackBoard.PausableDeltaTime * _params.MoveSpeed;
            if (velo.sqrMagnitude <= dir.sqrMagnitude)
            {
                _transform.position += velo;
            }
            else
            {
                _transform.position = expandedPoint;
            }
        }

        // プレイヤーを向く。
        private void LookAtPlayer()
        {
            _rotate.forward = _player.position - _transform.position;
        }

        // 一定時間で攻撃をトリガー。
        private bool FireTrigger()
        {
            _elapsed += _boss.BlackBoard.PausableDeltaTime;

            if (_elapsed > _params.FireRate) { _elapsed = 0; return true; }
            else { return false; }
        }

        // ボスの前向きと同じ方向に弾を飛ばす。
        private void Fire()
        {
            IOwnerTime owner = _boss.BlackBoard;
            Vector3 muzzle = _muzzle.position;

            float rx = Random.value * _params.Accuracy;
            float ry = Random.value * _params.Accuracy;
            float rz = Random.value * _params.Accuracy;
            Vector3 forward = _boss.BlackBoard.Forward + new Vector3(rx, ry, rz);

            BulletPool.Fire(owner, BulletKey.Funnel, muzzle, forward);
        }

        /// <summary>
        /// 攻撃を受けた。
        /// </summary>
        public void Damage(int value, string _)
        {
            // 体力を更新し、0以下になった場合は撃破。
            _currentHp -= value;
            if (_currentHp > 0) return;

            RadarMap(false);
            TrailEffect(false);
            DestroyedEffect(true);
            Hide();

            // 状態を撃破に変更
            _state = State.Defeated;
        }

        // 画面に表示させる。
        private void View()
        {
            _model.localScale = Vector3.one;
            _hitBox.enabled = true;
        }

        // 画面から隠す。
        private void Hide()
        {
            _model.localScale = Vector3.zero;
            _hitBox.enabled = false;
        }

        // レーダーマップに表示/非表示
        private void RadarMap(bool value)
        {
            if (TryGetComponent(out AgentScript a))
            {
                if (value) a.EnemyGenerate();
                else a.EnemyDestory();
            }
        }

        // トレイルのエフェクトを表示/非表示
        private void TrailEffect(bool value)
        {
            if (_trailEffect == null) return;
            
            if (value) _trailEffect.Play(_boss.BlackBoard);
            else _trailEffect.Stop();
        }

        // 撃破のエフェクトを表示/非表示
        private void DestroyedEffect(bool value)
        {
            if(_destroyedEffect == null) return;

            // 二回目のファンネル展開がすぐだとおかしくなるかも。
            if (value) _destroyedEffect.PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
            else _destroyedEffect.Stop();
        }

        /// <summary>
        /// レーザーサイトの表示/非表示
        /// </summary>
        public void LaserSight(bool value)
        {
            if (!this.TryGetComponentInChildren(out LineRenderer line)) return;

            const float Length = 10.0f;

            line.enabled = value;
            line.SetPosition(0, _muzzle.position);
            line.SetPosition(1, _muzzle.position + _muzzle.forward * Length);
        }
    }
}