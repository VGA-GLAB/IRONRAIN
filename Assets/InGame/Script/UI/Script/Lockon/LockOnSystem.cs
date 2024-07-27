using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

// 両手の人差し指でパネルを突く前提。
public class LockOnSystem : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper _event;
    [SerializeField] private LineRenderer _lineRenderer;
    [Header("両手の人差し指")]
    [SerializeField] private Transform[] _fingertip;
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
        // 線を非表示にする。
        _lineRenderer.positionCount = 0;

        // 状態のフラグ操作をコールバックに登録。
        _event.WhenSelect.AddListener(() => _isSelect = true);
        _event.WhenUnselect.AddListener(() => _isSelect = false);

        // タッチパネルに触れて対象をロックオン。
        _event.WhenSelect.AddListener(Touch);
    }

    private async UniTaskVoid M()
    {
        Debug.Log("マルチロックオン開始");
        var r = await MultiLockOnAsync(this.GetCancellationTokenOnDestroy());
        foreach (var v in r)
        {
            Debug.Log(v.name + "をマルチロックオンした");
        }
    }

    // Targetに触れた場合は、それをロックオンする。
    // この処理はマルチロック中も呼び出されているので注意。
    private void Touch()
    {
        FingertipCursor(_fingertip, _cursor);
        
        // Targetの数は実行中に増減するのでロックオンする直前にリスト化する。
        AllTargets(_targets, _parent);

        foreach (Transform t in _targets)
        {
            if (t.TryGetComponent(out EnemyUi u)) Debug.Log(u.Enemy.name + "が候補");
        }

        foreach (Transform t in _targets)
        {
            // Targetに触れているかチェック
            if (!IsCollision(_cursor, t, _cursorRadius, _targetRadius)) continue;
            // 一応Targetのコンポーネントが付いているかチェック
            if (!t.TryGetComponent(out EnemyUi ui)) continue;
            
            // ターゲット更新
            ui.OnButton();
            Debug.Log(ui.Enemy.name + "をロックオン");
            break;
        }
    }

    // なぞった結果、n体以上lock-onできなかった場合はやり直しの処理が無い。
    /// <summary>
    /// マルチロックが終わるまで待つ。
    /// タッチパネルを指でなぞり、触れたUIに対応する敵をロックオンする。
    /// </summary>
    /// <returns>ロックオンした敵のオブジェクト一覧</returns>
    public async UniTask<List<GameObject>> MultiLockOnAsync(CancellationToken token)
    {
        // パネルを指で突くまで待つ。
        await UniTask.WaitUntil(() => _isSelect, cancellationToken: token);

        // Targetの数は実行中に増減するのでマルチロックする直前にリスト化する。
        AllTargets(_targets, _parent);

        // パネルをなぞっている間に接触したTargetを一時的に保持しておくコレクション。
        _temp.Clear();

        // 線をリセット。
        _lineRenderer.positionCount = 0;

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
                    if (_temp.Add(t))
                    {
                        // 新しく追加した場合は、Target同士を結ぶ線を引く。
                        _lineRenderer.positionCount++;
                        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, t.position);
                    }
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
    private void FingertipCursor(Transform[] fingertip, Transform cursor)
    {
        //Vector3 p = cursor.position;
        //p.x = fingertip.position.x;
        //p.y = fingertip.position.y;

        //cursor.position = p;
        
        //カーソルに近い方の指の位置を登録する
        Transform fingerPostion;
        if (Vector3.SqrMagnitude(fingertip[0].position - cursor.position) <
            Vector3.SqrMagnitude(fingertip[1].position - cursor.position))
            fingerPostion = fingertip[0];
        else
            fingerPostion = fingertip[1];
        
        cursor.position = fingerPostion.position;
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