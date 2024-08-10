using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System;

namespace IronRain.Player
{
    public interface IPlayerStateModel : IDisposable
    {
        public void SetUp(PlayerEnvroment env, CancellationToken token);
        public void Start();
        public void FixedUpdate();
        public void Update();
    }
}
