using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.ShaderSystem
{
    public class WindowMaterialController : MonoBehaviour
    {
        [Header("ひびのテクスチャ")]
        [SerializeField] private Texture2D[] _centerTextures;
        [SerializeField] private Texture2D[] _rightTextures;
        [SerializeField] private Texture2D[] _leftTextures;
        
        [SerializeField] private Renderer _centerRenderer;
        [SerializeField] private Renderer _rightRenderer;
        [SerializeField] private Renderer _leftRenderer;

        private readonly int _crackAmountPropertyID = Shader.PropertyToID("_CrackAmount");
        private readonly int _crackTexturePropertyId = Shader.PropertyToID("_CrackTex");
        
        private CrackType currentCrack = CrackType.None;
        
        [ContextMenu("TestCrack")]
        private void TestCrack()
        {
            currentCrack++;
            currentCrack = currentCrack > CrackType.Type4 ? CrackType.None : currentCrack;
            if (currentCrack is CrackType.None) return;
            
            Crack(currentCrack);
        }

        private void Start()
        {
            Crack(0F);
        }

        /// <summary>日々の形基本的に1から4にかけて日々が増える</summary>
        public enum CrackType
        {
            None = -1,
            Type1 = 0,
            Type2 = 1,
            Type3 = 2,
            Type4 = 3
        }

        /// <summary>モニターをヒビに入れる関数</summary>
        /// <param name="crackRatio">0~1でどの程度ヒビを入れるのか</param>
        public void Crack(float crackRatio)
        {
            crackRatio = Mathf.Clamp01(crackRatio);
            
            _centerRenderer.sharedMaterial.SetFloat(_crackAmountPropertyID, crackRatio);
            _rightRenderer.sharedMaterial.SetFloat(_crackAmountPropertyID, crackRatio);
            _leftRenderer.sharedMaterial.SetFloat(_crackAmountPropertyID, crackRatio);
        }

        public void Crack(CrackType crackType)
        {
            if (crackType is CrackType.None)
            {
                Crack(0F);
            }
            else
            {
                Crack(1F);
                
                _centerRenderer.sharedMaterial.SetTexture(_crackTexturePropertyId, _centerTextures[(int)crackType]);
                _rightRenderer.sharedMaterial.SetTexture(_crackTexturePropertyId, _rightTextures[(int)crackType]);
                _leftRenderer.sharedMaterial.SetTexture(_crackTexturePropertyId, _leftTextures[(int)crackType]);
            }
        }
    }
}
