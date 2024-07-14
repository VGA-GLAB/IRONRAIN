using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunCon : MonoBehaviour
{
    [SerializeField] GameObject _bulletObj;
    [SerializeField] Transform _muzzle;

    private void Awake()
    {

    }

    public void Shot() 
    {
        Instantiate(_bulletObj, _muzzle);
    }
}
