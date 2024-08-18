using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.ShaderSystem
{
    public class CurveShaderManager : MonoBehaviour
    {
        [SerializeField] private CurveType _curveType = CurveType._CURVE_TYPE_NONE;
        [SerializeField, Range(0F, 1F)] private float _factor = 1F;
        [SerializeField] private float _offset = 0F;
        [SerializeField] private float _strength = 0.1F;
        [SerializeField] private Material[] _targetMats;

        private readonly int _curveFactorPropertyID = Shader.PropertyToID("_CurveFactor");
        private readonly int _curveOffsetPropertyID = Shader.PropertyToID("_CurveOffset");
        private readonly int _curveStrengthPropertyID = Shader.PropertyToID("_CurveStrength");

        private CurveType _lastType = CurveType._CURVE_TYPE_NONE;
        
        private enum CurveType
        {
            _CURVE_TYPE_NONE,
            _CURVE_TYPE_CAMERA_FORWARD,
            _CURVE_TYPE_CAMERA_DISTANCE,
            _CURVE_TYPE_WORLD_FORWARD
        }

        private void UpdateShaderParams()
        {
            foreach (var mat in _targetMats)
            {
                mat.SetFloat(_curveFactorPropertyID, _factor);
                mat.SetFloat(_curveOffsetPropertyID, _offset);
                mat.SetFloat(_curveStrengthPropertyID, _strength);
                
                mat.DisableKeyword(_lastType.ToString());
                mat.EnableKeyword(_curveType.ToString());
            }

            _lastType = _curveType;
        }
        
        private void OnValidate()
        {
            UpdateShaderParams();
        }
    }
}
