using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作説明パネル（レバーの動かし方などを説明しているパネル）
/// のアイコンを変更します
/// </summary>
public class IndicationPanelController : MonoBehaviour
{
    [SerializeField, Header("点滅間隔")] private float _flashingInterval;
    [SerializeField, Header("点滅時の最低アルファ値")] private float _ｍinAlpha;
    [SerializeField, Header("パネル拡大にかける時間")] private float _expansionTime;
    [SerializeField, Header("パネル縮小にかける時間")] private float _reductionTime;
    [SerializeField, Header("操作説明のUIを表示するオブジェクト")] private Image _iconArea;
    /// <summary>操作説明のUIを管理しているスクリプタブルオブジェクト</summary>
    [SerializeField] private IndicationUiScriptableObject _UIsprites;
    /// <summary>操作説明パネルのキャンバス</summary>
    private GameObject _indicationPanelCanvas;
    private Tween _indicationUITween;

    private void Start()
    {
        _indicationPanelCanvas = gameObject.transform.GetChild(0).gameObject;
        gameObject.SetActive(false);
    }
    public void ChangeIndicationUI(IndicationUIType type)
    {
        if (type == IndicationUIType.None)
        {
            _indicationUITween.Kill();
            _indicationUITween = _indicationPanelCanvas.transform.DOScale(Vector3.zero, _reductionTime)
                .OnComplete(() => _iconArea.gameObject.SetActive(false));
        }
        else
        {
            _iconArea.gameObject.SetActive(true);
            Sprite sprite = type switch
            {
                IndicationUIType.PushOutsideLever => _UIsprites.PushOutsideLever,
                IndicationUIType.PullOutsideLever => _UIsprites.PullOutsideLever,
                IndicationUIType.ControllerTrigger => _UIsprites.ControllerTrigger,
                IndicationUIType.ControllerWeaponChange => _UIsprites.ControllerWeaponChange,
                IndicationUIType.ControllerMove => _UIsprites.ContorllerMove,
                IndicationUIType.PushThrottle => _UIsprites.PushThrottle,
                IndicationUIType.ThrottleTrigger => _UIsprites.ThrottleTrigger,
                IndicationUIType.Toggle => _UIsprites.Toggle,
                _ => null
            };

            _iconArea.sprite = sprite;
            _iconArea.color = new Color(255, 255, 255, 1);

            //パネルの操作（拡大→UIを点滅させる）
            _indicationPanelCanvas.transform.localScale = Vector3.zero;
            _indicationPanelCanvas.transform.DOScale(Vector3.one, _expansionTime)
                .OnComplete(() => _indicationUITween = _iconArea.DOFade(_ｍinAlpha, _flashingInterval).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo));
        }
    }
}
