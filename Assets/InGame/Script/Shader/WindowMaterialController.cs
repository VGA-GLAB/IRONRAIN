using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.ShaderSystem
{
    public class WindowMaterialController : MonoBehaviour
    {
        private Material[] _materials;

        private int _crackAmountPropertyID = Shader.PropertyToID("_CrackAmount");

        private void Awake()
        {
            _materials = Array.ConvertAll(GetComponentsInChildren<Renderer>(), renderer => renderer.material);
        }

        /// <summary>モニターをヒビに入れる関数</summary>
        /// <param name="crackRatio">0~1でどの程度ヒビを入れるのか</param>
        public void Crack(float crackRatio)
        {
            crackRatio = Mathf.Clamp01(crackRatio);
            
            foreach (var mat in _materials)
            {
                mat?.SetFloat(_crackAmountPropertyID, crackRatio);
            }
        }
    }
}
