using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerLightController : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] private Color _color = Color.white;

    [SerializeField] private Material[] _materials = default;

    private int _colorPropertyId = Shader.PropertyToID("_Color");
    private void Awake()
    {
        foreach (var n in _materials)
        {
            n.SetColor(_colorPropertyId, _color);
        }
    }
}
