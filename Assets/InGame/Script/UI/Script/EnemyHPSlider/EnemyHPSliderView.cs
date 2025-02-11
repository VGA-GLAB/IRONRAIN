using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPSliderView : MonoBehaviour
{
    [SerializeField] private Image _slider;
    private Enemy.BlackBoard _bb;
    private int _enemyMaxHp;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="bb">ロックオン中の敵の黒板</param>
    /// <param name="maxHp">ロックオン中の敵の最大HP</param>
    public void Initialize(Enemy.BlackBoard bb, int maxHp)
    {
        _bb = bb;
        _enemyMaxHp = maxHp;
        
        // スライダーの値を変更する
        _slider.fillAmount = (float)_bb.Hp / _enemyMaxHp;
        
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if(_bb == null) return; // 黒板がnullなら処理を行わない

        if (!_bb.IsAlive) // 敵が死んだタイミングで非表示にして、黒板の参照をやめる
        {
            _bb = null;
            gameObject.SetActive(false);
            return;
        }
        
        float fillAmount = (float)_bb.Hp / _enemyMaxHp; // 0.0 ～ 1.0
        _slider.fillAmount = 0.2f + (fillAmount * 0.8f); // 画像に合わせて 0.2 ～ 1.0 にスケール
    }
}
