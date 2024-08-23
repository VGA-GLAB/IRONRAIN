using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ActivationButtonCotroller : MonoBehaviour
{
    [SerializeField] private LaunchManager _launchManager;
    [NonSerialized]
    public bool _isButtonActive = false;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) && _isButtonActive)
        {
            _launchManager.StartLaunchSequence();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finger") && _isButtonActive)
        {
            _launchManager.StartLaunchSequence();
        }
    }
}