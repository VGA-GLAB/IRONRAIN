using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    [RequireComponent(typeof(FunnelParams))]
    public class FunnelController : Character, IDamageable
    {
        enum State
        {
            Unexpanded, // 未展開(デフォルト)
            Expanded,   // 展開中
            Defeated,   // 撃破された
        }

        [SerializeField] private Transform _model;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private Collider _hitBox;
        [Header("エフェクトの設定")]
        [SerializeField] private Effect _destroyedEffect;
        [SerializeField] private Effect _trailEffect;
        [Header("弾のばらけ具合")]

        private Transform _transform;
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

        // ボス本体の円状の周囲に展開する。値は適当にベタ書き。
        private void SetExpandOffset()
        {
            float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
            float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
            float dist = Random.Range(2.0f, 3.0f);
            float h = Random.Range(1.5f, 2.5f);
            _expandOffset = new Vector3(cos * dist, h, sin * dist);
        }

        // ボスの周囲を浮遊するような動き。
        private void Move()
        {
            Vector3 expandedPoint = _boss.transform.position + _expandOffset;
            Vector3 dir = expandedPoint - _transform.position;

            if (dir.sqrMagnitude > 0.2f)
            {
                _velocity += dir.normalized;
                _velocity = Vector3.ClampMagnitude(_velocity, 33.0f);
            }

            // 速度で移動。
            _transform.position += _velocity * _boss.BlackBoard.PausableDeltaTime;
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
    }
}