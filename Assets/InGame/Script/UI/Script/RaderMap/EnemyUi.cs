using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyUi : MonoBehaviour, IPointerDownHandler
{
    public GameObject Enemy;
    /// <summary>�\�����郌�[�_�[�}�b�v </summary>
    [NonSerialized] public RaderMap RaderMap;

    private void Awake()
    {
        //���[�_�[�e�X�g����������
        RaderMap = GameObject.Find("RaderTest").GetComponent<RaderMap>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        RaderMap.PanelRock(Enemy);
    }
}
