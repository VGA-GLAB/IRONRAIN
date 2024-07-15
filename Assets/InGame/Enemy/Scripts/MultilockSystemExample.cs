using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 右手の人差し指でパネルを突く前提。
public class MultilockSystemExample : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper _event;
    [Header("右手の人差し指")]
    [SerializeField] private Transform _fingertip;
    [Header("指先の位置を示すパネル上のカーソル")]
    [SerializeField] private Transform _cursor;
    [Header("生成されたTargetの親")]
    [SerializeField] private Transform _parent;
    [Header("カーソルとTargetの当たり判定の設定")]
    [SerializeField] private float _cursorRadius = 0.015f;
    [SerializeField] private float _targetRadius = 0.03f;

    private bool _isSelect;

    private List<Transform> _targets = new List<Transform>();
    private List<GameObject> _lockOn = new List<GameObject>();
    private HashSet<Transform> _temp = new HashSet<Transform>();

    private void Start()
    {
        // 状態のフラグ操作をコールバックに登録。
        _event.WhenSelect.AddListener(() => _isSelect = true);
        _event.WhenUnselect.AddListener(() => _isSelect = false);
    }

    public async UniTask<List<GameObject>> LockOnAsync(CancellationToken token)
    {
        // パネルを指で突くまで待つ。
        await UniTask.WaitUntil(() => _isSelect, cancellationToken: token);

        // Targetの数は実行中に増減するのでマルチロックする直前にリスト化する。
        AllTargets(_targets, _parent);

        // パネルをなぞっている間に接触したTargetを一時的に保持しておくコレクション。
        _temp.Clear();

        // パネルをなぞっている状態。
        while (_isSelect)
        {
            // カーソルの位置を指先に合わせる。
            FingertipCursor(_fingertip, _cursor);

            // カーソルと接触しているTargetを一時的に保持。
            foreach (Transform t in _targets)
            {
                if (IsCollision(_cursor, t, _cursorRadius, _targetRadius))
                {
                    _temp.Add(t);
                }
            }

            await UniTask.Yield();
        }

        // パネルから指を離したタイミングで、なぞったTargetに対応した敵を返す。
        LockOnEnemies(_temp, _lockOn);

        return _lockOn;
    }

    // 生成されたTargetの親を調べ、Targetのみをリストに詰める。
    private void AllTargets(List<Transform> targets, Transform parent)
    {
        targets.Clear();

        // TargetオブジェクトはEnemyUiスクリプトを持っているのでそれで判定。
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent(out EnemyUi _))
            {
                targets.Add(child);
            }
        }
    }

    // パネル上のカーソルのxy座標を指先に合わせる。
    private void FingertipCursor(Transform fingertip, Transform cursor)
    {
        Vector3 p = cursor.position;
        p.x = fingertip.position.x;
        p.y = fingertip.position.y;

        cursor.position = p;
    }

    // カーソルとTargetがパネル上で接触しているかを判定。
    private bool IsCollision(Transform cursor, Transform target, float cursorRad, float targetRad)
    {
        // 三平方の定理で当たり判定。
        float dist = Vector3.Magnitude(target.position - cursor.position);
        float r = (cursorRad + targetRad) / 2;

        return dist <= r;
    }

    // なぞったTargetに対応した敵をリストに詰める。
    private void LockOnEnemies(HashSet<Transform> temp, List<GameObject> lockOn)
    {
        lockOn.Clear();

        // それぞれのTargetが、対応したEnemyへの参照を持っている。
        foreach (Transform t in temp)
        {
            if (t.TryGetComponent(out EnemyUi ui))
            {
                lockOn.Add(ui.Enemy);
            }
        }
    }
}
