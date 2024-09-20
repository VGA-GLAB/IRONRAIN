using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.Movie
{
    public class MovieCharacter : MonoBehaviour
    {
        [SerializeField] string _attackStateName;
        [SerializeField] float _speedX;
        [SerializeField] float _speedZ;
        [SerializeField] Transform _muzzle;

        Animator _animator;
        Transform _rotate;

        bool _isFirePlaying;

        void Start()
        {
            _animator = GetComponentInChildren<Animator>();
            _animator.Play("Idle");
            _rotate = transform.FindChildRecursive("Rotate");
        }

        void Update()
        {
            _animator.SetFloat("SpeedX", _speedX);
            _animator.SetFloat("SpeedZ", _speedZ);

            if (Input.GetKeyDown(KeyCode.Space)) Fire();
        }

        void ShootTheBullet()
        {
            //
        }
        
        public void Fire()
        {
            if (!_isFirePlaying)
            {
                StartCoroutine(FireAsync());
            }
        }

        IEnumerator FireAsync()
        {
            _isFirePlaying = true;

            yield return WeightControlAsync(0, 1);
            yield return PlayAttackAnimationAsync();
            yield return WeightControlAsync(1, 0);
            yield return PlayIdleAnimationAsync();

            _isFirePlaying = false;
        }

        IEnumerator WeightControlAsync(float begin, float end)
        {
            const float Speed = 5.0f;

            for (float t = 0; t <= 1.0f; t += Time.deltaTime * Speed)
            {
                float weight = Mathf.Lerp(begin, end, t);
                SetUpperBodyWeight(weight);
                yield return null;
            }

            SetUpperBodyWeight(end);
        }

        void SetUpperBodyWeight(float weight)
        {
            _animator.SetLayerWeight(Const.Layer.UpperBody, weight);
        }

        IEnumerator PlayAttackAnimationAsync()
        {
            const float ClipLength = 3.0f;

            yield return PlayAnimationAsync(_attackStateName, Const.Layer.UpperBody, ClipLength);
        }

        IEnumerator PlayIdleAnimationAsync()
        {
            // アイドル状態に戻すだけなので、特に待つ必要なし、適当な値で十分。
            const float Wait = 0.1f;

            yield return PlayAnimationAsync("Idle", Const.Layer.BaseLayer, Wait);
        }

        IEnumerator PlayAnimationAsync(string stateName, int layer, float playTime)
        {
            _animator.Play(stateName, layer);
            yield return new WaitForSeconds(playTime);
        }
    }
}
