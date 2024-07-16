using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DebugScript : MonoBehaviour
{
    [SerializeField] RobotInputDebug _robotInputDebug;
    [SerializeField] MyButton _button;
    [SerializeField] private bool _robotInput;

    private void Start()
    {
        if (_robotInput) 
        {
            _robotInputDebug.SetUp();
            _robotInputDebug.Active();
        }

        //_button.OnClickUp.Subscribe(_ => Debug.Log("ボタンが離されました")).AddTo(this);
        //_button.OnClickDown.Subscribe(_ => Debug.Log("ボタンが押されました")).AddTo(this);
    }
}
