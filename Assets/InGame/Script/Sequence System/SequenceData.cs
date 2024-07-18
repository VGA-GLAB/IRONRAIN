using System;
using Enemy.Control;
using UnityEngine;
using UnityEngine.Serialization;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public struct SequenceData
    {
        [Header("Prepare")]
        [SerializeField] private Transform _player;
        public Transform PlayerTransform => _player;
        [SerializeField] private Transform _secondHatchTarget;
        public Transform SecondHatchTarget => _secondHatchTarget;
        [SerializeField] private Transform _hangerOutsideTarget;
        public Transform HangerOutsideTarget => _hangerOutsideTarget;
        [SerializeField] private TutorialTextBoxController _textBox;
        public TutorialTextBoxController TextBox => _textBox;
        [SerializeField] private Material[] _monitorMaterials;
        public Material[] MonitorMaterials => _monitorMaterials;
        [SerializeField] private HatchController _firstHatch;
        public HatchController FirstHatch => _firstHatch;
        [SerializeField] private HatchController _secondHatch;
        public HatchController SecondHatch => _secondHatch;
        [Header("Chase")]
        [SerializeField] private PlayerStoryEvent _playerStoryEvent;
        public PlayerStoryEvent PlayerStoryEvent => _playerStoryEvent;
        [SerializeField] private PlayerController _playerController;
        public PlayerController PlayerController => _playerController;
        [SerializeField] private EnemyController _tutorialEnemy;
        public EnemyController TutorialEnemy => _tutorialEnemy;
        [SerializeField] private EnemyManager _enemyManager;
        public EnemyManager EnemyManager => _enemyManager;
        [SerializeField] private LockOnSystem _lockSystem;
        public LockOnSystem LockSystem => _lockSystem;
        [SerializeField] private RaderMap _raderMapMap;
        public RaderMap RaderMap => _raderMapMap;
    }
}
