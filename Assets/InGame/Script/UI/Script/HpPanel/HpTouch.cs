using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpTouch : MonoBehaviour
{
    [Header("UiZoom")]
    [SerializeField] UiZoom _uiZoom;
    [Header("体の部位")]
    [SerializeField] int _bodyPart;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Finger")
        {
            _uiZoom.OnButtomZoom(_bodyPart);
        }
    }
}
