using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Enemy.Extensions;
using Enemy.Boss;

namespace Enemy.Funnel
{
    [RequireComponent(typeof(FunnelParams))]
    public class FunnelController : Character, IDamageable
    {
        // 状態
        enum State { Unexpanded, Expanded, Defeated }

        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private FunnelEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        //[SerializeField] private Transform _model;
        //[Header("エフェクトの設定")]
        //[SerializeField] private Effect _destroyedEffect;
        //[SerializeField] private Effect _trailEffect;
        //[Header("展開モード")]
        //[SerializeField] private Mode _mode;

        //private Transform _transform;
        //private Transform _offset;
        //private Transform _rotate;
        //private Transform _player;
        //private BossController _boss;
        //private Transform _bossRotate;
        //private FunnelParams _params;

        // Perception層
        private Perception _perception;
        private HitPoint _hitPoint;
        private OverrideOrder _overrideOrder;
        // Action層
        private BodyController _bodyController;

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


            //_transform = transform;
            //_offset = FindOffset();
            //_rotate = FindRotate();
            //_player = FindPlayer();
            //_params = GetComponent<FunnelParams>();
        }

        private void Start()
        {
            //ResetStatus();
            //Hide();
        }

        private void Update()
        {
            // 展開中以外は画面に表示されていないので処理しない。
            if (_state != State.Expanded) return;

            Move();

            if (_mode == Mode.Right || _mode == Mode.Left) LookAtPlayer();
            else LookBossForward();

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
                if (!g.TryGetComponent(out FunnelController f)) continue;

                f.Register(boss);
                funnels.Add(f);
            }
        }

        private void Register(BossController boss)
        {
            // 必要な参照をまとめる。
            RequiredRef requiredRef = new RequiredRef(
                transform: transform,
                player: FindPlayer(),
                offset: FindOffset(),
                rotate: FindRotate(),
                funnelParams: GetComponent<FunnelParams>(),
                blackBoard: new BlackBoard(gameObject.name),
                animator: GetComponentInChildren<Animator>(),
                renderers: _renderers,
                effects: _effects,
                hitBoxes: _hitBoxes,
                boss: boss
                );
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

        // ボスの前方向を基準にオフセットされた位置に移動。
        //private void Move()
        //{
        //    Vector3 dx = _bossRotate.right * _expandOffset.x;
        //    Vector3 dy = _bossRotate.up * _expandOffset.y;
        //    Vector3 dz = _bossRotate.forward * _expandOffset.z;
        //    Vector3 p = _boss.transform.position + dx + dy + dz;
        //    Vector3 dir = p - _transform.position;
        //    Vector3 velo = dir.normalized * _boss.BlackBoard.PausableDeltaTime * _params.MoveSpeed;
        //    if (velo.sqrMagnitude <= dir.sqrMagnitude) _transform.position += velo;
        //    else _transform.position = p;
        //}

        // プレイヤーを向く。
        //private void LookAtPlayer()
        //{
        //    _rotate.forward = _player.position - _transform.position;
        //}

        // ボスの正面を向く。
        //private void LookBossForward()
        //{
        //    _rotate.forward = _bossRotate.forward;
        //}

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
            Vector3 forward = _bossRotate.forward + new Vector3(rx, ry, rz);

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
            _hitBoxes.enabled = true;
        }

        // 画面から隠す。
        private void Hide()
        {
            _model.localScale = Vector3.zero;
            _hitBoxes.enabled = false;
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