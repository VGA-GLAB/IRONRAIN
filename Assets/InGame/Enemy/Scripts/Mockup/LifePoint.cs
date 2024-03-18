using UnityEngine;
using UnityEngine.Events;

namespace Enemy.Mockup
{
    /// <summary>
    /// 体力の管理
    /// </summary>
    public class LifePoint : MonoBehaviour
    {
        /// <summary>
        /// 体力が0になった際に呼び出される。
        /// </summary>
        public event UnityAction OnDeath;

        [Header("体力の設定")]
        [SerializeField] private int _max = 100;

        private int _current;

        private void Awake()
        {
            _current = _max;
        }

        /// <summary>
        /// 体力を変化させる
        /// </summary>
        public void Change(int value)
        {
            _current += value;
            _current = Mathf.Clamp(_current, 0, _max);

            // 体力が0になったコールバックを呼ぶ
            if (_current == 0) OnDeath?.Invoke();
        }
    }
}
