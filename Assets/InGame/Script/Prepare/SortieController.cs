using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public sealed class SortieController : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _firstWaitSec = 5F;
    [SerializeField] private HatchController _secondHatch = default;
    [SerializeField] private HatchController.OpenDoorData _secondHatchData = default;
    [SerializeField] private TutorialTextBoxController _tutorialTextBoxController = default;
    [SerializeField] private string _text = "F3とF4を入力してください";
    [SerializeField] private Transform _hangerOutside = default;
    
    public async UniTask SortieSeqAsync(CancellationToken cancellationToken)
    {
        await UniTask.WaitForSeconds(_firstWaitSec, cancellationToken: cancellationToken);

        _tutorialTextBoxController.ClearText();
        
        await _tutorialTextBoxController.DoOpenTextBoxAsync(1F, cancellationToken);

        await _tutorialTextBoxController.DoTextChangeAsync(_text, 2F, cancellationToken);
        
        await UniTask.WhenAll(
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle3), cancellationToken: cancellationToken),
            UniTask.WaitUntil(() => InputProvider.Instance.GetStayInput(InputProvider.InputType.Toggle4), cancellationToken: cancellationToken)
        );

        await _tutorialTextBoxController.DoCloseTextBoxAsync(1F, cancellationToken);
        
        _tutorialTextBoxController.ClearText();
        
        await _secondHatch.OpenDoorAsync(_secondHatchData, cancellationToken);

        await _player.DOMove(_hangerOutside.position, 2F).ToUniTask(cancellationToken: cancellationToken);
    }
}
