using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum AnnounceUiType
{
    NoImage,
    Operator,
    Ryan
}

public class AnnounceUiController : MonoBehaviour
{
    //[Header("アナウンスUiオブジェクト")]
    //[SerializeField] private GameObject 
    /// <summary>
    /// アナウンスパネルを切り替える
    /// </summary>
    /// <param name="announceUiType">変更後のアナウンス状態</param>
    public void ChangeAnnounceUi(AnnounceUiType announceUiType)
    {

    }
}