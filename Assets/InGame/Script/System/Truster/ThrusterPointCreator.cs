using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ĂPPoint�����O�ɐ���
/// �ĂQ���E�̃|�C���g���擾����x�ɐ���
/// �ĂR���O�ɔz�u
/// </summary>
public class ThrusterPointContainer : MonoBehaviour
{
    private List<ThrusterPointData> _thrusterPointDataList = new();

    [SerializeField] private Transform _playerObj;
    [SerializeField] private Transform _insPos;
    [SerializeField] private Transform _centerPoint;
    [Header("��������I�u�W�F�N�g")]
    [SerializeField]
    private GameObject _createObject;
    [Header("��������I�u�W�F�N�g�̐�")]
    [SerializeField]
    private int _objCount = 40;
    [Header("���a")]
    [SerializeField]
    private float _radius = 5f;

    [Tooltip("���݂ǂ��̃|�C���g�ɂ��邩")]
    private int _nowPoint = 0;

    void Start()
    {
        InsThrusterPoint();
    }

    /// <summary>
    /// �X���X�^�[�|�C���g�𐶐�����
    /// </summary>
    private void InsThrusterPoint() 
    {
        // sin �̎����� 2��
        var oneCycle = 2.0f * Mathf.PI;
        //���S�_�܂ł̋���
        var distance = (_playerObj.transform.position - _centerPoint.position).sqrMagnitude;

        for (var i = 0; i < _objCount; ++i)
        {
            // �����̈ʒu (1.0 = 100% �̎� 2��)
            var point = ((float)i / _objCount) * oneCycle;

            var x = Mathf.Cos(point) * distance;
            var z = Mathf.Sin(point) * distance;

            var position = new Vector3(x + _insPos.transform.position.x, _playerObj.transform.position.y, z + _insPos.transform.position.z);

            _thrusterPointDataList[i] = new ThrusterPointData(position, i);

            //Instantiate(
            //    _createObject,
            //    position,
            //    Quaternion.identity,
            //    transform
            //);
        }
    }

    /// <summary>
    /// ���̍����̃|�C���g�Ɉړ�����
    /// </summary>
    public ThrusterPointData NextLeftPoint(float distance) 
    {
        _nowPoint++;
        return PointPositionUpdate(distance);
        
    }

    /// <summary>
    /// ���̉E���̃|�C���g�Ɉړ�����
    /// </summary>
    /// <returns></returns>
    public ThrusterPointData NextRightPoint(float distance) 
    {
        _nowPoint--;
        return PointPositionUpdate(distance);
    }

    /// <summary>
    /// �|�C���g�̃|�W�V�������X�V
    /// </summary>
    /// <param name="distance"></param>
    /// <returns></returns>
    private ThrusterPointData PointPositionUpdate(float distance) 
    {
        var pointData = _thrusterPointDataList[_nowPoint];
        var oneCycle = 2.0f * Mathf.PI;
        var point = (pointData.PointNum / _objCount) * oneCycle;
        var x = Mathf.Cos(point) * distance;
        var z = Mathf.Sin(point) * distance;
        var position = new Vector3(x + _insPos.transform.position.x, _playerObj.transform.position.y, z + _insPos.transform.position.z);
        pointData.Position = position;
        return pointData;
    }
}
