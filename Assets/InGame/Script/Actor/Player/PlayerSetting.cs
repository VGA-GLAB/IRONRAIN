using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.Player
{
    [CreateAssetMenu]
    public class PlayerSetting : ScriptableObject
    {
        public PlayerParams PlayerParamsData => _playerParams;

        [Header("キーボードの入力を許可するかどうか")]
        public bool IsKeyBoard;
        public bool IsVRInput;
        public bool IsFryCon;
        public bool IsToggleTest;
        [Header("チェイスシーンをすぐ始めるか")]
        public bool IsStartChaseScene;
        [Header("Playerのパラメーター設定")]
        [SerializeField] private PlayerParams _playerParams;

        [System.Serializable]
        public class PlayerParams
        {
            public float Speed => _speed;
            public float ThrusterMoveNum => _thrusterMoveNum;
            public float ThrusterMoveTime => _thrusterMoveTime;
            public int RestrictionLane => _restrictionLane;
            public int ForcingMoveLane => _forcingMoveLane;
            //public float ReturnLaneStrengthMagnification => _returnLaneStrengthMagnification;
            public float ReturnLaneStrength => _returnLaneStrength;
            public float MaxReturnLaneStrength => _maxReturnLaneStrength;
            public float QteTimeLimit => _qteTimeLimit;
            public int Hp => _hp;
            public float ThrusterCt => _thrusterCt;
            public float ChangeWeaponCt => _changeWeaponCt;
            public float QteGoDistance => _qteGoDistance;
            public float QteGoDistanceTime => _qteGoDistanceTime;
            public float Crack1 => _crack1;
            public float Crack2 => _crack2;
            public float Crack3 => _crack3;
            public float Crack4 => _crack4;

            [Header("PlayerのHp")]
            [SerializeField] private int _hp;
            [Header("===ビビに関する設定===")]
            [Header("ビビがはいる割合１番目")]
            [SerializeField] private float _crack1;
            [Header("ビビがはいる割合2番目")]
            [SerializeField] private float _crack2;
            [Header("ビビがはいる割合3番目")]
            [SerializeField] private float _crack3;
            [Header("ビビがはいる割合4番目")]
            [SerializeField] private float _crack4;
            [Header("===Playerの移動設定===")]
            [Header("スピード")]
            [SerializeField] private int _speed;
            [Header("１回のスラスターの移動量")]
            [SerializeField] private float _thrusterMoveNum;
            [Header("何秒間でスラスターで移動するか")]
            [SerializeField] private float _thrusterMoveTime;
            [Header("どのレーンから制限をかけるか")]
            [SerializeField] private int _restrictionLane;
            [Header("どのレーンから強制移動を始めるか")]
            [SerializeField] private int _forcingMoveLane;
            //[Header("レーンに戻そうとする力の倍率")]
            //[SerializeField] private float _returnLaneStrengthMagnification;
            [Header("レーンに戻そうとする力の強さ")]
            [SerializeField] private float _returnLaneStrength;
            [Header("レーンに戻そうとする力の強さの最大値")]
            [SerializeField] private float _maxReturnLaneStrength;
            [Header("スラスタークールタイム")]
            [SerializeField] private float _thrusterCt;

            [Header("===QTEの設定===")]
            [Header("QTEの時間制限")]
            [SerializeField] private float _qteTimeLimit;
            [Header("シールドの敵に対して進む距離")]
            [SerializeField] private float _qteGoDistance;
            [Header("シールドの敵に対して進む距離にかける時間")]
            [SerializeField] private float _qteGoDistanceTime;

            [Header("===武器の設定===")]
            [Header("武器変更のクールタイム")]
            [SerializeField] private float _changeWeaponCt;
        }

    }
}
