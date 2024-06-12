using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using TMPro;

public class QTESeqController : MonoBehaviour
{
    [SerializeField] private EnemyManager _enemyManager = default;
    [SerializeField] private PlayerQTEModel _playerQTEModel = default;
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [Header("QTEを開始するまでの待ち時間")]
    [SerializeField] private float _qteAwaitTimeSec = 5F;

    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;

    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    /// <summary>QTESeqの処理</summary>
    /// <param name="cancellationToken"></param>
    public async UniTask QTESeqAsync(CancellationToken cancellationToken)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: cancellationToken);

        // 盾持ちを出現させる
        _enemyManager.DetectPlayer(EnemyManager.Sequence.Tutorial);

        // 最初の待ち時間
        await UniTask.WaitForSeconds(_qteAwaitTimeSec, cancellationToken: cancellationToken);

        // QTEを開始させる
        _playerQTEModel.StartQTE();

        // QTEの終了（成功）を待つ
    }

    /// <summary>アナウンスとTextBoxの更新を同時に行う関数</summary>
    /// <param name="cancellationToken"></param>
    private async UniTask Announce(CancellationToken cancellationToken)
    {
        await _textBox.DoOpenTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);

        await UniTask.WhenAll(
            CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
            ChangeText()
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
