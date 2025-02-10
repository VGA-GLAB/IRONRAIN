using System;
using Enemy;
using Enemy.Boss;
using Enemy.Funnel;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class LockOnIconView : MonoBehaviour
{
    [SerializeField] private Sprite _lockOnIcon; // ロックオンアイコンのスプライト
    [SerializeField] private Image _icon;
    [SerializeField] private Vector3 _offset = new Vector3(0, 4.25f, -2.5f); // 通常エネミー/ボス用のオフセット
    [SerializeField] private Vector3 _funnelOffset = new Vector3(0, -0.88f, -2f); // ファンネル用のオフセット
    public event Action<LockOnIconView> OnRelease; // 照準UIを非表示にするイベント
    
    private Enemy.BlackBoard _enemyBlackBoard;
    private Enemy.Boss.BlackBoard _bossBlackBoard;
    private Enemy.Funnel.BlackBoard _funnelBlackBoard;
    private IDisposable _enemySubscription;
    
    public void Initialize()
    {
        _icon.sprite = _lockOnIcon; // スプライトを書き換える
        
        _enemyBlackBoard = GetComponentInParent<EnemyController>()?.BlackBoard as Enemy.BlackBoard;

        if (_enemyBlackBoard != null)
        {
            _icon.rectTransform.localPosition = _offset;
            NormalEnemy(); // 通常エネミーの処理
        }
        else
        {
            _bossBlackBoard = GetComponentInParent<BossController>()?.BlackBoard as Enemy.Boss.BlackBoard;

            if (_bossBlackBoard != null)
            {
                _icon.rectTransform.localPosition = _offset;
                Boss(); // ボスの処理
            }
            else
            {
                _icon.rectTransform.localPosition = _funnelOffset;
                transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
                Funnel();
            }
        }
    }

    /// <summary>
    /// 通常エネミー（アサルト/バズーカ/盾）の照準UIの管理
    /// </summary>
    private void NormalEnemy()
    {
        _enemySubscription?.Dispose();
        
        // 敵が死亡したタイミングでアイコンをプールに戻すための処理
        _enemySubscription = Observable.EveryUpdate()
            .Where(_ => !_enemyBlackBoard.IsAlive) 
            .Take(1)
            .Subscribe(_ =>
            {
                OnRelease?.Invoke(this);
                _enemySubscription.Dispose(); // 以降購読は不要なので解除しておく
            })
            .AddTo(this);
    }

    private void Boss()
    {
        _enemySubscription?.Dispose();
        
        // QTEが開始されたタイミングで
        _enemySubscription = Observable.EveryUpdate()
            .Where(_ => _bossBlackBoard.IsQteStarted) 
            .Take(1)
            .Subscribe(_ =>
            {
                OnRelease?.Invoke(this);
                _enemySubscription.Dispose(); // 以降購読は不要なので解除しておく
            })
            .AddTo(this);
    }

    private void Funnel()
    {
        _funnelBlackBoard = GetComponentInParent<FunnelController>()?.Perception.Ref.BlackBoard;
        
        _enemySubscription?.Dispose();
        
        // 敵が死亡したタイミングでアイコンをプールに戻すための処理
        _enemySubscription = Observable.EveryUpdate()
            .Where(_ => !_funnelBlackBoard.IsAlive) 
            .Take(1)
            .Subscribe(_ =>
            {
                OnRelease?.Invoke(this);
                _enemySubscription.Dispose(); // 以降購読は不要なので解除しておく
            })
            .AddTo(this);
    }
}
