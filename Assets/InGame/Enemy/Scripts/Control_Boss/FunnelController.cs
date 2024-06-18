using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace Enemy.Control.Boss
{
    public class FunnelController : MonoBehaviour, IDamageable
    {
        enum State
        {
            Unexpanded, // 未展開(デフォルト)
            Expanded,   // 展開中
            Defeated,   // 撃破された
        }

        // ボス本体を登録するメッセージ
        struct Message
        {
            public BossController Boss;
            public List<FunnelController> Funnels;
        }

        [Header("撃破された際の演出(任意)")]
        [SerializeField] private Effect _defeatedEffect;

        private Transform _transform;
        private BossController _boss;
        // ボス本体を基準として展開するので、この値にボス本体の位置を足す。
        private Vector3 _expandOffset;
        // 速度
        private Vector3 _velocity;
        // 現在の状態
        private State _state = State.Unexpanded;

        private void Awake()
        {
            _transform = transform;

            // メッセージの受信でボス本体を自身に登録。
            // 第二引数のリストに自身を追加して相互に参照させる。
            MessageBroker.Default.Receive<Message>().Subscribe(msg =>
            {
                _boss = msg.Boss;
                msg.Funnels.Add(this);
            }).AddTo(this);
        }

        private void Start()
        {
            // 初期状態では画面に非表示。
            _transform.localScale = Vector3.zero;
        }

        private void Update()
        {
            // 展開中の場合はボス本体に追従する。
            if (_state == State.Expanded && _boss != null)
            {
                // 展開後の位置
                Vector3 expandedPoint = _boss.transform.position + _expandOffset;

                // 周囲をふわふわするように計算。値は適当にベタ書き。
                if ((expandedPoint - _transform.position).sqrMagnitude > 0.2f)
                {
                    _velocity += (expandedPoint - _transform.position).normalized;
                    _velocity = Vector3.ClampMagnitude(_velocity, 33.0f);
                }

                // 速度で移動。
                _transform.position += _velocity * _boss.BlackBoard.PausableDeltaTime;
            }
        }

        /// <summary>
        /// ファンネルを展開。
        /// </summary>
        public void Expand()
        {
            // 画面に表示。
            _transform.localScale = Vector3.one;

            // ボスに位置を合わせる。
            _transform.position = _boss.transform.position;

            if (_boss == null) return;

            // ボス本体の円状の周囲に展開する。値は適当にベタ書き。
            float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
            float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
            float dist = Random.Range(2.0f, 3.0f);
            float h = Random.Range(1.5f, 2.5f);
            _expandOffset = new Vector3(cos * dist, h, sin * dist);

            // 展開中に状態を変更
            _state = State.Expanded;
        }

        /// <summary>
        /// 全ファンネルにボス本体を登録する。
        /// 第二引数のリストに自身を追加することで相互に参照させる。
        /// </summary>
        public static void RegisterOwner(BossController boss, List<FunnelController> funnels)
        {
            MessageBroker.Default.Publish(new Message { Boss = boss, Funnels = funnels });
        }


        /// <summary>
        /// 攻撃を受けた。
        /// </summary>
        public void Damage(int value, string weapon)
        {
            // 仮なので体力はなし。
            _transform.localScale = Vector3.zero;
            _state = State.Defeated;

            // 撃破された際の演出
            if (_defeatedEffect != null) _defeatedEffect.Play(_boss.BlackBoard);
        }
    }
}
