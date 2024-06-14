using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{
    public SceneTest[] SceneTests;
    private void Start()
    {
       SceneTests = FindObjectsByType<SceneTest>(FindObjectsSortMode.InstanceID);
    }
}
