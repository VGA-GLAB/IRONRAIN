using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunCon : MonoBehaviour
{
    [SerializeField] XRGrabInteractable _xRGrabInteractable;
    [SerializeField] GameObject _bulletObj;
    [SerializeField] Transform _muzzle;

    private void Awake()
    {
        _xRGrabInteractable.onActivate.AddListener(x => Shot());
    }

    public void Shot() 
    {
        Instantiate(_bulletObj, _muzzle);
    }
}
