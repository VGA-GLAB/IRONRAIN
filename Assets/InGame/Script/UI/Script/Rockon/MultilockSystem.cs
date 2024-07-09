using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MultilockSystem : MonoBehaviour
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;

    /// <summary>マルチロック中であるか </summary>
    public bool IsMultilock;

    /// <summary>敵のUIリスト </summary>
    private HashSet<GameObject> LockOnEnemy = new HashSet<GameObject>();
    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;
    [SerializeField, Tooltip("Rayの距離")] private float _rayDis = 10f;
    [SerializeField] private PlayerController _playerController;
    [SerializeField, Tooltip("Rayのレイヤーマスク")]
    LayerMask _layerMask;

    /// <summary>レーダーマップ </summary>
    private RaderMap _raderMap;
    /// <summary>ロックオンしたUi </summary>
    private HashSet<GameObject> _lockUi;
    private int _posCount;
    private bool _isFirstTouch = true;
    
    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = FindObjectOfType<RaderMap>();
    }

    private void Start()
    {
        // InputProvider.Instance.SetEnterInput(InputProvider.InputType.LeftTrigger, MultilockOnStart);
        // InputProvider.Instance.SetExitInput(InputProvider.InputType.LeftTrigger, MultilockAction);
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsMultilock)
        {
            //多重ロックオン発動時に流れる音
            CriAudioManager.Instance.SE.Play("SE", "SE_Lockon");
            SerchEnemy();
        }
    }

    private void LateUpdate()
    {
        _posCount = 0;
        _lineRenderer.positionCount = _posCount;
        //ラインレンダラーの更新
        if (_lockUi.Count < 1)
            return;
        _lineRenderer.positionCount = _lockUi.Count;

        foreach (GameObject obj in _lockUi)
        {
            _posCount++;
            _lineRenderer.positionCount = _posCount;
            _lineRenderer.SetPosition(_posCount - 1, obj.transform.position);
        }
    }

    /// <summary>
    /// エネミーを探す処理
    /// </summary>
    private void SerchEnemy()
    {
        //Rayを飛ばすスタート位置を決める
        var rayStartPosition = _rayOrigin.transform.position;
        //マウスでRayを飛ばす方向を決める
        var direction = _rayOrigin.transform.forward;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, _rayDis, _layerMask))
        {
            if (_isFirstTouch)
            {
                _isFirstTouch = false;
                //パネルに触れた時の音
                CriAudioManager.Instance.SE.Play("SE", "SE_Panel_Tap");
            }
            
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                //Debug.Log("当たった");
                if (!LockOnEnemy.Contains(enemyUi.Enemy))
                {
                    //ターゲットをロックしたときに出す音
                    CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
                }
                LockOnEnemy.Add(enemyUi.Enemy);
                _lockUi.Add(enemyUi.gameObject);
            }
        }
        else
        {
            EndMultilockAction();
        }

        Debug.DrawRay(rayStartPosition, direction, Color.blue);
    }

    /// <summary>
    /// マルチロックのスタート時に呼ばれる
    /// </summary>
    public void MultilockOnStart()
    {
        IsMultilock = true;
    }
    
    /// <summary>
    /// マルチロックの終了時に呼ばれる
    /// </summary>
    private void EndMultilockAction()
    {
        if (IsMultilock)
        {
            //プレイヤーを攻撃可能にする
            _playerController.PlayerEnvroment.RemoveState(PlayerStateType.NonAttack);
            
            if (LockOnEnemy.Count > 0)
            {
                //LockOnEnemy = LockOnEnemy.Distinct().ToList();
                _raderMap.MultiLockon(LockOnEnemy);
            }
            IsMultilock = false;
            LockOnEnemy.Clear();
        }
        _lockUi.Clear();
        _isFirstTouch = true;
    }

    public void EnemyDestory(GameObject enemy)
    {
        LockOnEnemy.Remove(enemy);
        _lockUi.Remove(enemy);
    }
}
