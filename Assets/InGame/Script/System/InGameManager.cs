using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class InGameManager : MonoBehaviour
{
    [Header("スキップ機能の有効化")]
    [SerializeField] private bool _isSkip = false;
    [Header("そのインデックスのシーケンスまでスキップします")]
    [SerializeField] private int _skipIndex = 0;

    [Space, Header("シーケンスがすべて終了した際に呼び出すシーン名")]
    [SerializeField] private string _loadSceneName;
    [Space]
    [SerializeField] private AbstractSequenceBase[] _sequences = default;
    private AbstractSequenceBase _currentSequence;
    public AbstractSequenceBase CurrentSequence => _currentSequence;



    public interface IProvidePlayerInformation
    {
        public ISubject<Unit> StartQTE { get; }
        
        public ISubject<QTEResultType> EndQTE { get; }
        
        public float TimeScale { get; set; }
    }

    public void InGameStartUnityEventReceive() => InGameStartAsync(this.GetCancellationTokenOnDestroy()).Forget();

    private async UniTask InGameStartAsync(CancellationToken ct)
    {
        for (int i = 0; i < _sequences.Length; i++)
        {
            if (_isSkip && i <= _skipIndex)
            {
                _sequences[i].OnSkip();
            }
            else
            {
                _currentSequence = _sequences[i];   
                await _sequences[i].PlaySequenceAsync(ct);
            }
        }
        
        InGameEnd();
    }

    private void InGameEnd()
    {
        SceneManager.LoadScene(_loadSceneName);
    }
}
