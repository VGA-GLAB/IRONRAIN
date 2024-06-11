using Cysharp.Threading.Tasks;
using Enemy.DebugUse;
using Enemy.Extensions;
using System.Threading;
using UnityEngine;

namespace Enemy.Control
{
    /// <summary>
    /// アタッチしたオブジェクトを巡回地点に沿って動かすだけ。
    /// 他のクラスに依存しておらず、Transformを外部から参照する用途。
    /// XZ平面だけでなくY軸の移動も行う。
    /// </summary>
    public class DebugPointP : MonoBehaviour
    {
        // 基準の座標にこの配列の各要素を足した地点を巡回する。
        [SerializeField]
        private Vector3[] _points = {
            new Vector3(10, 3, 20),
            new Vector3(25, 0, 5),
            new Vector3(3, -2, -15),
            new Vector3(-22, 1, -1)
        };

        // 基準の座標を足した巡回地点。
        private Vector3[] _offsetedPoints;

        private void Start()
        {
            CreateOffsetedPoints();
            PatrolAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        // 初期位置をオフセットした巡回地点を作る。
        private void CreateOffsetedPoints()
        {
            _offsetedPoints = new Vector3[_points.Length];

            for (int i = 0; i < _points.Length; i++)
            {
                _offsetedPoints[i] = _points[i] + transform.position;
            }
        }

        private async UniTaskVoid PatrolAsync(CancellationToken token)
        {
            if (_offsetedPoints == null) return;

            for (int i = 0; ; i++)
            {
                i %= _offsetedPoints.Length;

                float t = 0;
                Vector3 start = transform.position;
                float dist = Vector3.Magnitude(start - _offsetedPoints[i]);
                while (t < 1 && !token.IsCancellationRequested)
                {
                    transform.position = Vector3.Lerp(start, _offsetedPoints[i], t);
                    t += Time.deltaTime / dist * 10;

                    await UniTask.Yield();
                }

                if (token.IsCancellationRequested) break;
                else transform.position = _offsetedPoints[i];
            }
        }

        private void OnDrawGizmos()
        {
            // Editor上で点Pを動かす度に値が更新される。
            if (!Application.isPlaying) CreateOffsetedPoints();

            // 現在地
            GizmosUtils.Sphere(transform.position, 0.33f, ColorExtensions.ThinRed);

            // 巡回地点
            if (_offsetedPoints != null)
            {
                for (int i = 0; i < _offsetedPoints.Length; i++)
                {
                    GizmosUtils.Sphere(_offsetedPoints[i], 0.2f, ColorExtensions.ThinWhite);

                    // 点同士を結ぶ線
                    int j = (i + 1) % _offsetedPoints.Length;
                    GizmosUtils.Line(_offsetedPoints[i], _offsetedPoints[j], ColorExtensions.ThinWhite);
                }
            }
        }
    }
}