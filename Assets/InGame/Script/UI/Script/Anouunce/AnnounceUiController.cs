﻿using System;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public enum AnnounceUiType
{
    NoSingal,
    Operator,
    Ryan
}

public class AnnounceUiController : MonoBehaviour
{
    [Header("音を鳴らす位置")]
    [SerializeField] private Transform _soundTransform;
    [Header("バックグラウンドオブジェクト")] 
    [SerializeField] private Image _backGroundObject;
    [Header("フレームオブジェクト")] 
    [SerializeField] private Image _frameObject;
    [Header("キャラクターオブジェクト")] 
    [SerializeField] private Image _characterObject;

    [Header("つながっている時の背景")] 
    [SerializeField] private Sprite _connectingBackGroundSprite;
    [Header("つながっていない時の背景")] 
    [SerializeField] private Sprite _nosignalBackGroundSprite;

    [Header("オペレーターフレーム")] 
    [SerializeField] private Sprite _operatorFrameSprite;
    [Header("ライアンフレーム")] 
    [SerializeField] private Sprite _ryanFrameSprite;
    [Header("NoSignalフレーム")]
    [SerializeField] private Sprite _noSignalFrameSprite;
    [Header("繋げている時のフレーム")]
    [SerializeField] private Sprite _connectingFrameSprite;
    
    [Header("オペレーダーUi")]
    [SerializeField] private Sprite _operatorCharacterSprite;
    [Header("ライアンUi")]
    [SerializeField] private Sprite _ryanUiCharacterSprite;
    [Header("ReceptionUi")]
    [SerializeField] private Sprite[] _receptionSprite;

    [Header("receptionの間隔")] 
    [SerializeField] private float _receptionInterval;

    private AnnounceUiType _preAnnounceUiType;

    private void Start()
    {
        //NoSignalにする
        _backGroundObject.sprite = _nosignalBackGroundSprite;
        _frameObject.sprite = _noSignalFrameSprite;
        _characterObject.sprite = null;
        Color color = _characterObject.color;
        color.a = 0;
        _characterObject.color = color;
        _preAnnounceUiType = AnnounceUiType.NoSingal;
    }

    
    /// <summary>
    /// アナウンスパネルを切り替える
    /// </summary>
    /// <param name="announceUiType">変更後のアナウンス状態</param>
    public async UniTask ChangeAnnounceUi(AnnounceUiType announceUiType, CancellationToken cancellationToken = default)
    {
        //前兆音を鳴らす
        //CriAudioManager.Instance.SE.Play("SE", "SE_Panel");
        if(_preAnnounceUiType == AnnounceUiType.NoSingal && (announceUiType == AnnounceUiType.Operator || announceUiType == AnnounceUiType.Ryan))
        {
            Color color = _characterObject.color;
            color.a = 1;
            _characterObject.color = color;
            _backGroundObject.sprite = _connectingBackGroundSprite;
            _frameObject.sprite = _connectingFrameSprite;
            await AnimationConnecting(cancellationToken);
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Panel");
        }
        
        switch(announceUiType)
        {
            case AnnounceUiType.NoSingal:
                _backGroundObject.sprite = _nosignalBackGroundSprite;
                _frameObject.sprite = _noSignalFrameSprite;
                _characterObject.sprite = null;
                Color color = _characterObject.color;
                color.a = 0;
                _characterObject.color = color;
                break;
            case AnnounceUiType.Operator:
                _backGroundObject.sprite = _connectingBackGroundSprite;
                _frameObject.sprite = _operatorFrameSprite;
                _characterObject.sprite = _operatorCharacterSprite;
                break ;
            case AnnounceUiType.Ryan:
                _backGroundObject.sprite = _connectingBackGroundSprite;
                _frameObject.sprite = _ryanFrameSprite;
                _characterObject.sprite = _ryanUiCharacterSprite;
                break;
        }
        _preAnnounceUiType = announceUiType;
    }

    /// <summary>
    /// 接続中のアニメーション
    /// </summary>
    /// <returns></returns>
    private async UniTask AnimationConnecting(CancellationToken cancellationToken)
    {
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < _receptionSprite.Length; j++)
            {
                _characterObject.sprite = _receptionSprite[j];

                await UniTask.Delay(TimeSpan.FromSeconds(_receptionInterval), cancellationToken: cancellationToken);
            }
        }
    }
    //////テスト用/////

    [ContextMenu("Ryan")]
    public void TestRyan()
    {
        ChangeAnnounceUi(AnnounceUiType.Ryan).Forget();
    }

    [ContextMenu("NoImage")]
    public void TestNoImage()
    {
        ChangeAnnounceUi(AnnounceUiType.NoSingal).Forget();
    }

    [ContextMenu("Operator")]
    public void TestOperator()
    {
        ChangeAnnounceUi(AnnounceUiType.Operator).Forget();
    }
}