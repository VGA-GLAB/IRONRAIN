using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace IronRain.ShaderSystem
{
    public class CurveShaderManager : MonoBehaviour
    {
        [SerializeField] private CurveType _curveType = CurveType._CURVE_TYPE_NONE;
        [SerializeField, Range(0F, 1F)] private float _factor = 1F;

        public float CurveFactor
        {
            get => _factor;
            set
            {
                _factor = value;

                // マテリアルの値を更新する
                foreach (var mat in _materials)
                {
                    mat.SetFloat(_curveFactorPropertyID, _factor);
                }
            }
        }
        
        [SerializeField] private float _offset = 0F;
        [SerializeField] private float _strength = 0.1F;
        [SerializeField] private float _heightOffset = 0.0F;
        [SerializeField] private GameObject[] _targetRocks;
        [SerializeField] private Material _targetTerrainMat;

        private List<Material> _materials = new();

        private readonly int _curveFactorPropertyID = Shader.PropertyToID("_CurveFactor");
        private readonly int _curveOffsetPropertyID = Shader.PropertyToID("_CurveOffset");
        private readonly int _curveStrengthPropertyID = Shader.PropertyToID("_CurveStrength");
        private readonly int _curveHeightOffsetPropertyID = Shader.PropertyToID("_CurveHeightOffset");
        
        
        private CurveType _lastType = CurveType._CURVE_TYPE_NONE;
        
        private enum CurveType
        {
            _CURVE_TYPE_NONE,
            _CURVE_TYPE_CAMERA_FORWARD,
            _CURVE_TYPE_CAMERA_DISTANCE,
            _CURVE_TYPE_WORLD_FORWARD
        }

        private void Awake()
        {
            UpdateMaterialList();
            UpdateShaderParams();
        }

        [ContextMenu("MaterialUpdate")]
        private void UpdateMaterialList()
        {
            _materials.Clear();
            
            _materials.Add(_targetTerrainMat);
            
            foreach (var rock in _targetRocks)
            {
                foreach (var renderer in rock.GetComponentsInChildren<Renderer>(true))
                {
                    _materials.Add(renderer.material);
                }
            }
        }

        private void UpdateShaderParams()
        {
            MaterialPropertyBlock propBlock = new();

            foreach (var mat in _materials)
            {
                UpdateMaterial(mat);
            }

            _lastType = _curveType;
        }
        
        /// <summary>マテリアルの値を更新する</summary>
        private void UpdateMaterial(Material material)
        {
            material.SetFloat(_curveFactorPropertyID, _factor);
            material.SetFloat(_curveOffsetPropertyID, _offset);
            material.SetFloat(_curveStrengthPropertyID, _strength);
            material.SetFloat(_curveHeightOffsetPropertyID, _heightOffset);
            
            material.EnableKeyword(CurveType._CURVE_TYPE_WORLD_FORWARD.ToString());
        }
        
        private void OnValidate()
        {
            UpdateShaderParams();
        }
    }
}
