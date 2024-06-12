using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Enemy.Control;

public class BossStartSeqController : SequenceControllerBase
{
    private PlayerController _playerController;
    [SerializeField] private GameObject _chaseStage;
    [SerializeField] private GameObject _bossStage;
    [SerializeField] private EnemyManager _enemyManager;

    public void SetUp(PlayerController playerController) 
    {
        _playerController = playerController;
    }

    public async UniTask BossStart()
    {
        _chaseStage.SetActive(false);
        _bossStage.SetActive(true);
        _playerController.SeachState<PlayerStoryEvent>().BossStart();
        //Bossの起動処理
        _enemyManager.BossStart();
    }
}
