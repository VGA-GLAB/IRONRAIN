using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPSliderView : MonoBehaviour
{
    [SerializeField] private Image _slider;
    
    [Header("通常エネミーの設定")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 10f, -2f);
    [SerializeField] private Vector3 _scale = new Vector3(0.8f, 0.8f, 0.8f);
    
    [Header("ドローンの設定")]
    [SerializeField] private Vector3 _funnelOffset = new Vector3(0, 5f, -2f);
    [SerializeField] private Vector3 _funnelScale = new Vector3(0.4f, 0.4f, 0.4f);

    private BlackBoardWrapper _bbWrapper; // 黒板のラッパークラス
    private int _maxHp;

    /// <summary>
    /// HPスライダーの初期化
    /// </summary>
    /// <param name="bb">ロックオン中の敵の黒板</param>
    /// <param name="maxHp">ロックオン中の敵の最大HP</param>
    public void Initialize(object bb, int maxHp)
    {
        SetIconPositionAndScale(_offset, _scale);

        _bbWrapper = new BlackBoardWrapper(bb);
        _maxHp = maxHp;
        
        // エネミーの種類に応じてオフセットとスケールを変更
        if (bb is Enemy.BlackBoard)
        {
            //　通常エネミーの場合
            SetIconPositionAndScale(_offset, _scale);
        }
        else if (bb is Enemy.Funnel.BlackBoard)
        {
            // ファンネルの場合
            SetIconPositionAndScale(_funnelOffset, _funnelScale);
        }
        
        _slider.fillAmount = CalculateFillAmount(); // スライダーの値を設定
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// アイコンのオフセットとスケールを変更する
    /// </summary>
    private void SetIconPositionAndScale(Vector3 position, Vector3 scale)
    {
        _slider.rectTransform.localPosition = position;
        transform.localScale = scale;
    }

    private void Update()
    {
        // 黒板の参照がない場合かターゲットが死亡しているときは処理を行わない
        if (_bbWrapper == null || !_bbWrapper.IsAlive)
        {
            gameObject.SetActive(false);
            _bbWrapper = null;
            return;
        }

        UpdateFillAmount();
    }
    
    /// <summary>
    /// スライダーの表示更新
    /// </summary>
    private void UpdateFillAmount()
    {
        float fillAmount = CalculateFillAmount();
        _slider.DOFillAmount(fillAmount, 0.05f).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// HP に基づいたスライダーの値を計算
    /// </summary>
    private float CalculateFillAmount()
    {
        float hpRatio = (float)_bbWrapper.Hp / _maxHp;
        return 0.2f + (hpRatio * 0.8f); // 画像に合わせて 0.2 ～ 1.0 にスケール
    }
}
