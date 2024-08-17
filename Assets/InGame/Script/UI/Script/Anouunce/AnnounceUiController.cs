using System;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public enum AnnounceUiType
{
    NoImage,
    Operator,
    Ryan
}

public class AnnounceUiController : MonoBehaviour
{
    [Header("オペレーダーUi")]
    [SerializeField] private GameObject _operatorUi;
    [Header("ライアンUi")]
    [SerializeField] private GameObject _ryanUi;
    [Header("NoImageUi")]
    [SerializeField] private GameObject _noImageUi;

    [Header("音が鳴ってからパネルが切り替わるまでの時間")] 
    [SerializeField] private double _uiChangeInterval;
    /// <summary>
    /// アナウンスパネルを切り替える
    /// </summary>
    /// <param name="announceUiType">変更後のアナウンス状態</param>
    public async UniTaskVoid ChangeAnnounceUi(AnnounceUiType announceUiType)
    {
        //前兆音を鳴らす
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel");
        
        //数秒待つ
        await UniTask.Delay(TimeSpan.FromSeconds(_uiChangeInterval));
        
        switch(announceUiType)
        {
            case AnnounceUiType.NoImage:
                _noImageUi.transform.SetAsLastSibling();
                break;
            case AnnounceUiType.Operator:
                _operatorUi.transform.SetAsLastSibling();
                break ;
            case AnnounceUiType.Ryan:
                _ryanUi.transform.SetAsLastSibling();
                break;
        }
    }

    //////テスト用/////


    public void TestRyan()
    {
        ChangeAnnounceUi(AnnounceUiType.Ryan);
    }

    public void TestNoImage()
    {
        ChangeAnnounceUi(AnnounceUiType.NoImage);
    }

    public void TestOperator()
    {
        ChangeAnnounceUi(AnnounceUiType.Operator);
    }
}