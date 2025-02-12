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
    private IDisposable _enemySubscription;
    
    private Enemy.BlackBoard _enemyBlackBoard;
    private Enemy.Boss.BlackBoard _bossBlackBoard;
    private Enemy.Funnel.BlackBoard _funnelBlackBoard;
    
    public void Initialize()
    {
        _icon.sprite = _lockOnIcon; // スプライトを書き換える
        IdentifyEnemyType();
    }
    
    /// <summary>
    /// 敵の種類を判断してアイコンの位置の書き換えとイベント発火のタイミングを設定する
    /// </summary>
    private void IdentifyEnemyType()
    {
        // 親オブジェクトからコンポーネントを取得する
        var enemyController = GetComponentInParent<EnemyController>();
        var bossController = enemyController == null ? GetComponentInParent<BossController>() : null;
        var funnelController = (enemyController == null && bossController == null) ? GetComponentInParent<FunnelController>() : null;

        if (enemyController != null)
        {
            // 通常エネミーは死亡した時にイベント発火するように設定
            _enemyBlackBoard = enemyController.BlackBoard as Enemy.BlackBoard;
            SetIconPositionAndScale(_offset, Vector3.one);
            SubscribeToEnemyState(() => !_enemyBlackBoard.IsAlive);
        }
        else if (bossController != null)
        {
            // ボスはQTEが始まったタイミングでイベント発火するように設定
            _bossBlackBoard = bossController.BlackBoard as Enemy.Boss.BlackBoard;
            SetIconPositionAndScale(_offset, Vector3.one);
            SubscribeToEnemyState(() => _bossBlackBoard.IsQteStarted);
        }
        else if (funnelController != null)
        {
            // ファンネルは小さいので特別に位置と拡大率を変更した後、死亡した時にイベント発火するように設定
            _funnelBlackBoard = funnelController.Perception.Ref.BlackBoard;
            SetIconPositionAndScale(_funnelOffset, new Vector3(0.4f, 0.4f, 0.4f));
            SubscribeToEnemyState(() => !_funnelBlackBoard.IsAlive);
        }
    }
    
    /// <summary>
    /// アイコンのオフセットとスケールを変更する
    /// </summary>
    private void SetIconPositionAndScale(Vector3 position, Vector3 scale)
    {
        _icon.rectTransform.localPosition = position;
        transform.localScale = scale;
    }

    /// <summary>
    /// エネミーの状態を監視して登録されているタイミングでUIを非表示にするためのイベントを発火
    /// </summary>
    private void SubscribeToEnemyState(Func<bool> condition)
    {
        DisposeSubscription();
        
        _enemySubscription = Observable.EveryUpdate()
            .Where(_ => condition())
            .Take(1)
            .Subscribe(_ =>
            {
                OnRelease?.Invoke(this);
                DisposeSubscription();
            })
            .AddTo(this);
    }
    
    /// <summary>
    /// 購読をリセットする
    /// </summary>
    private void DisposeSubscription()
    {
        _enemySubscription?.Dispose();
        _enemySubscription = null;
    }
}
