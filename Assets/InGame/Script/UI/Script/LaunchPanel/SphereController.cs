using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SphereController : MonoBehaviour
{
    [Header("球体のアニメーター")]
    [SerializeField] private Animator _animator;

    public void ActiveAnimator()
    {
        _animator.SetTrigger("On");
    }
}