using System.Collections.Generic;
using UnityEngine;
using Enemy.Extensions;
using Enemy.Boss;

namespace Enemy.Funnel
{
    [RequireComponent(typeof(FunnelParams))]
    public class FunnelController : Character, IDamageable
    {
        [SerializeField] private Renderer[] _renderers;
        [SerializeField] private Transform _muzzle;
        [SerializeField] private FunnelEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        // Perception層
        private Perception _perception;
        private HitPoint _hitPoint;
        // Action層
        private BodyController _bodyController;

        // ボスを登録するまで動かさないフラグ。
        private bool _isRegistered;

        [ContextMenu("3DモデルのRendererへの参照を取得")]
        private void GetRendererAll()
        {
            List<Renderer> r = new List<Renderer>();
            foreach (Renderer sm in GetComponentsInChildren<SkinnedMeshRenderer>()) r.Add(sm);
            foreach (Renderer m in GetComponentsInChildren<MeshRenderer>()) r.Add(m);
            _renderers = r.ToArray();

            foreach (var v in _renderers) Debug.Log($"{name}: {v}");
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
            _hitPoint = new HitPoint(requiredRef);
            _bodyController = new BodyController(requiredRef);

            _isRegistered = true;
        }

        private void Update()
        {
            if (!_isRegistered) return;

            _perception.Update();
            _hitPoint.Update();
            _bodyController.Update();
        }

        /// <summary>
        /// ファンネルを展開。
        /// </summary>
        public void Expand()
        {
            _perception.ExpandOrder();
        }

        /// <summary>
        /// ファンネルに攻撃を命令。
        /// </summary>
        public void Fire()
        {
            _perception.FireOrder();
        }

        /// <summary>
        /// 攻撃を受けた。
        /// </summary>
        public void Damage(int value, string _)
        {
            _hitPoint.Damage(value, _);
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