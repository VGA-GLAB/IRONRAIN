using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class BossStartSeqController : SequenceControllerBase
{
    private PlayerController _playerController;
    [SerializeField] private GameObject _chaseStage;
    [SerializeField] private GameObject _bossStage;

    public void SetUp(PlayerController playerController) 
    {
        _playerController = playerController;
    }

    public async UniTask BossStart()
    {
        _chaseStage.SetActive(false);
        _bossStage.SetActive(true);
        _playerController.SeachState<PlayerStoryEvent>().BossStart();
        await UniTask.CompletedTask;
        //Bossの起動処理
    }
}
