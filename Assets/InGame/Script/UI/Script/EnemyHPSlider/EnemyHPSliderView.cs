using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPSliderView : MonoBehaviour
{
    [SerializeField] private Image _slider;
    
    [Header("通常エネミーの設定")]
    [SerializeField] private Vector3 _offset = new Vector3(0, 10f, -2f); // 通常エネミー用のオフセット
    [SerializeField] private Vector3 _scale = new Vector3(0.8f, 0.8f, 0.8f);
    
    [Header("ドローンの設定")]
    [SerializeField] private Vector3 _funnelOffset = new Vector3(0, 5f, -2f); // ファンネル用のオフセット
    [SerializeField] private Vector3 _funnelScale = new Vector3(0.4f, 0.4f, 0.4f);
    
    private Enemy.BlackBoard _enemyBlackBoard;
    private Enemy.Funnel.BlackBoard _funnelBlackBoard;
    private int _enemyMaxHp;
    private float _fillAmount;

    /// <summary>
    /// 通常エネミーの初期化
    /// </summary>
    /// <param name="bb">ロックオン中の敵の黒板</param>
    /// <param name="maxHp">ロックオン中の敵の最大HP</param>
    public void Initialize(Enemy.BlackBoard bb, int maxHp)
    {
        SetIconPositionAndScale(_offset, _scale);
        
        _enemyBlackBoard = bb;
        _enemyMaxHp = maxHp;
        
        // スライダーの値を変更する
        _slider.fillAmount = (float)_enemyBlackBoard.Hp / _enemyMaxHp;
        
        gameObject.SetActive(true);
    }
    
    /// <summary>
    /// ファンネルの初期化
    /// </summary>
    /// <param name="bb">ロックオン中の敵の黒板</param>
    /// <param name="maxHp">ロックオン中の敵の最大HP</param>
    public void Initialize(Enemy.Funnel.BlackBoard bb, int maxHp)
    {
        SetIconPositionAndScale(_funnelOffset, _funnelScale);
        
        _funnelBlackBoard = bb;
        _enemyMaxHp = maxHp;
        
        // スライダーの値を変更する
        _slider.fillAmount = (float)_funnelBlackBoard.Hp / _enemyMaxHp;
        
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
        if (_enemyBlackBoard != null)
        {
            if (!_enemyBlackBoard.IsAlive) // 敵が死んだタイミングで非表示にして、黒板の参照をやめる
            {
                _enemyBlackBoard = null;
                gameObject.SetActive(false);
                return;
            }
        
            _fillAmount = (float)_enemyBlackBoard.Hp / _enemyMaxHp; // 0.0 ～ 1.0
        }
        else if (_funnelBlackBoard != null)
        {
            if (!_funnelBlackBoard.IsAlive) // 敵が死んだタイミングで非表示にして、黒板の参照をやめる
            {
                _funnelBlackBoard = null;
                gameObject.SetActive(false);
                return;
            }
        
            _fillAmount = (float)_funnelBlackBoard.Hp / _enemyMaxHp; // 0.0 ～ 1.0
        }
        
        _fillAmount = 0.2f + (_fillAmount * 0.8f); // 画像に合わせて 0.2 ～ 1.0 にスケール

        _slider.DOFillAmount(_fillAmount, 0.05f).SetEase(Ease.OutCubic);
    }
}
