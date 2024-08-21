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

        /// <summary>日々の形基本的に1から4にかけて日々が増える</summary>
        public enum CrackType
        {
            None,
            Type1,
            Type2,
            Type3,
            Type4
        }

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

        public void Crack(CrackType crackType)
        {
            // Todo: テクスチャが来たらそれを差し替える処理に変更する
            if (crackType is CrackType.None) return;
            
            foreach (var mat in _materials)
            {
                mat?.SetFloat(_crackAmountPropertyID, 0.5F);
            }
        }
    }
}
