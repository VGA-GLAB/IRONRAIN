using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class ToggleButtonSequence : AbstractSequenceBase
{
    [SerializeField] private TutorialTextBoxController _textBox = default;
    [SerializeField] private float _firstDelayTime = 1F;
    [SerializeField] private PlayerController _playerController;
    private PlayerWeaponController _playerWeaponController;
    [Header("テキストボックスに関する値")]
    [SerializeField] private float _textBoxOpenAndCloseSec = 0.5F;
    [SerializeField, TextArea] private string _tutorialText = "現在、テストテキストが表示されています";
    [SerializeField] private float _textDuration = 3F;
    
    [Header("アナウンス音声に関する値")]
    [SerializeField] private string _announceCueSheetName = "SE";
    [SerializeField] private string _announceCueName = "SE_Kill";
    [SerializeField] private float _waitAfterAnnounceSec = 1F;

    [Header("マルチロックオン")] [SerializeField] private MouseMultilockSystem _mouseMultilockSystem;

    private void Start()
    {
        _playerWeaponController = _playerController.SeachState<PlayerWeaponController>();
    }
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
        
        //パネルに指示がでる前兆音を鳴らす
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel");
        
        await  UniTask.WhenAll(
            //CriAudioManager.Instance.SE.PlayAsync(_announceCueSheetName, _announceCueName, cancellationToken),
            ChangeText(_tutorialText),
            //トグル４、トグル５を押し、マルチロックをしてください
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), cancellationToken: cancellationToken),
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle5), cancellationToken: cancellationToken)
        );
        //プレイヤーを攻撃不可にする
        _playerController.PlayerEnvroment.AddState(PlayerStateType.NonAttack);
        //トグルの音を出す
        CriAudioManager.Instance.SE.Play("SE", "SE_Toggle");
        //マルチロックフラグがオンになる
        _mouseMultilockSystem.MultilockOnStart();
        
        //パネルに指示がでる前兆音を鳴らす
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel");
        
        await ChangeText("敵UIをなぞってマルチロックオンをしてください");//テキストを変える
        await UniTask.WaitUntil(() => !_mouseMultilockSystem.IsMultilock, cancellationToken: cancellationToken);
        _playerWeaponController.WeaponModel.MulchShot();
        //パネルに指示がでる前兆音を鳴らす
        CriAudioManager.Instance.SE.Play("SE", "SE_Panel");
        
        await ChangeText("OK");//テキストを変える
        await _textBox.DoCloseTextBoxAsync(_textBoxOpenAndCloseSec, cancellationToken);
        await UniTask.WaitForSeconds(3, cancellationToken: cancellationToken);
        async UniTask ChangeText(string changeText)
        {
            _textBox.ClearText();
            await _textBox.DoTextChangeAsync(changeText, _textDuration, cancellationToken);
            await UniTask.WaitForSeconds(_waitAfterAnnounceSec, cancellationToken: cancellationToken);
        }
    }
    
}
