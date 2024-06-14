using System.Threading;
using Cysharp.Threading.Tasks;
using Enemy.Control;
using UnityEngine;

public sealed class AvoidanceSequence : AbstractSequenceBase
{
    [SerializeField] private PlayerStoryEvent _playerStoryEvent = default;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private EnemyController _tutorialEnemy = default;
    [SerializeField] private float _shootWaitSec = 2F;
    
    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;
    
    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;
    
    public override async UniTask PlaySequenceAsync(CancellationToken ct)
    {

        await UniTask.WaitForSeconds(2F, cancellationToken: ct);

        await UniTask.WaitUntil(() => _tutorialEnemy.BlackBoard.IsApproachCompleted, cancellationToken: ct);
        
        
        Debug.Log("Shooooooooot!");
        _tutorialEnemy.Attack();
        
        await UniTask.WaitForSeconds(_shootWaitSec, cancellationToken: ct);

        _playerStoryEvent.StartChaseScene();
        _playerController.PlayerEnvroment.AddState(PlayerStateType.Inoperable);
        _tutorialEnemy.Pause();

        Debug.Log("レバーかスペースキーを押してください");
        // 特定の入力を受けたらPauseを回避する
        await UniTask.WaitUntil(() =>
            InputProvider.Instance.LeftLeverDir != Vector3.zero ||
            InputProvider.Instance.RightLeverDir != Vector3.zero ||
            UnityEngine.InputSystem.Keyboard.current.spaceKey.isPressed, cancellationToken: ct);
        
        _playerController.PlayerEnvroment.RemoveState(PlayerStateType.Inoperable);
        _tutorialEnemy.Resume();

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
        
        async UniTask ChangeText()
        {
            await _textBox.DoTextChangeAsync(_tutorialText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
}
