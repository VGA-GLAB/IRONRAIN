﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;

public sealed class BassStartSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private EnemyManager _enemyManager;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        //Bossの起動処理
        //_chaseStage.SetActive(false);
        //_bossStage.SetActive(true);
        _playerController.SeachState<PlayerStoryEvent>().BossStart();
        _enemyManager.BossStart();
    }
}