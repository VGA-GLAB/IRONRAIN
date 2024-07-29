using Enemy.DebugUse;
using Enemy.Extensions;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Enemy.Control
{
    /// <summary>
    /// 追跡シーン、敵との乱戦中に登場するNPC。
    /// 目標に向かって移動し、一定距離まで近づいたら撃破、そのまま直進して退場する。
    /// SetActiveの切り替えで有効無効を切り替える。
    /// </summary>
    [RequireComponent(typeof(BuddyNpcParams))]
    public class BuddyNpcController : Character, INpc
    {
        [SerializeField] private Character _target;

        private Transform _rotate;
        private BuddyNpcParams _params;

        GameObject INpc.GameObject => gameObject;
        EnemyManager.Sequence INpc.Sequence => EnemyManager.Sequence.MultiBattle;

        private void Awake()
        {
            _rotate = FindRotate();
            _params = GetComponent<BuddyNpcParams>();
        }

        private void Start()
        {
            // 自身を登録しておき、必要になるまで画面から隠しておく。
            Hide();
            EnemyManager.Register<INpc>(this);
        }

        private void OnDestroy()
        {
            // イベント終了後も参照されるかもしれないので、登録解除するタイミングはゲーム終了時。
            EnemyManager.Release<INpc>(this);
        }

        private void OnDrawGizmos()
        {
            // 目標までの線を描画
            if (_target != null)
            {
                GizmosUtils.Line(transform.position, _target.transform.position, ColorExtensions.ThinWhite);
            }

            // 目標の撃破距離を描画
            if (_params == null) _params = GetComponent<BuddyNpcParams>();
            GizmosUtils.WireCircle(transform.position, _params.DefeatDistance, ColorExtensions.ThinRed);
        }

        /// <summary>
        /// 外部から呼び出して再生。
        /// </summary>
        void INpc.Play()
        {
            View();
            
            if (_target != null)
            {
                DefeatTargetAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else
            {
                MoveForwardAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        // 画面に表示
        private void View()
        {         
            transform.localScale = Vector3.one;
        }

        // 画面から非表示になるが、外部から安全に呼び出せるようにスケールを0にしておく。
        private void Hide()
        {
            transform.localScale = Vector3.zero;
        }

        // 目標を撃破後、そのまま直進する。
        private async UniTaskVoid DefeatTargetAsync(CancellationToken token)
        {
            Transform tgt = _target.transform;
            Transform t = transform;
            // 目標にある程度近づくまで移動。
            Vector3 toTarget = tgt.position - t.position;
            while (toTarget.sqrMagnitude > _params.DefeatSqrDistance)
            {
                if (token.IsCancellationRequested) return;

                // SetActiveで無効化中は動かない。
                if (!gameObject.activeInHierarchy) { await UniTask.Yield(); continue; }

                // 目標がnullになった場合はその向きのまま直進する。
                if (tgt != null) toTarget = tgt.position - t.position;

                // 移動と回転
                t.position += toTarget.normalized * Time.deltaTime * _params.MoveSpeed;
                _rotate.forward = toTarget;

                await UniTask.Yield();
            }

            if (_target != null)
            {
                // 目標にある程度近づいたら1撃で撃破するダメージを与える。
                _target.GetComponent<IDamageable>().Damage(int.MaxValue);
            }

            MoveForwardAsync(token).Forget();
        }

        // 現在向いている方向に直進する。
        private async UniTaskVoid MoveForwardAsync(CancellationToken token)
        {
            Transform t = transform;
            while (true)
            {
                if (token.IsCancellationRequested) return;

                // SetActiveで無効化中は動かない。
                if (!gameObject.activeInHierarchy) { await UniTask.Yield(); continue; }

                // 移動
                t.position += _rotate.forward * Time.deltaTime * _params.MoveSpeed;

                await UniTask.Yield();
            }
        }
    }
}
