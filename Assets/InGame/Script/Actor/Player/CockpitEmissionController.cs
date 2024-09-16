using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
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

        private SortedDictionary<EmissionTargetType, MaterialData> _emissionDic = new();
        
        private void Start()
        {
            for (int i = 0; i < _emissionDataList.Count; i++)
            {
                var target = _emissionDataList[i];
                
                target.SetMaerial();

                if (!_emissionDic.ContainsKey(target.TargetType))
                {
                    _emissionDic.Add(target.TargetType, new MaterialData(target.Material));
                }
                else
                {
                    _emissionDic[target.TargetType].Materials.Add(target.Material);
                }
            }
        }

        /// <summary>
        /// 指定したタイプのエミッションを変更する
        /// </summary>
        /// <param name="emissionTargetType"></param>
        /// <param name="color"></param>
        public void SetEmission(EmissionTargetType emissionTargetType, Color color)
        {
            var target = _emissionDic[emissionTargetType];

            foreach (var data in target.Materials)
            {
                data.SetColor(_emissionId, color);
            }
        }

        private void SetEmission(Material mat, Color color)
        {
            mat.SetColor(_emissionId, color);
        }

        public void StartBlink(EmissionTargetType target, Color beforeColor, Color afterColor, float duration)
        {
            if (TryGetEmissionData(target, out MaterialData data) && data.AnimTween is null)
            {
                data.AnimTween = DOTween.To(() => beforeColor, x =>
                    {
                        foreach (var mat in data.Materials)
                        {
                            SetEmission(mat, x);
                        }
                    }, afterColor, duration)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetLink(this.gameObject)
                    .OnKill(() =>
                    {
                        data.AnimTween = null;
                        SetEmission(target, beforeColor);
                    });
            }
        }

        public void StopBlink(EmissionTargetType target)
        {
            if (TryGetEmissionData(target, out MaterialData data) && data.AnimTween is not null)
            {
                data.AnimTween.Kill();
                data.AnimTween = null;
            }
        }

        private bool TryGetEmissionData(EmissionTargetType target, out MaterialData data)
        {
            var isGet = _emissionDic.ContainsKey(target);

            data = isGet ? _emissionDic[target] : null;

            return isGet;
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

        private class MaterialData
        {
            public List<Material> Materials { get; set; }
            public Tween AnimTween { get; set; } = null;

            public MaterialData(params Material[] materials)
            {
                Materials = new(materials.Length);

                foreach (var mat in materials)
                {
                    Materials.Add(mat);
                }
            }

            public MaterialData()
            {
                Materials = new();
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
            /// <summary>攻撃ボタン</summary>
            FireTrigger
        }
    }
}