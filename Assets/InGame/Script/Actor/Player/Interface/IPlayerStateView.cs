using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace IronRain.Player
{
    public interface IPlayerStateView
    {
        public void SetUp(PlayerEnvroment env, CancellationToken token);
    }
}
