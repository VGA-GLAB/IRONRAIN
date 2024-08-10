﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Boss
{
    // 一時的に画面から消して動作を止めたい場合: xxx
    // これ以上動かさず、削除する場合: xxx

    /// <summary>
    /// ボスを操作する。
    /// </summary>
    [RequireComponent(typeof(BossParams))]
    public class BossController : Character, IDamageable
    {
        [SerializeField] private Transform[] _models;
        [SerializeField] private BossEffects _effects;
        [SerializeField] private Collider[] _hitBoxes;

        private BossParams _params;
        private BlackBoard _blackBoard;
        private List<FunnelController> _funnels;
        // Perception層
        private Perception _perception;
        private FireRate _fireRate;
        private HitPoint _hitPoint;
        private OverrideOrder _overrideOrder;
        // Action層
        private BodyController _bodyController;

        // 非表示にする非同期処理を実行中フラグ。
        // 二重に処理を呼ばないために必要。
        private bool _isCleanupRunning;

        /// <summary>
        /// 敵の状態を参照する。実行中に変化する値はこっち。
        /// </summary>
        public IReadonlyBlackBoard BlackBoard => _blackBoard;

        private void Awake()
        {
            // 必要な参照をまとめる。
            RequiredRef requiredRef = new RequiredRef(
                transform: transform,
                player: FindPlayer(),
                offset: FindOffset(),
                rotate: FindRotate(),
                bossParams: GetComponent<BossParams>(),
                blackBoard: new BlackBoard(gameObject.name),
                animator: GetComponentInChildren<Animator>(),
                models: _models,
                effects: _effects,
                hitBoxes: _hitBoxes,
                meleeEquip: GetComponent<MeleeEquipment>(),
                rangeEquip: GetComponent<RangeEquipment>(),
                funnels: new List<FunnelController>(),
                pointP: FindAnyObjectByType<DebugPointP>()
                );

            _params = requiredRef.BossParams;
            _blackBoard = requiredRef.BlackBoard;
            _funnels = requiredRef.Funnels;

            _perception = new Perception(requiredRef);
            _fireRate = new FireRate(requiredRef);
            _hitPoint = new HitPoint(requiredRef);
            _overrideOrder = new OverrideOrder(requiredRef);
            _bodyController = new BodyController(requiredRef);
        }

        private void Start()
        {
            EnemyManager.Register(this);
            FunnelController.RegisterOwner(this, _funnels);

            _perception.Init();
        }

        private void Update()
        {
            _perception.Update();
            _fireRate.UpdateIfAttacked();
            _hitPoint.Update();
            _overrideOrder.Update();

            // オブジェクトに諸々を反映させているので結果をハンドリングする。
            // 完了が返ってきた場合は、続けて後始末処理を呼び出す。
            // 非表示前処理 -> LateUpdate -> 次フレームのUpdate -> 非表示 の順で呼ばれる。
            if (_bodyController.Update() == BodyController.Result.Complete && !_isCleanupRunning)
            {
                _isCleanupRunning = true;
                StartCoroutine(CleanupAsync());
            }
        }

        private void LateUpdate()
        {
            _overrideOrder.ClearOrderedTrigger();
        }

        // 後始末、Update内から呼び出す。
        private IEnumerator CleanupAsync()
        {
            _perception.Dispose();
            _bodyController.Dispose();

            // 次フレームのUpdateの後まで待つ。
            yield return null;
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // 登録解除は死亡したタイミングではなく、ゲームが終了するタイミングになっている。
            // 死亡した敵かの判定が出来るようにするため。
            EnemyManager.Release(this);

            _perception.Dispose();
            _bodyController.Dispose();
        }

        private void OnDrawGizmosSelected()
        {
            _perception?.Draw();
        }

        /// <summary>
        /// 外部から敵の行動を制御する。
        /// /// </summary>
        public void Order(EnemyOrder order) => _overrideOrder.Buffer(order);

        /// <summary>
        /// ダメージ処理。
        /// </summary>
        public void Damage(int value, string weapon = "") => _hitPoint.Damage(value, weapon);
    }
}