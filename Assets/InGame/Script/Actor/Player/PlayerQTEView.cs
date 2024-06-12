using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerQTEView : MonoBehaviour
{
    [SerializeField] private Text _qteText;

    public void SetQteStateText(QTEState qteState) 
    {
        Debug.Log("QTE����");
        if (qteState == QTEState.QTE1)
        {
            _qteText.text = "QTEMode\n�E���o�[�{�^��1���������܂܉E���o�[����������Ԃɂ���";
        }
        else if (qteState == QTEState.QTE2)
        {
            _qteText.text = "QTEMode\n�E���o�[�{�^��1���������܂܉E���o�[��O�ɉ����o��";
        }
        else if (qteState == QTEState.QTE3) 
        {
            _qteText.text = "QTEMode\n�E���o�[�{�^��2������";
        }
        else
        {
            _qteText.text = "";
        }
    }
}
