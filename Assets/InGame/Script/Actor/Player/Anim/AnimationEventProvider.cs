using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace IronRain.Player
{
    public class AnimationEventProvider : MonoBehaviour
    {
        public Action OnPileBunkerInjection;
        public Action OnPileBunkerBack;
        public Action OnPileBunkerHit;
        public Action OnBlokenArm;

        /// <summary>
        /// パイルバンカー射出
        /// </summary>
        public void PileBunkerInjection() { OnPileBunkerInjection?.Invoke();
        }

        /// <summary>
        /// パイルバンカーが戻った時
        /// </summary>
        public void PileBunkerBack() { OnPileBunkerBack?.Invoke(); }

        /// <summary>
        /// パイルバンカーヒット
        /// </summary>
        public void PileBunkerHit() { OnPileBunkerHit?.Invoke(); }
        /// <summary>
        /// 腕が切られた時
        /// </summary>
        public void BlokenArm() { OnBlokenArm?.Invoke(); }

        private void OnDestroy()
        {
            OnPileBunkerInjection = null;
            OnPileBunkerBack = null;
            OnPileBunkerHit = null;
            OnBlokenArm = null;
        }
    }
}
