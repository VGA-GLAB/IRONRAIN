using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HomingMissile : MonoBehaviour
{
    [Header("初速")]
    [Range(10.0f, 30.0f)]
    [SerializeField] private float _initialSpeed = 30.0f;

    [Range(0.1f, 3.0f)]
    [Header("目標の情報の更新間隔(秒)")]
    [Tooltip("高速で動く目標に対して撃つ場合は値を小さくすると、より正確に狙えるが計算量が多くなる。")]
    [SerializeField] private float _updateDuration = 1.0f;

    [Range(0.1f, 2.0f)]
    [Header("目標に向かう力の強さ")]
    [SerializeField] private float _turnPower = 1.0f;

    private Vector3 _velocity;
    private Vector3 _targetDirection;
    private float _accuracy;
    private Transform _target;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    /// <summary>
    /// 発射後、目標を追尾し必ずヒットする。
    /// </summary>
    /// <param name="target">目標</param>
    /// <param name="launch">発射方向</param>
    /// <param name="onHit">目標にヒットした際に呼ばれるコールバック</param>
    public void Fire(Transform target, Vector3 launch, UnityAction onHit = null)
    {
        launch.Normalize();

        _velocity = launch * _initialSpeed;
        _target = target;

        StartCoroutine(UpdateAsync(onHit));
    }

    // 一定間隔で目標の情報を更新しつつ、目標に向けて飛ばす。
    // ヒットした場合は演出を再生し、画面から削除する。
    private IEnumerator UpdateAsync(UnityAction onHit)
    {
        Coroutine updateTargetDataAsync = StartCoroutine(UpdateTargetDataAsync());

        while (!IsCollision())
        {
            VelocityUpdate();
            AccuraryUpdate();
            TransformUpdate();

            yield return null;
        }

        StopCoroutine(updateTargetDataAsync);
        Damage();
        onHit?.Invoke();

        yield return HitEffectAsync();

        Delete();
    }

    // 速度を更新。
    private void VelocityUpdate()
    {
        Vector3 dir = _velocity.normalized; // 仮
        Vector3 forward = Vector3.Lerp(dir, _targetDirection, _accuracy);
        _velocity = forward * _initialSpeed;
    }

    // どのくらい目標に向かせるかを更新。
    private void AccuraryUpdate()
    {
        _accuracy += Time.deltaTime * _turnPower;
        _accuracy = Mathf.Clamp01(_accuracy);
    }

    // 移動と回転。
    private void TransformUpdate()
    {
        _transform.position += _velocity * Time.deltaTime;
        _transform.forward = _velocity;
    }

    // ヒットしたかどうかを距離の二乗で判定。
    private bool IsCollision()
    {
        const float HitThreshold = 1.0f;

        float targetSqrDist = (_target.position - _transform.position).sqrMagnitude;
        return targetSqrDist < HitThreshold;
    }

    // 目標にダメージを与える処理。
    private void Damage()
    {
        const int Value = int.MaxValue / 2;
        const string WeaponName = "xxx";
        if (_target.TryGetComponent(out IDamageable d)) d.Damage(Value, WeaponName);
    }

    // 音とかエフェクトとか出して消す場合はｺｺ。
    private IEnumerator HitEffectAsync()
    {
        yield return null;
    }

    // 画面から削除する。
    private void Delete()
    {
        gameObject.SetActive(false);
    }

    // 目標の情報は毎フレーム更新せずとも一定間隔で十分。
    private IEnumerator UpdateTargetDataAsync()
    {
        WaitForSeconds duration = new WaitForSeconds(_updateDuration);

        while (true)
        {
            _targetDirection = (_target.position - _transform.position).normalized;
            yield return duration;
        }
    }
}