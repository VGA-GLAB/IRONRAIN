using Cysharp.Threading.Tasks;
using Enemy.DebugUse;
using Enemy.Extensions;
using System.Threading;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// AddComponentでGameObjectに追加することで、ボスステージの点Pを仮で動かす。
    /// XZ平面だけでなくY軸の移動も行う。
    /// </summary>
    public class BossStagePivotTempMove : MonoBehaviour
    {
        // 初期位置を基準にした地点を巡回する。
        private Vector3[] _points = {
            new Vector3(10, 3, 20),
            new Vector3(25, 0, 5),
            new Vector3(3, -2, -15),
            new Vector3(-22, 1, -1)
        };

        private void Start()
        {
            CreateOffsetPoints();
            PatrolAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // 初期位置をオフセットした巡回地点を作る。
        private void CreateOffsetPoints()
        {
            for (int i = 0; i < _points.Length; i++)
            {
                _points[i] += transform.position;
            }
        }

        private async UniTaskVoid PatrolAsync(CancellationToken token)
        {
            for (int i = 0; ; i++)
            {
                i %= _points.Length;

                float t = 0;
                Vector3 start = transform.position;
                float dist = Vector3.Magnitude(start - _points[i]);
                while (t < 1 && !token.IsCancellationRequested)
                {
                    transform.position = Vector3.Lerp(start, _points[i], t);
                    t += Time.deltaTime / dist * 10;

                    await UniTask.Yield();
                }

                if (token.IsCancellationRequested) break;
                else transform.position = _points[i];
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                GizmosUtils.Sphere(transform.position, 0.33f, ColorExtensions.ThinRed);

                for (int i = 0; i < _points.Length; i++)
                {
                    GizmosUtils.Sphere(_points[i], 0.2f, ColorExtensions.ThinWhite);

                    // 点同士を結ぶ線
                    int j = (i + 1) % _points.Length;
                    GizmosUtils.Line(_points[i], _points[j], ColorExtensions.ThinWhite);
                }
            }
        }
    }
}