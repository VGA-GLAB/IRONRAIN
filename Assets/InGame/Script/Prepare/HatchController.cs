using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public sealed class HatchController : MonoBehaviour
{
    [SerializeField] private Transform _overDoor = default;
    [SerializeField] private Transform _underDoor = default;
    [SerializeField] private ParticleSystem _smoke = default;

    public async UniTask OpenDoorAsync(OpenDoorData data, CancellationToken cancellationToken)
    {
        _smoke.Play();
        
        await UniTask.WhenAll(
            _overDoor.DOMoveY(data.FirstMoveValue, data.FirstDuration).ToUniTask(cancellationToken: cancellationToken),
            _underDoor.DOMoveY(-data.FirstMoveValue, data.FirstDuration).ToUniTask(cancellationToken: cancellationToken)
        );
        
        await UniTask.WhenAll(
            _overDoor.DOMoveY(data.SecondMoveValue, data.SecondDuration).ToUniTask(cancellationToken: cancellationToken),
            _underDoor.DOMoveY(-data.SecondMoveValue, data.SecondDuration).ToUniTask(cancellationToken: cancellationToken)
        );
    }

    [Serializable]
    public struct OpenDoorData
    {
        [SerializeField] private float _firstDuration;
        public float FirstDuration { get => _firstDuration; set => _firstDuration = value; }
        
        [SerializeField] private float _firstMoveValue;
        public float FirstMoveValue { get => _firstMoveValue; set => _firstMoveValue = value; }

        [SerializeField] private float _secondDuration;
        public float SecondDuration { get => _secondDuration; set => _secondDuration = value; }
        
        [SerializeField] private float _secondMoveValue;
        public float SecondMoveValue { get => _secondMoveValue; set => _secondMoveValue = value; }
    }
}
