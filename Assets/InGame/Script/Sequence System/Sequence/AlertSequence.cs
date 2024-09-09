using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class AlertSequence : ISequence
    {
        [OpenScriptButton(typeof(AlertSequence))]
        [Description("ボス戦のアラートのSequence。\nライトの明滅とアラートを流します。")]
        [SerializeField, Header("ライトの明滅一回の時間")]
        private float _blinkSpanSec = 3F;
        [SerializeField, Header("明滅の回数")]
        private int _blinkCount = 3;

        [SerializeField, Header("明滅の色")]
        private Color _blinkColor = Color.red;

        [SerializeField, Header("明滅の最大強度")]
        private float _maxIntensity = 5F;
        
        private Light[] _alertLights;
        private Transform _voiceTransform;
        
        public void SetData(SequenceData data)
        {
            _alertLights = data.AlertLights;
            _voiceTransform = data.VoiceTransform;
        }

        public async UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // 前の色と強さをとっておく
            var lastColors = new Color[_alertLights.Length];
            var lastIntensity = new float[_alertLights.Length];

            for (int i = 0; i < _alertLights.Length; i++)
            {
                lastColors[i] = _alertLights[i].color;
                _alertLights[i].color = _blinkColor;
                lastIntensity[i] = _alertLights[i].intensity;
            }
            
            for (int i = 0; i < _blinkCount; i++)
            {
                // Alertオン
                CriAudioManager.Instance.CockpitSE.Play3D(
                    _voiceTransform.position,
                    "SE",
                    "SE_Alert"
                );
                
                await BlinkAsync(ct);
            }

            // 色と強さを戻す
            for (int i = 0; i < _alertLights.Length; i++)
            {
                _alertLights[i].color = lastColors[i];
                _alertLights[i].intensity = lastIntensity[i];
            }
        }

        private async UniTask BlinkAsync(CancellationToken ct)
        {
            // 0 -> 1
            await DOTween.To(
                () => 0F,
                intensity =>
                {
                    foreach (var light in _alertLights)
                    {
                        light.intensity = intensity;
                    }
                },
                _maxIntensity,
                _blinkSpanSec / 2).ToUniTask(cancellationToken: ct);
            // 1 -> 0;
            await DOTween.To(
                () => _maxIntensity,
                intensity =>
                {
                    foreach (var light in _alertLights)
                    {
                        light.intensity = intensity;
                    }
                },
                0F,
                _blinkSpanSec / 2).ToUniTask(cancellationToken: ct);
        }

        public void Skip() { }
    }
}
