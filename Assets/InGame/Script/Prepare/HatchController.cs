using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public sealed class HatchController : MonoBehaviour
{
    [SerializeField] private Animation _overDoor = default;
    [SerializeField] private Animation _underDoor = default;
    [SerializeField] private ParticleSystem _smoke = default;

    public UniTask OpenDoorAsync(OpenDoorData data, CancellationToken cancellationToken)
    {
        if (_smoke) _smoke.Play();

        _underDoor.Play();
        _overDoor.Play();
        
        return UniTask.CompletedTask;
    }

    public void Open()
    {
        if (_smoke) _smoke.Play();
        _underDoor.Play();
        _overDoor.Play();
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
