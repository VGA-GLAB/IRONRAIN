using System;
using System.Collections.Generic;
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
        [FormerlySerializedAs("rederMapMap")] [FormerlySerializedAs("_raderMapMap")] [SerializeField] private RadarMap raderMapMap;
        public RadarMap RaderMap => raderMapMap;
        [SerializeField] private AnnounceUiController _announceUiController;
        public AnnounceUiController AnnounceUiController => _announceUiController;
        [SerializeField] private PlayerAnimation _playerAnimation;
        public PlayerAnimation PlayerAnimation => _playerAnimation;
        [SerializeField] private LaunchManager _launchManager;
        public LaunchManager LaunchManager => _launchManager;
#if UNITY_EDITOR
        [SerializeField] private Recordings _recorder;
        public Recordings Recorder => _recorder;
        [SerializeField] private FFmpegConcatenate _concatenate;
        public FFmpegConcatenate Concatenate => _concatenate;
#endif
        [SerializeField] private CurveShaderManager _curveManager;
        public CurveShaderManager CurveManager => _curveManager;

        [SerializeField] private Light[] _alertLights;
        public Light[] AlertLights => _alertLights;
        [SerializeField] private Transform _voiceTransform;
        public Transform VoiceTransform => _voiceTransform;
        [SerializeField] private Camera _mainCamera;
        public Camera MainCamera => _mainCamera;
        [SerializeField] private CockpitEmissionController _cockpitEmissionController;
        public CockpitEmissionController CockpitEmissionController => _cockpitEmissionController;

        public SoundSequenceManager SoundManager { get; set;}
        
        public class SoundSequenceManager
        {
            private Dictionary<int, int> _indexList = new();

            public void RegisterIndex(int id, int index)
            {
                if (id < 0)
                {
                    Debug.LogError("そのID無効です " + id);
                    return;
                }
                if (_indexList.ContainsKey(id))
                {
                    Debug.LogError("そのIDは重複しています");
                    return;
                }
                
                _indexList.Add(id, index);
            }

            public int UnregisterIndex(int id)
            {
                if (!_indexList.ContainsKey(id))
                {
                    Debug.LogError("そのIDは存在していません");
                    return　-1;
                }
                
                var index = _indexList[id];
                _indexList.Remove(id);

                return index;
            }
        }
    }
}
