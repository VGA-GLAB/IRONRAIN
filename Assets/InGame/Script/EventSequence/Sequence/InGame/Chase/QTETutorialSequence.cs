using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

public sealed class QTETutorialSequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager = default;
    [SerializeField] private PlayerQTE _playerQTE = default;
    //[Header("どの程度の距離でQteをスタートさせるか")]
    //[SerializeField] private int _qteStartDis;
    private PlayerQTEModel _playerQTEModel = default;
    [Header("QTEを開始するまでの待ち時間")]
    [SerializeField] private float _qteAwaitTimeSec = 5F;

    // 敵を一覧で返す度にアロケーションを発生させないために、敵一覧を格納するためのリスト。
    // もし1度のアロケーション程度、気にならないのならば内部でリスト作って返すように作る
    // もしくは、イテレータで返すように作り直すから遠慮なく言ってほしい。
    List<EnemyController> _enemies = new List<EnemyController>();

    private void Start()
    {
        _playerQTEModel = _playerQTE.QTEModel;
    }

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: ct);

        // 盾持ちを出現させる
        _enemyManager.DetectPlayer(EnemyManager.Sequence.QTETutorial);

        // QTEチュートリアルの敵一覧をリストで返す。
        if (_enemyManager.TryGetEnemies(EnemyManager.Sequence.QTETutorial, _enemies))
        {
            // QTEチュートリアル用の敵
            Transform enemy = _enemies[0].transform;

            // QTEチュートリアルの敵のID、これを_playerQTEModel.StartQTEメソッドの引数にしてやれば大丈夫。
            System.Guid id = _enemies[0].BlackBoard.ID;
            Debug.Log("待機開始");
            /* 出現した敵が良い感じの位置に来るまでawait */
            await UniTask.WaitForSeconds(_qteAwaitTimeSec, cancellationToken: ct);

            /* QTE開始、成功をawait */
            await RepeatQTE(id);
        }
    }

    /// <summary>QTEイベントの成功を待つ</summary>
    public async UniTask RepeatQTE(System.Guid id)
    {
        // 比較用
        QTEResultType result = QTEResultType.Failure;

        // 成功するまでQTEを繰り返す
        while (result != QTEResultType.Success)
        {
            result = await _playerQTEModel.StartQTE(id);
        }
    }
}
