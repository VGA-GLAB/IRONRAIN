using System;
using Enemy;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LockOnIconView : MonoBehaviour
{
    [SerializeField] private Sprite _lockOnIcon; // ロックオンアイコンのスプライト
    [SerializeField] private Image _icon;
    public event Action<LockOnIconView> OnEnemyBroken; //敵が死亡した時に発火するイベント
    
    private EnemyController _enemyController;
    private IDisposable _enemySubscription;
    
    public void Initialize()
    {
        _icon.sprite = _lockOnIcon; // スプライトを書き換える
        _enemyController = GetComponentInParent<EnemyController>();

        _enemySubscription?.Dispose();
        
        // 敵が死亡したタイミングでアイコンをプールに戻すための処理
        _enemySubscription = Observable.EveryUpdate()
            .Where(_ => !_enemyController.BlackBoard.IsAlive) 
            .Take(1)
            .Subscribe(_ =>
            {
                OnEnemyBroken?.Invoke(this); // 敵が死亡した時のイベントを発火
                _enemySubscription.Dispose(); // 以降購読は不要なので解除しておく
            })
            .AddTo(this);
    }
}
