using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxController : MonoBehaviour
{
    [Header("表示するテキストコンポーネント")]
    [SerializeField] private Text _textBox;

    [Header("表示する文字列")] [TextArea(10, 20),SerializeField] private string _fullText;
    [Header("アニメーションの時間")] private float _typingAnimationTime = 3.0f;

    private void Awake()
    {
        _textBox.text = "";
    }

    public void WriteText()
    {
        // DOTextを使って複数行のタイピングエフェクトを実現
        _textBox.DOText(_fullText, _typingAnimationTime).SetEase(Ease.Linear).SetLink(_textBox.gameObject);
    }
}