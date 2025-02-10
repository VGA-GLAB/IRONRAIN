using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LockOnIconView : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private Sprite _lockOnIcon; // ロックオンアイコンのスプライト
    [SerializeField] private Image _icon;
    private Transform _target; //ターゲットのTranscorm
    
    private void Start()
    {
        _icon.sprite = _lockOnIcon;
        Debug.Log($"{gameObject.name} 初期化完了");
    }
    
    private void Update()
    {
        if (_target != null)
        {
            SetTargetPosition();
        }
    }
    
    /// <summary>
    /// ターゲットを設定する
    /// </summary>
    public void SetTarget(Transform target)
    {
        _target = target;
        SetTargetPosition();
        Show();
    }

    /// <summary>
    /// 位置を変更する
    /// </summary>
    public void SetTargetPosition()
    {
        if (_target == null) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(_target.position);
        if (screenPos.z > 0) // カメラの後ろにいる場合は無視
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(_canvasRect, screenPos, Camera.main, out Vector3 worldPos);
            transform.position = worldPos;
        }
        else
        {
            Hide();
        }
    }
    
    /// <summary>
    /// ロックオンアイコンを表示する
    /// </summary>
    private void Show()
    {
        _icon.gameObject.SetActive(true);
    }

    /// <summary>
    /// ロックオンアイコンを非表示にする
    /// </summary>
    public void Hide()
    {
        _icon.gameObject.SetActive(false);
    }
}
