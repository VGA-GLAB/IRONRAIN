using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class FirstAnnounceSequence : AbstractSequenceBase
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private PlayerController _playerController = null;
    [Header("最初の待ち時間")]
    [SerializeField] private float _firstAwaitTimeSec = 10F;
    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;

    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;
    
    public override async UniTask PlaySequenceAsync(CancellationToken ct)
    {// 最初に何秒か待つ
        await UniTask.WaitForSeconds(_firstAwaitTimeSec, cancellationToken: ct);
        
        await Announce(ct);
    }
    
    /// <summary>アナウンスとTextBoxの更新を同時に行う関数</summary>
    /// <param name="cancellationToken"></param>
    private async UniTask Announce(CancellationToken cancellationToken)
    {
        await _textBox.DoOpenTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);

        await UniTask.WhenAll(
            //CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
            ChangeText()
        );
        
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle1)),
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle2))
        );
        
        await _textBox.DoCloseTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);
        _textBox.ClearText();
        
        async UniTask ChangeText()
        {
            await _textBox.DoTextChangeAsync(_tutorialText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
}
