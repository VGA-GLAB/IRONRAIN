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
    public class MultiBattleSequenceNpc : MonoBehaviour , INpc
    {
        [Header("撃破目標")]
        [SerializeField] private EnemyController _target;
        [Header("自身への参照とパラメータ")]
        [SerializeField] private Transform _rotate;
        [SerializeField] private float _moveSpeed = 3;
        [Min(1)]
        [SerializeField] private float _defeatDistance = 5;

        EnemyManager.Sequence INpc.Sequence => EnemyManager.Sequence.MultiBattle;

        private void Start()
        {
            // 画面から非表示になるが、外部から安全に呼び出せるようにスケールを0にしておく。
            transform.localScale = Vector3.zero;

            // 自信を登録
            EnemyManager.Register<INpc>(this);
        }

        private void OnDestroy()
        {
            // 自身の登録を解除
            EnemyManager.Release<INpc>(this);
        }

        void INpc.Play()
        {
            // 画面に表示
            transform.localScale = Vector3.one;
            
            if (_target != null)
            {
                DefeatTargetAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
            else
            {
                MoveForwardAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        // 目標を撃破後、そのまま直進する。
        private async UniTaskVoid DefeatTargetAsync(CancellationToken token)
        {
            Transform tgt = _target.transform;
            Transform t = transform;
            // 目標にある程度近づくまで移動。
            Vector3 toTarget = tgt.position - t.position;
            while (toTarget.sqrMagnitude > _defeatDistance * _defeatDistance)
            {
                if (token.IsCancellationRequested) return;

                // SetActiveで無効化中は動かない。
                if (!gameObject.activeInHierarchy) { await UniTask.Yield(); continue; }

                toTarget = tgt.position - t.position;

                // 移動と回転
                t.position += toTarget.normalized * Time.deltaTime * _moveSpeed;
                _rotate.forward = toTarget;

                await UniTask.Yield();
            }

            // 目標にある程度近づいたら1撃で撃破するダメージを与える。
            _target.GetComponent<IDamageable>().Damage(int.MaxValue);

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
                t.position += _rotate.forward * Time.deltaTime * _moveSpeed;

                await UniTask.Yield();
            }
        }

        private void OnDrawGizmos()
        {
            // 目標までの線を描画
            if (_target != null)
            {
                GizmosUtils.Line(transform.position, _target.transform.position, ColorExtensions.ThinWhite);
            }

            // 目標の撃破距離を描画
            GizmosUtils.WireSphere(transform.position, _defeatDistance, ColorExtensions.ThinRed);
        }
    }
}
