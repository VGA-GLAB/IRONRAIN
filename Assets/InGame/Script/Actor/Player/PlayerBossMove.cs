using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IronRain.Player
{
    public class PlayerBossMove : PlayerComponentBase
    {
        public PlayerBossMoveModel MoveModel { get; private set; }

        private void Awake()
        {
            MoveModel = _playerStateModel as PlayerBossMoveModel;
        }

        private void OnEnable()
        {
            MoveModel.ResetPos();
        }
    }
}
