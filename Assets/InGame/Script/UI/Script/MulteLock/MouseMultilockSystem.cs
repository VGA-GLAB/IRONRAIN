using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using IronRain.Player;

public class MouseMultiLockSystem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField, Tooltip("Rayを飛ばす起点")] GameObject _rayOrigin;

    /// <summary>マルチロック中であるか </summary>
    public bool IsMultiLock;

    // 敵のUIリスト 
    private HashSet<GameObject> _lockOnEnemy = new();

    [SerializeField, Tooltip("Rayのレイヤーマスク")] private LayerMask _layerMask;

    [SerializeField, Tooltip("使用するLineRenderer")] private LineRenderer _lineRenderer;

    [SerializeField, Tooltip("Rayの距離")] private float _rayDis = 100f;
    [SerializeField] private PlayerController _playerController;

    // レーダーマップ
    private RaderMap _raderMap;

    // ロックオンしたUI
    private HashSet<GameObject> _lockUi = new();

    private int _posCount;

    private void Awake()
    {
        //レーダーテストを検索する
        _raderMap = FindObjectOfType(typeof(RaderMap)).GetComponent<RaderMap>();
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

    /// <summary>エネミーを探す処理</summary>
    private void SearchEnemy()
    {
        //Rayを飛ばすスタート位置を決める
        var rayStartPosition = _rayOrigin.transform.position;
        var mousePos = Input.mousePosition;
        mousePos.z = 1f;
        //マウスでRayを飛ばす方向を決める
        var worldMousePos = Camera.main.ScreenToWorldPoint(mousePos);
        var direction = (worldMousePos - rayStartPosition).normalized;
        //Hitしたオブジェクト格納用
        RaycastHit hit;
        if (Physics.Raycast(rayStartPosition, direction, out hit, _rayDis, _layerMask))
        {
            if (hit.collider.gameObject.TryGetComponent(out EnemyUi enemyUi))
            {
                if (!_lockOnEnemy.Contains(enemyUi.Enemy))
                {
                    //ターゲットをロックしたときに出す音
                    CriAudioManager.Instance.SE.Play("SE", "SE_Targeting");
                }

                _lockOnEnemy.Add(enemyUi.Enemy);
                _lockUi.Add(enemyUi.gameObject);
            }
        }
    }

    /// <summary>マルチロックのスタート時に呼ばれる</summary>
    public void MultiLockOnStart()
    {
        IsMultiLock = true;
    }

    /// <summary>マルチロックの終了時に呼ばれる</summary>
    private void EndMultiLockAction()
    {
        if (IsMultiLock)
        {
            //プレイヤーを攻撃可能にする
            _playerController.PlayerEnvroment.RemoveState(PlayerStateType.NonAttack);

            if (_lockOnEnemy.Count > 0)
            {
                //LockOnEnemy = LockOnEnemy.Distinct().ToList();
                // _raderMap.MultiLockon(LockOnEnemy);
            }

            IsMultiLock = false;
            _lockOnEnemy.Clear();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //パネルに触れた時の音
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel_Tap");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsMultiLock)
        {
            //多重ロックオン発動時に流れる音
            CriAudioManager.Instance.SE.Play("SE", "SE_Lockon");
            SearchEnemy();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndMultiLockAction();
        _lockUi.Clear();
    }

    public void EnemyDestroy(GameObject enemy)
    {
        _lockOnEnemy.Remove(enemy);
        _lockUi.Remove(enemy);
    }
}