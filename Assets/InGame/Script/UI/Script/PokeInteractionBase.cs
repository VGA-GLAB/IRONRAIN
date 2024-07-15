using System.Collections.Generic;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

// デフォルトでは人差し指以外では突いても反応しない？
// 横側から判定に入っても反応しないのでドラッグ操作が難しい。
// InteractableUnityEventWrapperの各種コールバックも判定に横から入った場合はどれも呼ばれない。
// 判定が重なっている場合は貫通しない、通常のUIと同じ。

// パネル本体にのみ判定を持たせ、指先で触れている状態かどうかを判定する。
// パネルに触れている状態の場合、UI上のアイコンの位置と指先の当たり判定を三平方の定理でチェックする。
public class PokeInteractionBase : MonoBehaviour
{
    [SerializeField] InteractableUnityEventWrapper _event;
    [Header("指先がUIに触れた場合に位置を追跡する")]
    [SerializeField] GameObject _cursor;
    [Header("左右どちらかの人差し指")]
    [SerializeField] GameObject _fingertip;
    [Header("指先で触れるUI上のアイコン")]
    [SerializeField] List<GameObject> _icons = new List<GameObject>();

    bool _isHover;
    bool _isSelect;
    bool _isInteractorView;
    bool _isSelectingInteractorView;

    private bool _isCheck = false;
    
    void Start()
    {
        // 状態のフラグ操作をコールバックに登録。
        _event.WhenHover.AddListener(() => _isHover = true);
        _event.WhenUnhover.AddListener(() => _isHover = false);
        _event.WhenSelect.AddListener(() => _isSelect = true);
        _event.WhenUnselect.AddListener(() => _isSelect = false);
        _event.WhenInteractorViewAdded.AddListener(() => _isInteractorView = true);
        _event.WhenInteractorViewRemoved.AddListener(() => _isInteractorView = false);
        _event.WhenSelectingInteractorViewAdded.AddListener(() => _isSelectingInteractorView = true);
        _event.WhenSelectingInteractorViewRemoved.AddListener(() => _isSelectingInteractorView = false);
    }

    void Update()
    {
        // 指先がUIに触れていた場合は反映。
        if (_isSelect)
        {
            FingertipCursor(_fingertip, _cursor);
            CheckIcons(_cursor);
        }
        else
        {
            _isCheck = false;
        }
    }

    // 指先の位置にカーソルを合わせる。
    void FingertipCursor(GameObject fingertip, GameObject cursor)
    {
        Vector3 p = cursor.transform.position;
        p.x = fingertip.transform.position.x;
        p.y = fingertip.transform.position.y;

        cursor.transform.position = p;
    }

    // 指先のカーソルと同じ位置のアイコンを大きめに描画。
    void CheckIcons(GameObject cursor)
    {
        if (_isCheck)
            return;
        
        const float CursorRad = 0.015f;
        const float IconRad = 0.03f;
        
        foreach (GameObject g in _icons)
        {
            // 三平方の定理で当たり判定。
            float dist = Vector3.Magnitude(g.transform.position - cursor.transform.position);
            float r = (CursorRad + IconRad) / 2;
            if (dist <= r)
            {
                //g.transform.localScale = Vector3.one * 1.2f;
                var enemyUi = g.GetComponent<EnemyUi>();
                enemyUi.OnButton();
                _isCheck = true;
                break;
            }
            // else
            // {
            //     g.transform.localScale = Vector3.one;
            // }
        }
    }

    /// <summary>
    /// iconリストに追加する
    /// </summary>
    /// <param name="enemyUi"></param>
    public void AddIcon(GameObject enemyUi)
    {
        _icons.Add(enemyUi);
    }
    
    /// <summary>
    /// iconリストからUiをリストを削除する
    /// </summary>
    /// <param name="enemyUi"></param>
    public void RemoveIcon(GameObject enemyUi)
    {
        _icons.Remove(enemyUi);
    }
}
