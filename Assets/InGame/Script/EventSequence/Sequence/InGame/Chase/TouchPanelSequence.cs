using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

public sealed class TouchPanelSequence : AbstractSequenceBase
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private EnemyManager _enemyManager = default;
    [SerializeField] private float _shootWaitSec = 2F;
    [SerializeField] private PlayerController _playerController = null;

    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;
    
    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;
    
    [Header("パネルチュートリアルを待つ時間")]
    [SerializeField] private float _waitTime = 5f;
    public async override UniTask PlaySequenceAsync(CancellationToken ct)
    {
        // 最初の待ち時間
        await UniTask.WaitForSeconds(2F, cancellationToken: ct);

        //プレイヤーの攻撃、移動不可にする
        _playerController.PlayerEnvroment.AddState(PlayerStateType.Inoperable);
        //敵を複数体出現させる
        _enemyManager.DetectPlayer(EnemyManager.Sequence.TouchPanel);
        _enemyManager.Pause(EnemyManager.Sequence.TouchPanel);
        
        //タッチパネルの操作をオペレーターがしゃべる
        await Announce(ct);
        
        //一定時間待つ
        await UniTask.WaitForSeconds(_waitTime, cancellationToken: ct);

        //プレイヤーを動けるようにする
        _playerController.PlayerEnvroment.RemoveState(PlayerStateType.Inoperable);
        //敵を消して終了
        _enemyManager.Resume(EnemyManager.Sequence.TouchPanel);
        _enemyManager.DefeatThemAll(EnemyManager.Sequence.TouchPanel);
    }
    
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
