using System;
using UnityEngine;

public class TestMonoBehaviour : MonoBehaviour
{
    [SerializeField]
    private string _logMessage;

    private void OnSceneLoaded()
    {
        Debug.Log(
            $"Loaded: {gameObject.name}\n" +
            $"{_logMessage}");
    }

    private void Activation()
    {
        Debug.Log(
            $"Activation: {gameObject.name}\n" +
            $"{_logMessage}");
    }

    private void Deactivation()
    {
        Debug.Log(
            $"Deactivation: {gameObject.name}\n" +
            $"{_logMessage}");
    }
}