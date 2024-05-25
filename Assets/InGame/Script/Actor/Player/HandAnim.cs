using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HandAnim : MonoBehaviour
{
    [SerializeField] InputActionProperty _pinchAnimationAction;
    [SerializeField] InputActionProperty _gripAnimAction;
    [SerializeField] Animator _handAnim;

    private void Update()
    {
        float trigger = _pinchAnimationAction.action.ReadValue<float>();
        _handAnim.SetFloat("Trigger", trigger);
    }
}
