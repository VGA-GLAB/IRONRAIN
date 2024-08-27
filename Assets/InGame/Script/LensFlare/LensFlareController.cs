using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.LensFlare
{
    public sealed class LensFlareController : MonoBehaviour
    {
        [SerializeField, Header("コックピットの正面カメラ")] private Camera _forwardCamera = default;

        [SerializeField, Header("一番外側のレンズフレアの距離")]
        private float _distance = 100F;
        [SerializeField, Header("基準のDistanceを1としてAlphaのグラデーション")]
        private AnimationCurve _alphaCurve;
        [SerializeField, Header("基準のDistanceを1としてScaleのグラデーション")]
        private AnimationCurve _scaleCurve;

        private Material[] _childrenMats;
        private readonly int _baseColorPropertyID = Shader.PropertyToID("_BaseColor");

        private void Start()
        {
            if (_forwardCamera) gameObject.SetActive(false);
            
            _childrenMats = Array.ConvertAll(transform.GetComponentsInChildren<Renderer>(), x => x.material);
        }

        private void Update()
        {
            // 正面をずっとカメラに向ける
            transform.LookAt(_forwardCamera.transform);

            var sqrtDist = (transform.position - _forwardCamera.transform.position).magnitude;
            var time = sqrtDist / _distance;
            
            // アルファを更新
            var alpha = _alphaCurve.Evaluate(time);
            foreach (var mat in _childrenMats)
            {
                mat.SetColor(_baseColorPropertyID, new (1, 1, 1, alpha));
            }
            
            // でかさを更新
            var scale = _scaleCurve.Evaluate(time);
            transform.localScale = new(scale, scale, scale);
        }
        
    }
}
