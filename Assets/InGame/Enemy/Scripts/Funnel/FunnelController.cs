using System.Collections.Generic;
using UnityEngine;
using Enemy.Extensions;
using Enemy.Boss;

namespace Enemy.Funnel
{
    [RequireComponent(typeof(FunnelParams))]
    public class FunnelController : Character, IDamageable
    {
        [SerializeField] private Transform _muzzle;
        [SerializeField] private FunnelEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        private Perception _perception;
        private StateMachine _stateMachine;

        // ボスを登録するまで動かさないフラグ。
        private bool _isRegistered;

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

        // Awakeの代わりに、ここで初期化する。
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
                boss: boss,
                muzzle: _muzzle
                );

            _perception = new Perception(requiredRef);
            _stateMachine = new StateMachine(requiredRef);

            _perception.InitializeOnStart();

            _isRegistered = true;
        }

        private void Update()
        {
            if (!_isRegistered) return;

            _perception.Update();
            _stateMachine.Update();
        }

        /// <summary>
        /// ファンネルを展開。
        /// </summary>
        public void Expand() => _perception.Order(EnemyOrder.Type.FunnelExpand);

        /// <summary>
        /// 攻撃を受けた。
        /// </summary>
        public void Damage(int value, string _) => _perception.Damage(value, _);

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