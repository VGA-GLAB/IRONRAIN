using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public sealed class TutorialTextBoxController : MonoBehaviour
{
    [SerializeField] private Text _textUI = default;
    [SerializeField] private Image _textBox = default;

    private void Awake()
    {
        // 初期化
        _textUI.text = "";
        _textUI.enabled = false;
        _textBox.enabled = false;
    }

    public async UniTask DoTextChangeAsync(string text, float duration, CancellationToken cancellationToken, 
        ScrambleMode scrambleMode = ScrambleMode.None)
    {
        await _textUI.DOText(text, duration, scrambleMode: scrambleMode)
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public async UniTask DoOpenTextBoxAsync(float duration, CancellationToken cancellationToken)
    {
        _textUI.enabled = true;
        _textBox.enabled = true;
        
        transform.localScale = Vector3.zero;

        await transform.DOScale(Vector3.one, duration)
            .OnKill(() =>
            {
                if (gameObject)
                {
                    transform.localScale = Vector3.one;
                }
            })
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public async UniTask DoCloseTextBoxAsync(float duration, CancellationToken cancellationToken)
    {
        await transform.DOScale(Vector3.zero, duration)
            .OnKill(() =>
            {
                _textUI.enabled = false;
                _textBox.enabled = false;
                
                if (gameObject)
                {
                    transform.localScale = Vector3.one;
                }
            })
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                _textUI.enabled = false;
                _textBox.enabled = false;
            })
            .ToUniTask(cancellationToken: cancellationToken);
    }

    public void ClearText() => _textUI.text = "";
}
