using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using IronRain.Player;

public class MultiLockSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;

    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock;

    /// <summary>敵のUIリスト </summary>
    private HashSet<GameObject> _lockOnEnemy = new();
    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
    [FormerlySerializedAs("_rayDis")] [SerializeField, Tooltip("Rayの距離")] private float _rayDistance = 10f;
    [SerializeField] private PlayerController _playerController;
    [SerializeField, Tooltip("Rayのレイヤーマスク")] LayerMask _layerMask;

    [SerializeField, Tooltip("ドラッグした時に音がなる距離")] private float _dragDistance = 0.1f;
    // 前回のdrag位置
    private Vector3 _preDragPos;
    // レーダーマップ 
    private RaderMap _raderMap;
    /// <summary>ロックオンしたUI </summary>
    private HashSet<GameObject> _lockUI = new();
    private int _posCount;
    private bool _isFirstTouch = true;
    
    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = FindObjectOfType<RaderMap>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsMultilock)
        {
            SerchEnemy();
        }
    }

    private void LateUpdate()
    {
        _posCount = 0;
        _lineRenderer.positionCount = _posCount;
        //ラインレンダラーの更新
        if (_lockUI.Count < 1)
            return;
        _lineRenderer.positionCount = _lockUI.Count;

        foreach (GameObject obj in _lockUI)
        {
            _posCount++;
            _lineRenderer.positionCount = _posCount;
            _lineRenderer.SetPosition(_posCount - 1, obj.transform.position);
        }
    }

    // エネミーを探す処理
    private void SerchEnemy()
    {
        //Rayを飛ばすスタート位置を決める
        var rayStartPosition = _rayOrigin.transform.position;
        //マウスでRayを飛ばす方向を決める
        var direction = _rayOrigin.transform.forward;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, _rayDistance, _layerMask))
        {
            if (_isFirstTouch)
            {
                _isFirstTouch = false;
                //パネルに触れた時の音
                CriAudioManager.Instance.SE.Play("SE", "SE_Panel_Tap");
            }
            
            // マルチロックで移動距離に応じて音を鳴らす
            Vector3 currentDragPosition = hit.transform.position;
            if ((currentDragPosition - _preDragPos).sqrMagnitude > _dragDistance * _dragDistance)
            {
                //多重ロックオン発動時に流れる音
                CriAudioManager.Instance.SE.Play("SE", "SE_Lockon");
                _preDragPos = currentDragPosition;
            }
            
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                //Debug.Log("当たった");
                if (!_lockOnEnemy.Contains(enemyUi.Enemy))
                {
                    //ターゲットをロックしたときに出す音
                    CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
                }
                _lockOnEnemy.Add(enemyUi.Enemy);
                _lockUI.Add(enemyUi.gameObject);
            }
        }
        else
        {
            EndMultiLockAction();
        }
    }

    /// <summary>マルチロックのスタート時に呼ばれる</summary>
    public void MultiLockOnStart()
    {
        IsMultilock = true;
    }
    
    /// <summary>マルチロックの終了時に呼ばれる</summary>
    private void EndMultiLockAction()
    {
        if (IsMultilock)
        {
            //プレイヤーを攻撃可能にする
            _playerController.PlayerEnvroment.RemoveState(PlayerStateType.NonAttack);
            
            if (_lockOnEnemy.Count > 0)
            {
                //LockOnEnemy = LockOnEnemy.Distinct().ToList();
               // _raderMap.MultiLockon(LockOnEnemy);
            }
            IsMultilock = false;
            _lockOnEnemy.Clear();
        }
        _lockUI.Clear();
        _isFirstTouch = true;
    }

    public void EnemyDestroy(GameObject enemy)
    {
        _lockOnEnemy.Remove(enemy);
        _lockUI.Remove(enemy);
    }
}
