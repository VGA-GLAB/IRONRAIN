using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugScript : MonoBehaviour
{
    [SerializeField] RobotInputDebug _robotInputDebug;
    [SerializeField] private bool _robotInput;

    private void Start()
    {
        if (_robotInput) 
        {
            _robotInputDebug.Active();
        }
    }
}
