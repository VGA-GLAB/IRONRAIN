using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapBackground : MonoBehaviour
{
    [Header("Poke時のイベントを発火する。")]
    [SerializeField] private InteractableUnityEventWrapper _pokeCollision;
    [Header("Poke時のイベントを反映する。")]
    [SerializeField] private Image _background;

    void Start()
    {
        _pokeCollision.WhenSelect.AddListener(() => _background.color = Color.green);
        _pokeCollision.WhenUnselect.AddListener(() => _background.color = Color.white);
    }
}
