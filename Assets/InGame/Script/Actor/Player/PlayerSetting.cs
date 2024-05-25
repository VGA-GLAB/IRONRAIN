using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSetting : ScriptableObject
{
    public PlayerParams PlayerParamsData => _playerParams;

    [Header("�L�[�{�[�h�̓��͂������邩�ǂ���")]
    public bool IsKeyBoard;
    [Header("Player�̃p�����[�^�[�ݒ�")]
    [SerializeField] private PlayerParams _playerParams;

    [System.Serializable]
    public class PlayerParams 
    {
        public float OneGearSpeed => _oneGearSpeed;
        public float TwoGearSpeed => _twoGearSpeed;
        public float ThreeGearSpeed => _threeGearSpeed;
        public float ThrusterMoveNum => _thrusterMoveNum;
        public int ThrusterMoveTime => _thrusterMoveTime;
        public int RestrictionLane => _restrictionLane;
        //public float ReturnLaneStrengthMagnification => _returnLaneStrengthMagnification;
        public float ReturnLaneStrength => _returnLaneStrength;
        public float MaxReturnLaneStrength => _maxReturnLaneStrength;
        public float QteTimeLimit => _qteTimeLimit;
        public int Hp => _hp;

        [Header("Player��Hp")]
        [SerializeField] private int _hp;
        [Header("===Player�̈ړ��ݒ�===")]
        [Header("1��")]
        [SerializeField] private float _oneGearSpeed;
        [Header("2��")]
        [SerializeField] private float _twoGearSpeed;
        [Header("3��")]
        [SerializeField] private float _threeGearSpeed;
        [Header("�P��̃X���X�^�[�̈ړ���")]
        [SerializeField] private float _thrusterMoveNum;
        [Header("���b�ԂŃX���X�^�[�ňړ����邩")]
        [SerializeField] private int _thrusterMoveTime;
        [Header("�ǂ̃��[�����琧���������邩")]
        [SerializeField] private int _restrictionLane;
        //[Header("���[���ɖ߂����Ƃ���͂̔{��")]
        //[SerializeField] private float _returnLaneStrengthMagnification;
        [Header("���[���ɖ߂����Ƃ���͂̋���")]
        [SerializeField] private float _returnLaneStrength;
        [Header("���[���ɖ߂����Ƃ���͂̋����̍ő�l")]
        [SerializeField] private float _maxReturnLaneStrength;

        [Header("===QTE�̐ݒ�===")]
        [Header("QTE�̎��Ԑ���")]
        [SerializeField] float _qteTimeLimit;
    }

}
