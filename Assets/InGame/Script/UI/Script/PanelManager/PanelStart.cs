using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PanelStart : MonoBehaviour
{
    [SerializeField, Tooltip("スタート時にフェードするパネル")]
    private Image _startPanel;

    [SerializeField, Tooltip("フェードする時間")] private float _time;
    public void StartPanel()
    {
        _startPanel.DOFade(0, _time).OnComplete(() => 
            _startPanel.gameObject.SetActive(false)).SetLink(this.gameObject);
    }
}
