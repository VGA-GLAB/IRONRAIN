using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;

public class AttackSeqController : MonoBehaviour
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private EnemyManager _enemyManager = default;

    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;

    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    /// <summary>AttackSeqの処理</summary>
    /// <param name="cancellationToken"></param>
    public async UniTask AttackSeqAsync(CancellationToken cancellationToken)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: cancellationToken);

        // 攻撃の仕方をオペレーターが喋る
        await Announce(cancellationToken);

        // 実際に1機倒す
        await UniTask.WaitUntil(() => _enemyManager.IsAllDefeated(EnemyManager.Sequence.Tutorial));
}

    /// <summary>アナウンスとTextBoxの更新を同時に行う関数</summary>
    /// <param name="cancellationToken"></param>
    private async UniTask Announce(CancellationToken cancellationToken)
    {
        await _textBox.DoOpenTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);

        _textBox.ClearText();

        await UniTask.WhenAll(
            //CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
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
