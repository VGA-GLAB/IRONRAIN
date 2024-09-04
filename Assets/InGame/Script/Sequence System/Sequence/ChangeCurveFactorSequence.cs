using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using IronRain.ShaderSystem;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class ChangeCurveFactorSequence : ISequence
    {
        [OpenScriptButton(typeof(ChangeCurveFactorSequence))]
        [Description("地形のカーブを変更するSequence\n何秒間でどの程度のCurveの強さに変えるのか指定してください")]
        [Header("Curve変更する際のAnimationの時間(秒)")]
        [SerializeField] private float _duration = 1F;
        [Header("最終的にどの程度のカーブの強さに変更するのか")]
        [SerializeField] private float _endValue = 0F;
        
        private CurveShaderManager _curveManager;
        
        public void SetData(SequenceData data)
        {
            _curveManager = data.CurveManager;
        }

        public UniTask PlayAsync(CancellationToken ct, Action<Exception> exceptionHandler = null)
        {
            // カーブの強さを変更している
            DOTween.To(
                () => _curveManager.CurveFactor,
                x => _curveManager.CurveFactor = x,
                _endValue,
                _duration).ToUniTask(cancellationToken: ct).Forget();

            return UniTask.CompletedTask;
        }

        public void Skip()
        {
            _curveManager.CurveFactor = _endValue;
        }
    }
}
