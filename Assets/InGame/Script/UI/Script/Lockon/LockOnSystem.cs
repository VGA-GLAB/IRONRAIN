using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// 両手の人差し指でパネルを突く前提。
public class LockOnSystem : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper _event;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private GameObject _center;
    [SerializeField,Header("音を出す場所")] private Transform _soundTransform;
    [SerializeField,Header("マルチロックの敵の最低数")] private int _minMultiLockCount = 2;
    [SerializeField,Header("両手の人差し指")] private Transform[] _fingertip;
    [SerializeField,Header("指先の位置を示すパネル上のカーソル")] private Transform _cursor;
    [SerializeField,Header("生成されたTargetの親")] private Transform _parent;
    [SerializeField,Header("カーソルとTargetの当たり判定の設定")] private float _cursorRadius = 0.015f;
    [SerializeField] private float _targetRadius = 0.03f;
    [SerializeField,Header("中心から離れ過ぎている場合にLineRendererを消す距離")] private float _limitLineRendererDistance = 100;
    [SerializeField, Tooltip("Rayのレイヤーマスク")] LayerMask _layerMask;
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;
    [SerializeField, Tooltip("マウス時のあたり判定")] private float _mouseTargetRadius = 1.0f;
    [SerializeField] private RadarMap _radarMap;
    private bool _isMouseMultiLock;
    private bool _isMultiLock;
    private bool _isButtonSelect;

    private bool _isSelect;

    private List<Transform> _targets = new();
    private List<GameObject> _lockOn = new();
    private HashSet<Transform> _temp = new();

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

    private void Update()
    {
        //マウス用のタッチ処理
        if (Input.GetKeyDown(KeyCode.N) && !_isMouseMultiLock)
        {
            //RadarMap radarMap = FindObjectOfType<RadarMap>();
            GameObject nowLockEnemy = _radarMap.GetRockEnemy;
            // ToDo:GameObjectから直接型指定 
            GameObject lockOnEnemy = null;
            foreach(GameObject enemy in _radarMap.Enemies)
            {
                if(nowLockEnemy != enemy)
                {
                    lockOnEnemy = enemy;
                    break;
                }
            }
            if(lockOnEnemy != null)
            {
                _radarMap.PanelRock(lockOnEnemy);
            }  
        }

        if (!Input.GetKeyDown(KeyCode.M) || !_isMultiLock)
            return;

        if(_isButtonSelect)
        {
            // ボタン操作で全てをロックする
            _isMouseMultiLock = false;
            foreach (Transform targetTransform in _targets)
            {
                CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Lockon");
                _temp.Add(targetTransform);
                TargetIcon enemyUI = targetTransform.GetComponent<TargetIcon>();
                enemyUI.LockOnUI.SetActive(true);
            }
            //ButtonAllLockOn();
        }
        else
        {
            // ボタンでマルチロック判定をオンにする
            _isButtonSelect = true; 
        }
    }

    private void LateUpdate()
    {
        if(_isMultiLock)
        {
            // ラインレンダラーを設定
            _lineRenderer.positionCount = 0;
            // ラインレンダラーの更新
            if (_temp.Count >= 1 && LimitLineRenderer())
            {
                foreach (Transform transform in _temp)
                {
                    _lineRenderer.positionCount++;
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, transform.position);
                }
            }
        }
    }

    // Targetに触れた場合は、それをロックオンする。
    // この処理はマルチロック中も呼び出されているので注意。
    private void Touch()
    {
        // パネルに触れた時の音
        CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Panel_Tap");

        // マルチロック時は処理を行わない
        if (_isMultiLock)
            return;
        
        FingertipCursor(_fingertip, _cursor);
        // Targetの数は実行中に増減するのでロックオンする直前にリスト化する。
        AllTargets(_targets, _parent);

        float minDistance = float.MaxValue;
        TargetIcon minEnemyUi = null;

        foreach (Transform t in _targets)
        {
            // Targetに触れているかチェック
            if (!IsCollision(_cursor, t, _cursorRadius, _targetRadius)) continue;
           
            if (!t.TryGetComponent(out TargetIcon ui)) continue;
            // カーソルに近い物を判定する
            float dis = Vector3.SqrMagnitude(t.position - _cursor.position);
            if (!(dis <= minDistance))
                continue;

            minDistance = dis;
            minEnemyUi = ui;
        }
        
        if(minEnemyUi != null)
        {
            // ターゲット更新
            minEnemyUi.OnButton();
        }
    }
    
    /// <summary>マルチロックが終わるまで待つ。タッチパネルを指でなぞり、触れたUIに対応する敵をロックオンする。</summary>
    public async UniTask<List<GameObject>> MultiLockOnAsync(CancellationToken token)
    {
        //１回目のマルチロックオン時のみ呼び出す
        if(!_isMultiLock)
        {
            _isMultiLock = true;
            // パネルを指で突くまで待つ。
            await UniTask.WaitUntil(() => _isSelect || _isButtonSelect, cancellationToken: token);
            _isButtonSelect = true;
            // Targetの数は実行中に増減するのでマルチロックする直前にリスト化する。
            AllTargets(_targets, _parent);
            // パネルをなぞっている間に接触したTargetを一時的に保持しておくコレクション。
            _temp.Clear();
            // 線をリセット。
            _lineRenderer.positionCount = 0;
            _isMouseMultiLock = true;
        }

        while (_isSelect)
        {
            await UniTask.Yield();
            // カーソルの位置を指先に合わせる。
            FingertipCursor(_fingertip, _cursor);
            Transform minDisTarget = null;
            float minDistance = float.MaxValue;
            // カーソルと接触しているTargetを一時的に保持。
            foreach (Transform t in _targets)
            {
                if (!IsCollision(_cursor, t, _cursorRadius, _targetRadius))
                    continue;
                
                if (_temp.Contains(t))
                    continue;
                
                //カーソルに近い物を判定する
                float dis = Vector3.SqrMagnitude(t.position - _cursor.position);
                if (!(dis <= minDistance))
                    continue;

                minDistance = dis;
                minDisTarget = t;
            }

            //ターゲットを追加する
            if (minDisTarget != null)
            {
                _temp.Add(minDisTarget);
                var enemyUi = minDisTarget.GetComponent<TargetIcon>();
                enemyUi.LockOnUI.SetActive(true);
                CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Lockon");
            }
        }
        
        await UniTask.Yield();

        // _tempがより少ない場合に再帰的にMultiLockOnAsyncを呼び出す
        if (_temp.Count < _targets.Count && !token.IsCancellationRequested)
        {
            // ビジーウェイティングを避けるために、ここでディレイを導入することを検討
            await UniTask.Delay(500, cancellationToken: token);
            // 再度ロックオンのロジックを実行
            await MultiLockOnAsync(token);
        }

        // パネルから指を離したタイミングで、なぞったTargetに対応した敵を返す。
        _isButtonSelect = false;
        LockOnEnemies(_temp, _lockOn);
        
        return _lockOn;
    }

    /// <summary>LineRendererをリセットする</summary>
    public void FinishMultiLock()
    {
        _isMultiLock = false;
        _lineRenderer.positionCount = 0;
        _temp.Clear();
    }

    // 生成されたTargetの親を調べ、Targetのみをリストに詰める。
    private void AllTargets(List<Transform> targets, Transform parent)
    {
        targets.Clear();

        // TargetオブジェクトはEnemyUiスクリプトを持っているのでそれで判定。
        foreach (Transform child in parent)
        {
            if (child.TryGetComponent(out TargetIcon enemy))
            {
                enemy.LockOnUI.SetActive(false);
                targets.Add(enemy.gameObject.transform);
            }
        }
    }

    // パネル上のカーソルのxy座標を指先に合わせる。
    private void FingertipCursor(Transform[] fingertip, Transform cursor)
    {
        cursor.position = Vector3.SqrMagnitude(fingertip[0].position - cursor.position) <
                          Vector3.SqrMagnitude(fingertip[1].position - cursor.position) 
            ? fingertip[0].position : fingertip[1].position;
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
            if (t.TryGetComponent(out TargetIcon ui))
            {
                lockOn.Add(ui.Enemy);
            }
        }
    }

    // マルチロック時のLineRendererの表示非表示の切り替え
    private bool LimitLineRenderer()
    {
        foreach(Transform t in _temp)
        {
            Vector3 uiDir = t.transform.position;
            //敵の高さとプレイヤーの高さを合わせる
            uiDir.z = _center.transform.position.y;
            uiDir = t.transform.position - _center.transform.position;
            float distance = uiDir.magnitude;
            
            if (distance > _limitLineRendererDistance)
                return false;
        }

        return true;
    }

    //ボタン操作で全てをロックする関数
    // private void ButtonAllLockOn()
    // {
    //     _isMouseMultiLock = false;
    //     foreach (Transform targetTransform in _targets)
    //     {
    //         CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Lockon");
    //         _temp.Add(targetTransform);
    //         EnemyUi enemyUI = targetTransform.GetComponent<EnemyUi>();
    //         enemyUI.LockOnUi.SetActive(true);
    //     }
    // }
}
// private void OnButtonSelectFlag()
// {
//     _isButtonSelect = true; 
// }

// private IEnumerator LockOnCoroutine(int count)
// {
//     for (int i = 0; i < count; i++)
//     {
//         yield return new WaitForSeconds(0.1f);
//         CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_LockOn");
//     }
// }
    
// private async UniTaskVoid M()
// {
//     var r = await MultiLockOnAsync(this.GetCancellationTokenOnDestroy());
// }