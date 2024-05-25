using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//ObjectPool�Ƃ�Object���ʂ�Instantiate�܂���Destroy����ƃp�[�t�H�[�}���X�������邱�Ƃ�h�����߂̃f�U�C���p�^�[��
//�e���Q�[����vampireSurvivors�̂悤�ɑ�ʂ�Instantiate�܂���Destroy����Q�[���ł͕K�{
//�t�ɂ��܂��ʂ�Instantiate�܂���Destroy���Ȃ��Q�[���ł�Object�����炩���ߐ������Ēu���Ă����̂ɂ����������g������ObjectPool���g���K�v�͂Ȃ�

public class ObjectPool
{
    private Dictionary<PoolObjType, GameObject> _insPrefab = new();
    private List<Pool> _pool = new();

    /// <summary>
    /// �����Object����
    /// </summary>
    /// <param name="poolObj"></param>
    /// <param name="insPos"></param>
    /// <param name="insNum"></param>
    public void CreateInitPool(GameObject poolObj, Transform insPos, int insNum, PoolObjType type) 
    {
        if (_insPrefab.ContainsKey(type))
        {

        }
        else 
        {
            _insPrefab.Add(type, poolObj);
        }

        for (int i = 0; i < insNum; i++) 
        {
            GameObject.Instantiate(poolObj, insPos);
            _pool.Add(new Pool(poolObj, type));
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�����W�Ŏg�������Ƃ��ɌĂяo���֐�
    /// </summary>
    /// <param name="position">�I�u�W�F�N�g���X�|�[��������ʒu���w�肷��</param>
    /// <param name="objectType">�I�u�W�F�N�g�̎��</param>
    /// <returns>���������I�u�W�F�N�g</returns>
    public GameObject UseObject(Vector2 position, PoolObjType objectType)
    {
        foreach (var pool in _pool)
        {
            if (pool.Object.activeSelf == false && pool.Type == objectType)
            {
                pool.Object.transform.position = position;
                pool.Object.SetActive(true);
                return pool.Object;
            }
        }

        //�v�[���̒��ɊY������Type��Object���Ȃ������琶������
        var newObj = GameObject.Instantiate(_insPrefab[objectType], Vector3.zero, Quaternion.identity);
        newObj.transform.position = position;
        newObj.SetActive(true);
        _pool.Add(new Pool(newObj, objectType));

        Debug.LogWarning($"{objectType}�̃v�[���̃I�u�W�F�N�g��������Ȃ��������ߐV���ɃI�u�W�F�N�g�𐶐����܂�" +
       $"\n���̃I�u�W�F�N�g�̓v�[���̍ő�l�����Ȃ��\��������܂�" +
       $"����{objectType}�̐���{_pool.FindAll(x => x.Type == objectType).Count}�ł�");

        return newObj;
    }

    /// <summary> �v�[������Obj��ۑ����邽�߂̍\���� </summary>
    public struct Pool
    {
        public GameObject Object;
        public PoolObjType Type;

        public Pool(GameObject g, PoolObjType t)
        {
            Object = g;
            Type = t;
        }
    }
}

public enum PoolObjType 
{
    PlayerBullet,
}