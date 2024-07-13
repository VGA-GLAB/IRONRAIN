using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class StartSceneController : MonoBehaviour
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private Light[] _lights = default;

    [FormerlySerializedAs("_text")] [SerializeField]
    private float _oneCharDuration = 0.05F;
    [SerializeField, TextArea] private string _firstText = "Press Any Button";
    [Header("サイレン")]
    [SerializeField] private float _loopSec = 1.5F;

    [Header("サイレンが鳴ってからの待ち時間")]
    private float _waitSec = 5F;
    [Header("第二テキスト")] [SerializeField, TextArea]
    private string _secondText = "機体に乗れ！";

    [SerializeField] private string _loadSceneName = "ChaseScene";

    private async void Start()
    {
        await StartSceneAsync(this.GetCancellationTokenOnDestroy());
    }

    private async UniTask StartSceneAsync(CancellationToken ct)
    {
        await _textBox.DoOpenTextBoxAsync(1F, ct);

        await _textBox.DoTextChangeAsync(_firstText, _oneCharDuration, ct);

        await UniTask.WaitUntil(() => UnityEngine.InputSystem.Keyboard.current.anyKey.isPressed, cancellationToken: ct);

        await _textBox.DoCloseTextBoxAsync(1F, ct);
        _textBox.ClearText();

        using var sirenCTS = new CancellationTokenSource();
        Siren(sirenCTS.Token);

        CriAudioManager.Instance.SE.Play("SE", "SE_Alert");

        await UniTask.WaitForSeconds(_waitSec, cancellationToken: ct);

        await _textBox.DoOpenTextBoxAsync(1F, ct);

        await _textBox.DoTextChangeAsync(_secondText, _oneCharDuration, ct);

        await UniTask.WaitForSeconds(2F, cancellationToken: ct);

        await _textBox.DoCloseTextBoxAsync(1F, ct);

        sirenCTS.Cancel();

        await Fade(ct);

        await SceneManager.LoadSceneAsync(_loadSceneName);
    }

    private void Siren(CancellationToken ct)
    {
        RenderSettings.ambientIntensity = 0.5F;
        
        foreach (var t in _lights)
        {
            t.color = Color.red;
            DOTween.To(() => 1.5F, x => t.intensity = x, 0F, _loopSec)
                .SetLoops(-1, LoopType.Yoyo)
                .ToUniTask(cancellationToken: ct);
        }
    }

    private async UniTask Fade(CancellationToken ct)
    {
        foreach (var t in _lights)
        {
            t.color = Color.red;
            DOTween.To(() => t.intensity, x => t.intensity = x, 0F, 1F)
                .ToUniTask(cancellationToken: ct);
        }
        
        await DOTween.To(() => RenderSettings.ambientIntensity, x => RenderSettings.ambientIntensity = x, 0F, 1F)
            .ToUniTask(cancellationToken: ct);
    }
}
