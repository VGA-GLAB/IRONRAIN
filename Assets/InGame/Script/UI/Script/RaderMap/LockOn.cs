using System;
using IronRain.Player;
using UnityEngine;

public class LockOn : MonoBehaviour
{
    [SerializeField, Header("音を鳴らす位置")] private Transform _soundTransform;
    private GameObject _nearEnemy;
    
    [SerializeField, Tooltip("視野角の基準点")] private Transform _origin;
    [SerializeField, Tooltip("視野角（度数法）")] private float _sightAngle;
    [SerializeField, Header("ロックオン可能距離")] private float _rockonDistance;
    [SerializeField, Header("アサルトライフルでロックオン可能な範囲")] private float _arLockOnDirection = 0.3f; // ブラッシュアップで追加
    
    [SerializeField] private RadarMap _radarMap;
    [SerializeField] private PlayerWeaponController _playerWeaponController;

    public PlayerWeaponController PlayerWeaponController => _playerWeaponController;
    
    /// <summary>現在ロックされているエネミー</summary>
    public GameObject GetRockEnemy { get; private set; }
    public float NearestEnemy { get; private set; }
    
    /// <summary> 一番近い敵をロックオンする</summary>
    public void NearEnemyLockOn()
    {
        //タッチパネルロックオンの判定
        if (GetRockEnemy != null) //ロックオンしていて、ロックオン状態かつ視野角に収まっていたらreturn
        {
            var agent = GetRockEnemy.GetComponent<AgentScript>();
            if (agent.IsRockOn && IsVisible(agent.gameObject))
                return;
        }

        _radarMap.ResetUI();　//全てのエネミーのロックオンを外す
        
        if (_playerWeaponController.WeaponModel.CurrentWeaponIndex == 0)
        {
            AssaultLockOn();　//装備中の武器がアサルトライフルの場合の処理
            return;
        }
        
        var nearEnemy = NearEnemy(); //敵がいなかったら
        
        if (nearEnemy.obj is null)
        {
            NearestEnemy = float.MaxValue;
            GetRockEnemy = null;
            return;
        }

        TargetIconChange(nearEnemy);
    }
    
    /// <summary>アサルトライフルのロックオン処理</summary>
    public void AssaultLockOn()
    {
        Debug.Log("呼び出された");
        var nearEnemy = NearEnemy(); //アサルトライフルのロックオンで敵を検索
        if (nearEnemy.obj != null) 
        {
            TargetIconChange(nearEnemy);
        }
    }

    /// <summary>
    /// プレイヤーから１番近い敵のゲームオブジェクトを返す
    /// </summary>
    private (GameObject obj, float) NearEnemy()
    {
        _nearEnemy = null;
        float nearDistance = float.MaxValue;

        //エネミーとの距離を判定する
        for (int i = 0; i < _radarMap.Enemies.Count; i++)
        {
            if (!IsVisible(_radarMap.Enemies[i].gameObject)) continue; //視野角内にいなかったら次

            float distance = Vector3.Distance(_radarMap.Enemies[i].transform.position, _radarMap._playerTransform.transform.position);
            if (distance < nearDistance)
            {
                nearDistance = distance;
                _nearEnemy = _radarMap.Enemies[i].gameObject;
            }
        }

        return (_nearEnemy, nearDistance);
    }

    /// <summary>ターゲットが視野角内にいるかを判定する</summary>
    private bool IsVisible(GameObject enemy)
    {
        var selfDir = _origin.forward; //自身の向き
        var targetDir = enemy.transform.position - _origin.position; //ターゲットまでのベクトルと距離
        var targetDis = targetDir.magnitude;

        float cosHalfSight = Mathf.Cos(_sightAngle / 2 * Mathf.Deg2Rad);  //視野角（の半分）の余弦
        float cosTarget = Vector3.Dot(selfDir, targetDir.normalized); // 自身とターゲットへの向きの内積計算

        return _playerWeaponController.WeaponModel.CurrentWeaponIndex == 0 ? cosTarget > _arLockOnDirection : cosTarget > cosHalfSight
            && targetDis < _rockonDistance; //視野角の判定
    }

    /// <summary>Panelを押したときのロックオン処理</summary>
    public void PanelRock(GameObject enemyObject)
    {
        var enemyAgent = enemyObject.GetComponent<AgentScript>();

        if (enemyAgent.IsRockOn)
        {
            _radarMap.ResetUI(); //全てのエネミーのロックオンを外す
        }
        else
        {
            if (!IsVisible(enemyObject)) //視野角内にあるかを判定する
                return;

            _radarMap.ResetUI(); //全てのエネミーのロックオンを外す

            if (!_radarMap._enemyMaps.ContainsKey(enemyAgent.gameObject))
                return;
            enemyAgent.IsRockOn = true;

            var rockonUi = _radarMap._enemyMaps[enemyAgent.gameObject].gameObject.GetComponent<TargetIcon>();
            rockonUi.LockOn();

            GetRockEnemy = enemyAgent.gameObject;
            NearestEnemy = Vector3.Distance(enemyAgent.gameObject.transform.position, _radarMap._playerTransform.transform.position);

            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");　//ターゲットが切り替わる音を出す
            if (_radarMap._isStartTouchPanel) _radarMap._isTouch = true;
        }
    }
    
    /// <summary>ターゲットアイコンを更新する</summary>
    private void TargetIconChange((GameObject obj, float) nearEnemy)
    {
        AgentScript agentScript = nearEnemy.obj.GetComponent<AgentScript>();
        var rockOnUI = _radarMap._enemyMaps[agentScript.gameObject].gameObject.GetComponent<TargetIcon>();
        NearestEnemy = nearEnemy.Item2;
        agentScript.IsRockOn = true;
        rockOnUI.LockOnUI.SetActive(true); //アイコンをロックオン状態にする
        
        if (nearEnemy.obj != GetRockEnemy)　//前のターゲットと違うかを判定
        {
            GetRockEnemy = nearEnemy.obj;
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Targeting");　//ターゲットが切り替わる音を出す
        }
    }

    //視野角をギズモ化
    private void OnDrawGizmos()
    {
        // 視界の範囲（正面及び左右の端）をギズモとして描く
        if (_origin != null)
        {
            Vector3 selfDir = _origin.forward;
            Vector3 rightBorder = Quaternion.Euler(0, _sightAngle / 2, 0) * selfDir; //右端
            Vector3 leftBorder = Quaternion.Euler(0, -1 * _sightAngle / 2, 0) * selfDir; //左端
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_origin.transform.position, selfDir * _rockonDistance);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(_origin.transform.position, rightBorder * _rockonDistance);
            Gizmos.DrawRay(_origin.transform.position, leftBorder * _rockonDistance);
        }
    }
}
