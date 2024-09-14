using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.Player
{
    /// <summary>
    /// コックピットのエミッションを管理するクラス
    /// </summary>
    public class CockpitEmissionController : MonoBehaviour
    {
        [SerializeField] private List<EmissionDataClass> _emissionDataList = new();
        private int _emissionId = Shader.PropertyToID("_EmissionColor");

        private void Start()
        {
            for (int i = 0; i < _emissionDataList.Count; i++)
            {
                _emissionDataList[i].SetMaerial();
            }
        }

        /// <summary>
        /// 指定したタイプのエミッションを変更する
        /// </summary>
        /// <param name="emissionTargetType"></param>
        /// <param name="color"></param>
        public void SetEmission(EmissionTargetType emissionTargetType, Color color) 
        {
            for (int i = 0; i < _emissionDataList.Count; i++) 
            {
                if (_emissionDataList[i].TargetType == emissionTargetType) 
                {
                    _emissionDataList[i].Material.SetColor(_emissionId, color);
                }
            }
        }

        /// <summary>
        /// ボタンなどから呼んで挙動を確認する用
        /// </summary>
        /// <param name="emissionTargetType"></param>
        public void TestEmission(string emissionName)
        {
            var emissionTargetType = (EmissionTargetType)System.Enum.Parse(typeof(EmissionTargetType), emissionName, true);

            for (int i = 0; i < _emissionDataList.Count; i++)
            {
                if (_emissionDataList[i].TargetType == emissionTargetType)
                {
                    _emissionDataList[i].Material.SetColor(_emissionId, new Color(1, 1, 1, 1));
                }
            }
        }

        [System.Serializable]
        public class EmissionDataClass
        {
            public Material Material => _material;

            public Renderer Renderer;
            public EmissionTargetType TargetType;
            private Material _material;


            public void SetMaerial() 
            {
                if (Renderer)
                {
                    _material = Renderer.material;
                }
                else 
                {
                    Debug.LogError("RendererがNullです");
                }
            }
        }

        public enum EmissionTargetType
        {
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch1,
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch2,
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch3,
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch4,
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch5,
            /// <summary>toggleスイッチ</summary>
            ToggleSwitch6,
            /// <summary>レバー</summary>
            LeftLever,
            /// <summary>レバー背面ボタン</summary>
            LeverBackButton,
            /// <summary>右のフライトレバー</summary>
            RightFryLever,
            /// <summary>フライトレバーの武器切り替えボタン(親指で押すボタン)</summary>
            FryLeverWeaponChangeButton,
            /// <summary>外側の２つのレバー</summary>
            OutSideLever,
        }
    }
}