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
            CriAudioManager.Instance.SE.Play("SE", "SE_Lockon");
            SerchEnemy();
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
    }

}
