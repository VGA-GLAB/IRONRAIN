using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

namespace IronRain.Player
{
    public interface IPlayerState : IDisposable
    {
        public IPlayerStateModel PlayerStateModel { get; }
        public IPlayerStateView PlayerStateView { get; }

        public void SetUp(PlayerEnvroment env, CancellationToken token);
    }
}
