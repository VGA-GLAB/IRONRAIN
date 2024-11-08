using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 操作説明パネル（レバーの動かし方などを説明しているパネル）
/// のアイコンを変更します
/// </summary>
public class IndicationPanelController : MonoBehaviour
{
    [SerializeField] private Image _iconArea;
    [SerializeField] private IndicationUiScriptableObject _sprites;
    [SerializeField, Header("点滅間隔")] private float _flashingInterval;
    [SerializeField, Header("点滅時の最低アルファ値")] private float _ｍinAlpha;
    [SerializeField, Header("パネル拡大にかける時間")] private float _expansionTime;
    [SerializeField, Header("パネル縮小にかける時間")] private float _reductionTime;
    private GameObject _canvas;
    private Tween _indicationUITween;

    private void Start()
    {
        _canvas = gameObject.transform.GetChild(0).gameObject;
        gameObject.SetActive(false);
    }
    public void ChangeIndicationUI(IndicationUIType type)
    {
        if (type == IndicationUIType.None)
        {
            _indicationUITween.Kill();
            _indicationUITween = _canvas.transform.DOScale(Vector3.zero, _reductionTime)
                .OnComplete(() => _iconArea.gameObject.SetActive(false));
        }
        else
        {
            _iconArea.gameObject.SetActive(true);
            Sprite sprite = type switch
            {
                IndicationUIType.PushOutsideLever => _sprites.PushOutsideLever,
                IndicationUIType.PullOutsideLever => _sprites.PullOutsideLever,
                IndicationUIType.ControllerTrigger => _sprites.ControllerTrigger,
                IndicationUIType.ControllerWeaponChange => _sprites.ControllerWeaponChange,
                IndicationUIType.ControllerMove => _sprites.ContorllerMove,
                IndicationUIType.PushThrottle => _sprites.PushThrottle,
                IndicationUIType.ThrottleTrigger => _sprites.ThrottleTrigger,
                IndicationUIType.Toggle => _sprites.Toggle,
                _ => null
            };

            _iconArea.sprite = sprite;
            _iconArea.color = new Color(255, 255, 255, 1);

            //パネルの操作（拡大→UIを点滅させる）
            _canvas.transform.localScale = Vector3.zero;
            _canvas.transform.DOScale(Vector3.one, _expansionTime)
                .OnComplete(() => _indicationUITween = _iconArea.DOFade(_ｍinAlpha, _flashingInterval).SetEase(Ease.Flash).SetLoops(-1, LoopType.Yoyo));
        }
    }
}

/// <summary>
/// 操作説明の列挙型
/// </summary>
public enum IndicationUIType
{
    None,
    /// <summary>外側のレバーを押し出す</summary>
    PushOutsideLever,
    /// <summary>外側のレバーを引く</summary>
    PullOutsideLever,
    /// <summary>スティックレバーで攻撃</summary>
    ControllerTrigger,
    /// <summary>スティックレバーで武器変更</summary>
    ControllerWeaponChange,
    /// <summary>スティックレバーを倒す</summary>
    ControllerMove,
    /// <summary>スロットルレバーを押し出す</summary>
    PushThrottle,
    /// <summary>スロットルレバーを引く</summary>
    PullThrottle,
    /// <summary>スロットルレバー背面のボタンを押す</summary>
    ThrottleTrigger,
    /// <summary>トグルスイッチ</summary>
    Toggle
}
