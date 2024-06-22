using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ToggleButtonSequence : AbstractSequenceBase
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private float _firstDelayTime = 1F;
    
    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;
    
    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    [Header("マルチロックオン")] [SerializeField] private MouseMultilockSystem _mouseMultilockSystem;
    
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        await UniTask.WaitForSeconds(_firstDelayTime, cancellationToken: ct);
        
        await Announce(ct);
    }
    
    /// <summary>アナウンスとTextBoxの更新を同時に行う関数</summary>
    /// <param name="cancellationToken"></param>
    private async UniTask Announce(CancellationToken cancellationToken)
    {
        await _textBox.DoOpenTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);

        await  UniTask.WhenAll(
            //CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
            ChangeText(_tutorialText),
            //トグル４、トグル５を押し、マルチロックをしてください
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), cancellationToken: cancellationToken),
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle5), cancellationToken: cancellationToken)
        );
        //マルチロックフラグがオンになる
        _mouseMultilockSystem.MultilockOnStart();
        
        
        await UniTask.WhenAll(
            //マルチロック処理
            UniTask.WaitUntil(() => _mouseMultilockSystem.IsMultilock == false, cancellationToken: cancellationToken),
            ChangeText("OK") //テキストを変える
        );
        await _textBox.DoCloseTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);
        
        async UniTask ChangeText(string changeText)
        {
            _textBox.ClearText();
            await _textBox.DoTextChangeAsync(changeText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
    
}
