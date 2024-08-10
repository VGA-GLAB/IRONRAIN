﻿using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;
using Enemy.Boss;

namespace Enemy
{
    /// <summary>
    /// 近接攻撃の装備。
    /// 攻撃タイミングはアニメーションイベント任せ。
    /// </summary>
    public class MeleeEquipment : Equipment
    {
        [Header("攻撃エフェクト(任意)")]
        [SerializeField] private Effect _attackEffect;
        [Header("範囲の設定")]
        [SerializeField] private float _forwardOffset;
        [SerializeField] private float _heightOffset;
        [Min(1.0f)]
        [SerializeField] private float _radius = 3.0f;

        private Transform _rotate;
        private AnimationEvent _animationEvent;
        private IOwnerTime _owner;

        private void Awake()
        {
            _rotate = FindRotate();
            // Animatorが1つだけの前提。
            _animationEvent = GetComponentInChildren<AnimationEvent>();
        }

        private void Start()
        {
            // 雑魚敵とボスの両用。
            if (TryGetComponent(out EnemyController e)) _owner = e.BlackBoard;
            else if (TryGetComponent(out BossController b)) _owner = b.BlackBoard;
        }

        private void OnEnable()
        {
            _animationEvent.OnMeleeAttackStart += Collision;
        }

        private void OnDisable()
        {
            _animationEvent.OnMeleeAttackStart -= Collision;
        }

        // 当たり判定を出してダメージを与える。
        private void Collision()
        {
            // 球状の当たり判定なので対象が上下にズレている場合は当たらない場合がある。
            RaycastExtensions.OverlapSphere(Origin(), _radius, col =>
            {
                if (col == null) return;
                
                // コライダーと同じオブジェクトにコンポーネントが付いている前提。
                if (col.TryGetComponent(out IDamageable dmg)) dmg.Damage(1);
            });
            
            // 判定の瞬間の演出
            if (_attackEffect != null) _attackEffect.Play(_owner);

            OnCollision();

            // タイミングを更新。
            LastAttackTiming = Time.time;
        }

        // 攻撃の基準となる座標を返す。
        private Vector3 Origin()
        {
            if (_rotate == null) return transform.position;

            // Y軸以外で回転しても正常な値を返す
            Vector3 f = _rotate.forward * _forwardOffset;
            Vector3 h = _rotate.up * _heightOffset;

            return transform.position + f + h;
        }

        /// <summary>
        /// 座標が攻撃範囲内かを返す。
        /// </summary>
        public bool IsWithinRange(in Vector3 point)
        {
            return (Origin() - point).sqrMagnitude <= _radius * _radius;
        }

        /// <summary>
        /// 派生クラスで射撃する際に追加で呼ぶ処理。
        /// </summary>
        protected virtual void OnCollision() { }

        private void OnDrawGizmosSelected()
        {
            // 攻撃範囲。
            GizmosUtils.WireCircle(Origin(), _radius, ColorExtensions.ThinRed);
        }
    }
}