using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using Enemy;

namespace IronRain.Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator _anim;
        [SerializeField] private Animator _pileAnim;
        [SerializeField] private GameObject _fallObj;
        [SerializeField] private Transform _fallInsPos;
        [SerializeField] private List<GameObject> _rightArm = new();
        [SerializeField] private ParticleEffect _particleEffect;
        private CancellationToken _token;
        private PlayerEnvroment _playerEnvroment;

        private bool _isCustomSpeed = false;

        private void Awake()
        {
            _token = this.GetCancellationTokenOnDestroy();
        }

        public void SetUp(PlayerEnvroment playerEnvroment) 
        {
            _playerEnvroment = playerEnvroment;
        }

        private void Update()
        {
            if (!_isCustomSpeed)
            {
                _anim.speed = ProvidePlayerInformation.TimeScale;
            }
        }

        public async UniTask JetpackPurgeAnim()
        {
            _anim.Play("Purge");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
            PlayerLoopTiming.Update, _token);
        }

        public async UniTask FallAnim()
        {
            var obj = Instantiate(_fallObj, _fallInsPos);
            Animator anim = obj.GetComponent<Animator>();
            await UniTask.WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
            PlayerLoopTiming.Update, _token);
            obj.SetActive(false);
        }

        public async UniTask QteAttack()
        {
            _anim.SetTrigger("PileAttackTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8,
            PlayerLoopTiming.Update, _token);

            _isCustomSpeed = true;
            _anim.speed = 0;
        }

        //
        public async UniTask QteGuard() 
        {
            CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_Damage_05");
            _anim.SetTrigger("PileGuardTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
            PlayerLoopTiming.Update, _token);
        }

        public async UniTask QteFinish()
        {
            _anim.SetTrigger("PileFinishTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
            PlayerLoopTiming.Update, _token);
        }

        public async UniTask PileFire() 
        {
            _pileAnim.SetTrigger("FireTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
                PlayerLoopTiming.Update, _token);
        }

        public async UniTask EndPileFire() 
        {
            _pileAnim.SetTrigger("EndFireTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
                PlayerLoopTiming.Update, _token);
            _pileAnim.speed = 0;

        }

        public async UniTask PileFinish()
        {
            _pileAnim.SetTrigger("FinishTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95,
                PlayerLoopTiming.Update, _token);
        }

        /// <summary>
        /// アニメーションを指定した時間で止める
        /// </summary>
        /// <returns></returns>
        public async UniTask AnimationEndStop(float normalizedTime)
        {
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedTime,
            PlayerLoopTiming.Update, _token);
            _isCustomSpeed = true;
            _anim.speed = 0;
        }

        public void AnimationSpeedReset()
        {
            _isCustomSpeed = false;
            _anim.speed = ProvidePlayerInformation.TimeScale;
        }

        public async UniTask NextAnim() 
        {
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.1,
     PlayerLoopTiming.Update, _token);
        }

        public async UniTask NextStopAnim(float normalizedTime) 
        {
            AnimationSpeedReset();
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.1,
            PlayerLoopTiming.Update, _token);

            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= normalizedTime,
            PlayerLoopTiming.Update, _token);
            _isCustomSpeed = true;
            _anim.speed = 0;
        }

        public async UniTask PlayerAssaultDusarm()
        {
            _anim.SetTrigger("AssaultDisarmTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
                PlayerLoopTiming.Update, _token);
        }

        public async UniTask PlayerRocketDisarm()
        {
            _anim.SetTrigger("RocketDisarmTrigger");
            await UniTask.WaitUntil(() => _anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98,
                PlayerLoopTiming.Update, _token);
        }

        public async UniTask LeftArmDestroy() 
        {
            for (int i = 0; i < _rightArm.Count; i++) 
            {
                _rightArm[i].SetActive(false);
            }
            _particleEffect.Play();
            CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_BlokenArm");
            await UniTask.CompletedTask;
        }
    }
}
