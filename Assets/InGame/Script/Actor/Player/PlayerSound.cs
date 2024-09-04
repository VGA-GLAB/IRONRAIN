using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronRain.Player;

public class PlayerSound : MonoBehaviour
{
    [SerializeField] private Transform _toggleSoundPos;

    private PlayerEnvroment _playerEnvroment;
    private AnimationEventProvider _animationEventProvider;

    void Start()
    {
     

    }

    public void SetUp(PlayerEnvroment playerEnvroment)
    {
        _playerEnvroment = playerEnvroment;
        _animationEventProvider = playerEnvroment.AnimationEventProvider;

        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle1, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle2, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle3, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle4, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle5, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle6, () => CriAudioManager.Instance.SE.Play3D(_toggleSoundPos.position, "SE", "SE_Toggle"));

        _animationEventProvider.OnBlokenArm += () => {
            CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_BlokenArm"); };
        _animationEventProvider.OnPileBunkerBack += () => { CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position,"SE", "SE_PileBunker_Retrun"); };
        _animationEventProvider.OnPileBunkerHit += () => {
            CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_PileBunker_Hit"); };
        _animationEventProvider.OnPileBunkerInjection += () => {
            CriAudioManager.Instance.SE.Play3D(_playerEnvroment.PlayerTransform.position, "SE", "SE_PileBunker"); };
    }
}
