using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SequenceControllerBase : MonoBehaviour
{
    protected List<EventSequenceBase> _sequences;
    
    protected EventSequenceBase _currentSequence;
    
    public interface IEventSequence
    {
        public event UnityAction OnSequenceStart;

        public event UnityAction OnSequenceEnd;
    }
    
    [Serializable]
    public abstract class EventSequenceBase : IEventSequence
    {
        [SerializeField]
        private UnityEvent _onSequenceStart;

        public UnityEvent OnSequenceStartEvent => _onSequenceStart;

        [SerializeField]
        private UnityEvent _onSequenceEnd;

        public UnityEvent OnSequenceEndEvent => _onSequenceEnd;

        public event UnityAction OnSequenceStart
        {
            add => _onSequenceStart.AddListener(value);
            remove => _onSequenceStart.RemoveListener(value);
        }
        
        public event UnityAction OnSequenceEnd
        {
            add => _onSequenceEnd.AddListener(value);
            remove => _onSequenceEnd.RemoveListener(value);
        }
    }

    /// <summary>イベントを挿入したいシークエンスを取得する</summary>
    /// <typeparam name="T">取得したいシーケンス</typeparam>
    /// <returns></returns>
    public IEventSequence GetSequence<T>() where T : EventSequenceBase
    {
        foreach (var VARIABLE in _sequences)
        {
            if (VARIABLE is T)
            {
                return VARIABLE;
            }
        }

        Debug.LogWarning($"現在, {nameof(T)}シーケンスは存在していません");
        return null;
    }
    
    private EventSequenceBase GetSequenceBase<T>() where T : EventSequenceBase
    {
        foreach (var VARIABLE in _sequences)
        {
            if (VARIABLE is T)
            {
                return VARIABLE;
            }
        }

        Debug.LogWarning($"現在, {nameof(T)}シーケンスは存在していません");
        return null;
    }

    /// <summary>現在のシークエンスを取得する</summary>
    /// <returns></returns>
    public IEventSequence GetCurrentSequence() => _currentSequence;

    /// <summary></summary>
    /// <typeparam name="T">次のSequence</typeparam>
    public void ChangeSequence<T>() where T : EventSequenceBase
    {
        _currentSequence?.OnSequenceEndEvent?.Invoke();
        _currentSequence = GetSequenceBase<T>();
        _currentSequence.OnSequenceStartEvent?.Invoke();
    }
}
