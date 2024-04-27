using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class PlayerSetting : ScriptableObject
{
    public PlayerParams PlayerParamsData => _playerParams;

    [Header("キーボードの入力を許可するかどうか")]
    public bool IsKeyBoard;
    [Header("Playerのパラメーター設定")]
    [SerializeField] private PlayerParams _playerParams;

    [System.Serializable]
    public class PlayerParams 
    {
        public float OneGearSpeed => _oneGearSpeed;
        public float TwoGearSpeed => _twoGearSpeed;
        public float ThreeGearSpeed => _threeGearSpeed;
        public float ThrusterMoveNum => _thrusterMoveNum;
        public int ThrusterMoveTime => _thrusterMoveTime;
        public float QteTimeLimit => _qteTimeLimit;
        public int Hp => _hp;

        [Header("PlayerのHp")]
        [SerializeField] private int _hp;
        [Header("===Playerの移動設定===")]
        [Header("1速")]
        [SerializeField] private float _oneGearSpeed;
        [Header("2速")]
        [SerializeField] private float _twoGearSpeed;
        [Header("3速")]
        [SerializeField] private float _threeGearSpeed;
        [Header("１回のスラスターの移動量")]
        [SerializeField] private float _thrusterMoveNum;
        [Header("何秒間でスラスターで移動するか")]
        [SerializeField] private int _thrusterMoveTime;

        [Header("===QTEの設定===")]
        [Header("QTEの時間制限")]
        [SerializeField] float _qteTimeLimit;
    }

}
