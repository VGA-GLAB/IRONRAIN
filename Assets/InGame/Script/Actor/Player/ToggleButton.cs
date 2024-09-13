using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InputProvider;

public class ToggleButton : MonoBehaviour
{
    [SerializeField] private Animator _toggleAnimator;
    [SerializeField] private bool _isOn;
    [SerializeField] List<Renderer> _renderer = new();
    [SerializeField] InputType _inputType;

    private int _emissionId = Shader.PropertyToID("_EmissionColor");
    private List<Material> _materialList = new();

    private void Start()
    {
        for (int i = 0; i < _renderer.Count; i++) 
        {
            _materialList.Add(_renderer[i].material);
        }
    }

    private void Update()
    {
        _toggleAnimator.SetBool("On", _isOn);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finger") 
        {
            _isOn = !_isOn;
            InputProvider.Instance.CallEnterInput(_inputType);
        }
    }

    public void TestEmission() 
    {
        SetEmission(new Color(1, 1, 1, 1));
    }

    public void SetEmission(Color value) 
    {
        for (int i = 0; i < _renderer.Count; i++) 
        {
            _materialList[i].SetColor(_emissionId, value);
        }
    }
}
