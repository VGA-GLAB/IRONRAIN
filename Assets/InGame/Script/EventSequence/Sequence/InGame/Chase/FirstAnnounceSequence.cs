using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

public sealed class FirstAnnounceSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerStoryEvent _playerStoryEvent = default;
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private EnemyController _tutorialEnemy = default;
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
    [Header("弾を打つまでの待ち時間")]
    [SerializeField] private float _shootWaitSec = 2F;

    public override void OnSkip()
    {
        _playerStoryEvent.StartChaseScene();
        _playerController.PlayerEnvroment.AddState(PlayerStateType.NonAttack);
    }

    public override async UniTask PlaySequenceAsync(CancellationToken ct)
    {// 最初に何秒か待つ
        await UniTask.WaitForSeconds(_firstAwaitTimeSec, cancellationToken: ct);
        
        await UniTask.WaitForSeconds(2F, cancellationToken: ct);
        
        await Announce(ct);
    }
    
    /// <summary>アナウンスとTextBoxの更新を同時に行う関数</summary>
    /// <param name="cancellationToken"></param>
    private async UniTask Announce(CancellationToken cancellationToken)
    {
        await UniTask.WaitUntil(() => _tutorialEnemy.BlackBoard.IsApproachCompleted, cancellationToken: cancellationToken);
        _playerStoryEvent.StartChaseScene();
        _playerController.PlayerEnvroment.AddState(PlayerStateType.Inoperable);
        _playerController.PlayerEnvroment.AddState(PlayerStateType.NonAttack);
        
        await _textBox.DoOpenTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);

        await UniTask.WhenAll(
            //CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
            ChangeText()
        );
        
        Debug.Log("Shooooooooot!");
        _tutorialEnemy.Attack();
        _tutorialEnemy.Pause();

        await UniTask.WaitForSeconds(_shootWaitSec, cancellationToken: cancellationToken);

        Debug.Log("レバーかスペースキーを押してください");
        // 特定の入力を受けたらPauseを回避する
        await UniTask.WaitUntil(() =>
            InputProvider.Instance.LeftLeverDir != Vector3.zero ||
            InputProvider.Instance.RightLeverDir != Vector3.zero ||
            UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed, cancellationToken: cancellationToken);
        
        _playerController.PlayerEnvroment.RemoveState(PlayerStateType.Inoperable);
        _tutorialEnemy.Resume();

        await UniTask.Yield(cancellationToken: cancellationToken);
        
        //_playerController.PlayerEnvroment.AddState(PlayerStateType.Inoperable);
        
        await _textBox.DoCloseTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);
        _textBox.ClearText();
        
        async UniTask ChangeText()
        {
            await _textBox.DoTextChangeAsync(_tutorialText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
}
