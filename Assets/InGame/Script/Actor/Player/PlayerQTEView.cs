using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQTEView : MonoBehaviour
{
    [SerializeField] private Text _qteText;

    public void SetQteStateText(QTEState qteState) 
    {
        if (qteState == QTEState.QTE1)
        {
            _qteText.text = "QTEMode\n右レバーボタン1を押したまま右レバーを引いた状態にする";
        }
        else if (qteState == QTEState.QTE2)
        {
            _qteText.text = "QTEMode\n右レバーボタン1を押したまま右レバーを前に押し出す";
        }
        else if (qteState == QTEState.QTE3) 
        {
            _qteText.text = "QTEMode\n右レバーボタン2を押す";
        }
        else
        {
            _qteText.text = "";
        }
    }
}
