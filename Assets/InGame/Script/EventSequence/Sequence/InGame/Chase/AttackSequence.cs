using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;

public sealed class AttackSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerController _controller = default;
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private EnemyManager _enemyManager = default;

    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "左クリックで攻撃";
    [SerializeField] private float _textDuration = 3F;

    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: ct);

        // 攻撃の仕方をオペレーターが喋る
        await Announce(ct);

        // プレイヤーを攻撃可能な状態にする
        _controller.PlayerEnvroment.RemoveState(PlayerStateType.NonAttack);

        // 実際に1機倒す
        await UniTask.WaitUntil(() => _enemyManager.IsAllDefeated(EnemyManager.Sequence.Attack));
    }

    public override void OnSkip()
    {
        // プレイヤーを攻撃可能な状態にする
        _controller.PlayerEnvroment.RemoveState(PlayerStateType.NonAttack);
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
