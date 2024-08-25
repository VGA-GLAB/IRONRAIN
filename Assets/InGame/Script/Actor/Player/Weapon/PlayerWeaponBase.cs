using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cysharp.Threading.Tasks;
using UniRx;
using System;

namespace IronRain.Player
{
    [RequireComponent(typeof(BulletPool))]
    public abstract class PlayerWeaponBase : MonoBehaviour, IDisposable
    {
        public PlayerWeaponParams WeaponParam => _params;
        public int CurrentBullets => _currentBullets;
        public GameObject WeaponObject => _weaponObject;
        public IReactiveProperty<bool> IsReload => _isReload;

        [SerializeField] protected GameObject _weaponObject;
        [SerializeField] protected PlayerWeaponParams _params;
        [SerializeField] protected string _shotSeCueName;
        [SerializeField] protected BulletPool _bulletPool;
        [SerializeField] protected Transform _effectPos;
        [SerializeField] protected ParticleEffect _muzzleFlashEffect;
        [SerializeField] protected ParticleEffect _smokeEffect;
        [SerializeField] protected Effect _cartridgeEffect;


        protected bool _isShotInput;
        [Tooltip("現在の弾数")]
        protected int _currentBullets;
        private float _currentTime;
        [Tooltip("射撃中かどうか")]
        private bool _isFire;
        [Tooltip("リロード中かどうか")]
        private ReactiveProperty<bool> _isReload = new();
        private PlayerEnvroment _playerEnvroment;
        protected EffectOwnerTime _effectOwnerTime = new();

        public virtual void SetUp(PlayerEnvroment playerEnvroment)
        {
            _playerEnvroment = playerEnvroment;
        }

        protected virtual void Start()
        {
            _currentBullets = _params.MagazineSize;
        }

        private void Update()
        {
            if (_playerEnvroment == null) return;

            if (!_playerEnvroment.PlayerState.HasFlag(PlayerStateType.MultiLockOn)) 
            {
                _playerEnvroment.RaderMap.NearEnemyLockon();
            }
            //次の発射までの計算
            _currentTime += Time.deltaTime;
            if (_currentTime > _params.ShotRate)
            {
                _isFire = true;
            }
        }
        
        /// <summary>
        /// ショットボタンが押された時
        /// </summary>
        public virtual void ShotTrigger()
        {
            if (_isFire && 0 < _currentBullets && !_isReload.Value)
            {
                //後でオブジェクトプールに
                var bulletCon = _bulletPool.GetBullet(_params.WeaponType);
                bulletCon.SetUp(
                    _playerEnvroment.RaderMap.GetRockEnemy,
                    _params.ShotDamage,
                    _playerEnvroment.PlayerTransform.forward,
                    _params.WeaponName);

                CriAudioManager.Instance.SE.Play("SE", _shotSeCueName);

                _isFire = false;
                _currentTime = 0;
                _currentBullets--;
            }

            if (_currentBullets == 0 && !_isReload.Value)
            {
                Reload().Forget();
            }
        }

        /// <summary>
        /// 弾が出たとき
        /// </summary>
        public virtual void OnShot() 
        {

        }


        /// <summary>
        /// マルチショット
        /// </summary>
        public void MulchShot()
        {
            //var lockOnEnemys = _playerEnvroment.RaderMap.MultiLockEnemys;

            //for (int i = 0; i < lockOnEnemys.Count; i++)
            //{
            //    var bulletCon = _bulletPool.GetBullet();
            //    bulletCon.SetUp(
            //        _playerEnvroment.RaderMap.MultiLockEnemys[i],
            //        _params.ShotDamage,
            //        _playerEnvroment.PlayerTransform.forward,
            //        _params.WeaponName);
            //}
        }

        /// <summary>
        /// リロードの処理
        /// </summary>
        protected virtual async UniTask Reload()
        {
            _isReload.Value = true;
            //アニメーション挟む
            await UniTask.WaitForSeconds(_params.ReloadTime,false,PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
            _currentBullets = _params.MagazineSize;
            _isReload.Value = false;
        }

        public virtual void Dispose()
        {
            _isReload.Dispose();
        }

        public class EffectOwnerTime : IOwnerTime
        {
            public float PausableDeltaTime => ProvidePlayerInformation.TimeScale;
        }

    }
}
