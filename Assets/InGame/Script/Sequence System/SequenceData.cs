using System;
using UnityEngine;

namespace IronRain.SequenceSystem
{
    [Serializable]
    public struct SequenceData
    {
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
    }
}
