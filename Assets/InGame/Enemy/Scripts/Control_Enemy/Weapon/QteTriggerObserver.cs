using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// 盾持ち敵のQTE判定にプレイヤーが接触したタイミングで、装備者にQTE開始の命令をする。
    /// その命令をどう処理するかは装備者に委ねる。
    /// </summary>
    public class QteTriggerObserver : MonoBehaviour
    {
        [Header("QTE判定にプレイヤーが接触")]
        [SerializeField] private Collider _qteTrigger;

        private EnemyController _owner;
        private EnemyOrder _order;

        private void Awake()
        {
            _owner = GetComponent<EnemyController>();

            // QTE判定にプレイヤーが接触した命令
            _order = new EnemyOrder();
            _order.OrderType = EnemyOrder.Type.QteTrigger;
        }

        private void Start()
        {
            // トリガーになっているかチェックするが、あくまで監視する側なので操作はしない。
            if (!_qteTrigger.isTrigger)
            {
                Debug.LogWarning($"QTE用の判定がトリガーになっていない{gameObject.name}");
            }

            // 判定に接触時に装備者の命令処理を呼ぶ。
            _qteTrigger.OnTriggerEnterAsObservable()
                .Where(col => col.CompareTag(Const.PlayerTag))
                .Subscribe(col => _owner.Order(_order))
                .AddTo(this);
        }
    }
}
