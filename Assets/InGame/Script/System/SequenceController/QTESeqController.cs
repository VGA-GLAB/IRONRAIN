using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;

public class QTESeqController : MonoBehaviour
{
    [SerializeField] private EnemyManager _enemyManager = default;
    [SerializeField] private PlayerQTE _playerQTE = default;
    private PlayerQTEModel _playerQTEModel = default;
    [Header("QTEを開始するまでの待ち時間")]
    [SerializeField] private float _qteAwaitTimeSec = 5F;

    private void Start()
    {
        _playerQTEModel = _playerQTE.QTEModel;
    }
    /// <summary>QTESeqの処理</summary>
    /// <param name="cancellationToken"></param>
    public async UniTask QTESeqAsync(CancellationToken cancellationToken)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: cancellationToken);

        // 盾持ちを出現させる
        _enemyManager.DetectPlayer(EnemyManager.Sequence.Tutorial);

        // 最初の待ち時間
        await UniTask.WaitForSeconds(_qteAwaitTimeSec, cancellationToken: cancellationToken);

        // QTEを開始させて、成功を待機する
        await RepeatQTE();
    }

    /// <summary>QTEイベントの成功を待つ</summary>
    public async UniTask RepeatQTE()
    {
        // 比較用
        QTEResultType result = QTEResultType.Failure;

        // 成功するまでQTEを繰り返す
        while (result != QTEResultType.Success)
        {
            result = await _playerQTEModel.StartQTE();
        }
    }
}
