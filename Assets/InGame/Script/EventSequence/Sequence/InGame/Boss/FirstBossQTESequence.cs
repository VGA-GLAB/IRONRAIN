using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;
using Enemy.Control.Boss;

public sealed class FirstBossQTESequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private BossController _bossController;
    [SerializeField] private PlayerController _playerController;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        _enemyManager.BossFirstQte();

        var qteModel = _playerController.SeachState<PlayerQTE>().QTEModel;

        await qteModel.StartQTE(Guid.Empty, QteType.BossQte1);
    }
}
