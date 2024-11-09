using Oculus.Interaction;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapBackground : MonoBehaviour
{
    [SerializeField,Header("Poke時のイベントを発火する。")] private InteractableUnityEventWrapper _pokeCollision;
    [SerializeField,Header("Poke時のイベントを反映する。")] private Image _background;

    void Start()
    {
        _pokeCollision.WhenSelect.AddListener(() => _background.color = Color.green);
        _pokeCollision.WhenUnselect.AddListener(() => _background.color = Color.white);
    }
}
