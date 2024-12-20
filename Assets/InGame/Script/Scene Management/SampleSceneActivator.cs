using System;
using UnityEngine;

public class SampleSceneActivator : MonoBehaviour
{
    [SerializeField]
    private string _sceneName;
    [SerializeField]
    private KeyCode _key;

    private void Update()
    {
        if (Input.GetKeyDown(_key))
        {
            IronRain.SceneManagement.SceneManager.Activation(_sceneName);
        }
    }
}