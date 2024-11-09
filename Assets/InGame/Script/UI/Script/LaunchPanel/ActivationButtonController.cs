using System;
using UnityEngine;

public class ActivationButtonController : MonoBehaviour
{
    [SerializeField] private LaunchManager _launchManager;
    [NonSerialized] public bool _isButtonActive = false;
    
    
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