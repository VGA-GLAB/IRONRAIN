using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    public class SequencePlayer : MonoBehaviour
    {
        private enum Sequence
        {
            PrepareStartSequence,
            StartUpSequence,
            PrepareSortieSequence,
            SortieSequence,
            FirstAnnounceSequence,
            AvoidSequence,
            AttackSequence,
            TouchPanelSequence,
            QTETutorialSequence,
            MultiBattleSequence,
            PurgeSequence,
            FallSequence,
            BossStartSequence,
            FirstFunnelSequence,
            ToggleButtonSequence,
            SecondFunnelSequence,
            BossAgainSequence,
            BreakLeftArmSequence,
            FirstBossQTESequence,
            SecondQTESequence,
            BossEndSequence
        }
        
        public ISequence CurrentSequence => _currentSequence;

        [SerializeField] private SequenceManager _manager;
        [SerializeField] private bool _playOnStart = true;
        [Header("スキップ機能")]
        [SerializeField] private bool _isSkip = false;
        [SerializeField] private Sequence _startSequence;

        private ISequence[] _sequences;
        private ISequence _currentSequence;

        private void Start()
        {
            _sequences = _manager.GetSequences();

            if (_playOnStart)
            {
                Play();
            }
        }

        public void Play() => PlayAsync(this.GetCancellationTokenOnDestroy()).Forget();
        
        public async UniTask PlayAsync(CancellationToken ct)
        {
            var sequences = _manager.GetSequences();

            // スキップする際にそのSequenceがあるか、そしてそのSequenceのインデックスを取得する
            int startIndex = 0;

            foreach (var seq in sequences)
            {
                if (seq is SequenceGroup group && group.GroupName == _startSequence.ToString())
                {
                    break;
                }

                startIndex++;
            }

            if (startIndex >= sequences.Length)
            {
                startIndex = 0;
            }
            
            for (int i = 0; i < sequences.Length; i++)
            {
                _currentSequence = sequences[i];
                
                try
                {
                    if (_isSkip && i < startIndex)
                    {
                        _currentSequence.Skip();
                    }
                    else
                    {
                        await _currentSequence.PlayAsync(ct, x =>ExceptionReceiver(x, i));
                    }
                }
                catch (Exception e) when (e is not OperationCanceledException)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }

        private void ExceptionReceiver(Exception e, int index)
        {
            Debug.LogError($"Sequence Element{index}でエラー");
            throw e;
        }
    }
}
