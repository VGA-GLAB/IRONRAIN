using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UniRx;
using UnityEngine;

public sealed class ToggleSwitchController : MonoBehaviour
{
    [SerializeField] private ReactiveProperty<bool> _toggleInputRP = new (true);
    [SerializeField] private Transform _lever = default;
    
    public ReactiveProperty<bool> ToggleInputRP => _toggleInputRP;

    private TweenerCore<Quaternion, Vector3, QuaternionOptions> _tweener;
    
    private void Awake()
    {
        _toggleInputRP.AddTo(this);

        _toggleInputRP.Subscribe(LeverChange);
    }

    private void LeverChange(bool callback)
    {
        if (callback)
        {
            _tweener?.Kill();
            _tweener = _lever.DORotate(new(0F, 0F, 45F), 0.5F).OnComplete(() => _tweener = null);
        }
        else
        {
            _tweener?.Kill();
            _tweener = _lever.DORotate(new (0F, 0F, -45F), 0.5F).OnComplete(() => _tweener = null);
        }
    }
}
