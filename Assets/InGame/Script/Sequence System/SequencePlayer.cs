using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    // シーケンスの管理　スキップの処理　例外処理　デバッグロジックを管理
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

        private async UniTask PlayAsync(CancellationToken ct)
        {
            if (_sequences == null || _sequences.Length == 0)
            {
                Debug.LogWarning("シーケンスが存在しません。シーケンスを確認してください");
                return;
            }

            // スキップ開始インデックスを計算
            int startIndex = GetStartIndex();

            
            for (int i = 0; i < _sequences.Length; i++)
            {
                _currentSequence = _sequences[i];
                Debug.Log($"[{i}/{_sequences.Length}] シーケンス開始: {_currentSequence.GetType().Name}");
                
                try
                {
                    if (_isSkip && i < startIndex)
                    {
                        Debug.Log($"シーケンスをスキップ: {_currentSequence.GetType().Name}");
                        _currentSequence.Skip();
                    }
                    else
                    {
                        await _currentSequence.PlayAsync(ct, ex =>ExceptionReceiver(ex, i));
                    }
                }
                catch (OperationCanceledException)
                {
                    Debug.LogWarning("シーケンスがキャンセルされました。");
                }
                catch (Exception e)
                {
                    ExceptionReceiver(e, i);
                }
            }
        }

        private int GetStartIndex()
        {
            // スタートシーケンス名に対応するインデックスを探す
            for (int i = 0; i < _sequences.Length; i++)
            {
                if (_sequences[i] is SequenceGroup group && group.GroupName == _startSequence.ToString())
                {
                    return i;
                }
            }

            return 0;
        }

        private void ExceptionReceiver(Exception e, int index)
        {
            if (e is OperationCanceledException)
            {
                return;
            }

            Debug.LogError($"シーケンス {index} でエラーが発生しています: {e.GetType().Name} - {e.Message}\n{e.StackTrace}");
            throw e;
        }
    }
}
