﻿using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using VContainer;

namespace Enemy.Control.Boss
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

        // ボス本体を登録するメッセージ
        struct Message
        {
            public BossController Boss;
            public List<FunnelController> Funnels;
        }

        [SerializeField] private Transform _model;
        [Header("エフェクトの設定")]
        [SerializeField] private Effect _destroyedEffect;
        [SerializeField] private Effect _trailEffect;

        private Transform _transform;
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
            _player = FindPlayer();
            _params = GetComponent<FunnelParams>();

            // メッセージの受信でボス本体を自身に登録。
            // 第二引数のリストに自身を追加して相互に参照させる。
            MessageBroker.Default.Receive<Message>().Subscribe(msg =>
            {
                _boss = msg.Boss;
                msg.Funnels.Add(this);
            }).AddTo(this);

            // ファンネル毎に射撃タイミングをずらすために経過時間の初期値を変える。
            _elapsed = Random.value;
        }

        private void Start()
        {
            _currentHp = _params.MaxHp;

            // 初期状態では画面に非表示。
            Hide();
        }

        private void Update()
        {
            // 展開中以外は画面に表示されていないので処理しない。
            if (_state != State.Expanded || _boss == null) return;

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

            // 一定時間毎に攻撃する。攻撃間隔は適当。
            _elapsed += _boss.BlackBoard.PausableDeltaTime;
            if (_elapsed > _params.FireRate)
            {
                _elapsed = 0;

                // ボスの前向きと同じ方向に飛ばす。
                BulletPool.Fire(
                    _boss.BlackBoard,
                    BulletKey.MachineGun,
                    _transform.position,
                    _boss.BlackBoard.Forward
                    );

                // 攻撃音
                AudioWrapper.PlaySE("SE_Funnel_Fire");
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

            if (_boss == null) return;

            // 初期化
            _currentHp = _params.MaxHp;
            _transform.position = _boss.transform.position;

            // ボス本体の円状の周囲に展開する。値は適当にベタ書き。
            float sin = Mathf.Sin(2 * Mathf.PI * Random.value);
            float cos = Mathf.Cos(2 * Mathf.PI * Random.value);
            float dist = Random.Range(2.0f, 3.0f);
            float h = Random.Range(1.5f, 2.5f);
            _expandOffset = new Vector3(cos * dist, h, sin * dist);

            // レーダーに表示する。
            if (TryGetComponent(out AgentScript a)) a.EnemyGenerate();
            // トレイルの演出再生。
            if (_trailEffect != null) _trailEffect.Play(_boss.BlackBoard);
            // ファンネルが飛んでいる音(ループしなくて良い？)
            AudioWrapper.PlaySE("SE_Funnel_Fly");
            // 撃破したものを使いまわすので、撃破時の演出を止める。
            if (_destroyedEffect != null) _destroyedEffect.Stop();

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
        public void Damage(int value, string _)
        {
            // 体力を更新し、0以下になった場合は撃破。
            _currentHp -= value;
            if (_currentHp > 0) return;

            // レーダーから消す。
            if (TryGetComponent(out AgentScript a)) a.EnemyDestory();
            // トレイルの演出を停止。
            if (_trailEffect != null) _trailEffect.Stop();
            // 撃破された際の演出
            if (_destroyedEffect != null)
            {
                // 二回目のファンネル展開がすぐだとおかしくなるかも。
                _destroyedEffect.PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }

            Hide();

            // 状態を撃破に変更
            _state = State.Defeated;
        }

        // 画面に表示させる。
        private void View()
        {
            _model.localScale = Vector3.one;
        }

        // 画面から隠す。
        private void Hide()
        {
            _model.localScale = Vector3.zero;
        }
    }
}