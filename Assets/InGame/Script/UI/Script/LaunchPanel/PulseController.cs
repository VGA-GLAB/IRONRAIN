using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
public class PulseController : MonoBehaviour
{
    [Header("スクロールさせたいImage")] [SerializeField]
    private RectTransform[] _images;

    [Header("スクロール速度")] [SerializeField] private float _scrollSpeed = 10f;
    [Header("スタート位置")] [SerializeField] private float _startPositionX;
    [Header("リセット位置")] [SerializeField] private float _resetPositionX;
    private bool _isStart;

    private LaunchManager _launchManager;

    private void Awake()
    {
        _launchManager = GameObject.FindObjectOfType<LaunchManager>();
        if (_launchManager != null)
        {
            _launchManager.SkipLaunchUIEvent += SkipAction;
        }
    }

    private void Update()
    {
        if (!_isStart)
            return;
        
        foreach (RectTransform image in _images)
        {
            // 現在の位置を取得
            Vector2 position = image.anchoredPosition;

            // Y座標をスクロール速度に基づいて更新
            position.x -= _scrollSpeed * Time.deltaTime;

            // リセット位置に達したらスタート位置に戻す
            if (position.x <= _resetPositionX)
            {
                position.x = _startPositionX;
            }

            // 位置を更新
            image.anchoredPosition = position;
        }
    }

    public void StartScroll()
    {
        _isStart = true;
    }

    private void SkipAction()
    {
        StartScroll();
    }

    private void OnDisable()
    {
        _launchManager.SkipLaunchUIEvent -= SkipAction;
        _isStart = false;
    }
}