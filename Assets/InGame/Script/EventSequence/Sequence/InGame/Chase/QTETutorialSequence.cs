using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

public sealed class QTETutorialSequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager = default;
    [SerializeField] private PlayerController _playerCon = default;
    [Header("どの程度の距離でQteをスタートさせるか")]
    [SerializeField] private float _qteStartDis;
    private PlayerQTEModel _playerQTEModel = default;

    // 敵を一覧で返す度にアロケーションを発生させないために、敵一覧を格納するためのリスト。
    // もし1度のアロケーション程度、気にならないのならば内部でリスト作って返すように作る
    // もしくは、イテレータで返すように作り直すから遠慮なく言ってほしい。
    List<EnemyController> _enemies = new List<EnemyController>();

    private void Start()
    {
        _playerQTEModel = _playerCon.SeachState<PlayerQTE>().QTEModel;
        _playerCon.PlayerEnvroment.RemoveState(PlayerStateType.NonMoveForward);
    }

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        _playerCon.PlayerEnvroment.AddState(PlayerStateType.Inoperable);
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
            /* 出現した敵が良い感じの位置に来るまでawait */
            await UniTask.WaitUntil
                (() =>(_playerCon.transform.position - _enemies[0].transform.position).sqrMagnitude < _qteStartDis,
                PlayerLoopTiming.Update,
                ct);
            /* QTE開始、成功をawait */
            await RepeatQTE(id);
        }
        _playerCon.PlayerEnvroment.RemoveState(PlayerStateType.Inoperable);
        _playerCon.PlayerEnvroment.RemoveState(PlayerStateType.NonMoveForward);
    }

    /// <summary>QTEイベントの成功を待つ</summary>
    public async UniTask RepeatQTE(System.Guid id)
    {
        // 比較用
        QTEResultType result = QTEResultType.Failure;

        // 成功するまでQTEを繰り返す
        while (result != QTEResultType.Success)
        {
            result = await _playerQTEModel.StartQTE(id, QteType.NormalQte);
        }
    }
}
