using Enemy;
using Enemy.Funnel;
using UniRx;
using UnityEngine;

public class EnemyHPSlider : MonoBehaviour
{
    [SerializeField] private LockOn _lockOn; // ロックオン機能
    [SerializeField] private EnemyHPSliderView _view; // スライダーの表示を変更するクラス
    
    private ReactiveProperty<Transform > _currentTarget = new ReactiveProperty<Transform>(); // 現在ロックオン中のターゲット
    
    private void Start()
    {
        SubscribeToTargetChanges();
    }
    
    /// <summary>
    /// ターゲット変更を監視・変更時の処理の購読を行う
    /// </summary>
    private void SubscribeToTargetChanges()
    {
        _lockOn
            .ObserveEveryValueChanged(lockOn => lockOn.GetRockEnemy)
            .Subscribe(enemy => _currentTarget.Value = enemy?.transform)
            .AddTo(this);

        //ターゲットが変更されたときにアイコンの更新処理を呼び出す
        _currentTarget
            .Subscribe(_ => UpdateLockOnIcons())
            .AddTo(this);
    }

    /// <summary>
    /// スライダーを表示する対象を変更する
    /// </summary>
    private void UpdateLockOnIcons()
    {
        // ロックオン中の敵がいない場合、スライダーを非表示に。以降の処理は行わない
        if (_lockOn.GetRockEnemy == null)
        {
            _view.gameObject.SetActive(false);
            return;
        }
        
        SetViewParent(_lockOn.GetRockEnemy.transform); // スライダーオブジェクトを操作する
        
        // ロックオン中の敵が通常エネミーかファンネルなら、HPスライダーを設定してtrueを返す
        if (TrySetEnemy() || TrySetFunnel()) return;
        
        _view.gameObject.SetActive(false); // その他（ボス）はスライダーを非表示にする
    }
    
    /// <summary>
    /// スライダーオブジェクトをターゲットの子オブジェクトに追加
    /// </summary>
    private void SetViewParent(Transform parent)
    {
        _view.transform.SetParent(parent);
        _view.transform.localPosition = Vector3.zero;
        _view.transform.localRotation = Quaternion.identity;
    }

    /// <summary>
    /// 通常エネミーのHPスライダーを設定
    /// </summary>
    private bool TrySetEnemy()
    {
        if (_lockOn.GetRockEnemy.TryGetComponent(out EnemyController enemyController))
        {
            Enemy.BlackBoard enemyBB = enemyController.BlackBoard as Enemy.BlackBoard;
            EnemyParams enemyParam = _lockOn.GetRockEnemy.GetComponent<EnemyParams>();
            _view.Initialize(enemyBB, enemyParam.MaxHp); //スライダーの値をセットする
            return true;
        }
        
        return false;
    }

    /// <summary>
    /// ファンネルのHPスライダーを設定
    /// </summary>
    private bool TrySetFunnel()
    {
        if (_lockOn.GetRockEnemy.TryGetComponent(out FunnelController funnelController))
        {
            Enemy.Funnel.BlackBoard funnelBB = funnelController.Perception.Ref.BlackBoard;
            FunnelParams funnelParam = _lockOn.GetRockEnemy.GetComponent<FunnelParams>();
            _view.Initialize(funnelBB, funnelParam.MaxHp);
            return true;
        }

        return false;
    }
}
