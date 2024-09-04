using System;
using Enemy;
using UnityEngine;
using UnityEngine.Serialization;
using IronRain.Player;
using IronRain.ShaderSystem;
using Recorder = UnityEngine.Profiling.Recorder;

#if UNITY_EDITOR
using IronRain.Recording;
using UnityEditor.Recorder;
#endif

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
        [SerializeField] private Renderer[] _monitorRenderer;
        public Material[] MonitorMaterials => Array.ConvertAll(_monitorRenderer, x => x.material);
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
        [SerializeField] private AnnounceUiController _announceUiController;
        public AnnounceUiController AnnounceUiController => _announceUiController;
        [SerializeField] private PlayerAnimation _playerAnimation;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        [SerializeField] private LaunchManager _launchManager;
        public LaunchManager LaunchManager => _launchManager;
#if UNITY_EDITOR
        [SerializeField] private CustomRecorderController _recorder;
        public CustomRecorderController Recorder => _recorder;
#endif
        [SerializeField] private CurveShaderManager _curveManager;
        public CurveShaderManager CurveManager => _curveManager;
    }
}
