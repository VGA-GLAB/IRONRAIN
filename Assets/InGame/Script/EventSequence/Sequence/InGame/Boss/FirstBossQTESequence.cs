using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Enemy.Control;
using Enemy.Control.Boss;

public sealed class FirstBossQTESequence : AbstractSequenceBase
{
    [SerializeField] private EnemyManager _enemyManager;
    [SerializeField] private BossController _bossController;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "QTE1終了";
    [SerializeField] private float _textDuration = 2F;
    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        _enemyManager.BossFirstQte();

        var qteModel = _playerController.SeachState<PlayerQTE>().QTEModel;

        await qteModel.StartQTE(Guid.Empty, QteType.BossQte1);
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

        await _textBox.DoCloseTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);
        _textBox.ClearText();

        async UniTask ChangeText()
        {
            await _textBox.DoTextChangeAsync(_tutorialText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
}
