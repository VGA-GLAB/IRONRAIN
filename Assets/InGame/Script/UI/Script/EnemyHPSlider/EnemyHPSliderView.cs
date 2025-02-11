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
        
    }

    private void Update()
    {
        _slider.fillAmount = (float)_bb.Hp / _enemyMaxHp;
        Debug.Log(_bb?.Hp);
    }
}
