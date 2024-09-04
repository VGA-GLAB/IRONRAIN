using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Enemy.NPC
{
    public class Callback
    {
        public event UnityAction OnAppearAction;
        public event UnityAction OnAttackAction;

        public void InvokeAppearAction()
        {
            OnAppearAction?.Invoke();
        }

        public void InvokeAttackAction()
        {
            OnAttackAction?.Invoke();
        }
    }
}
