using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IronRain.SequenceSystem
{
    public class PokeCockpitControl : MonoBehaviour
    {
        [SerializeField] private InteractableUnityEventWrapper _move;
        [SerializeField] private InteractableUnityEventWrapper _rotateRight;
        [SerializeField] private InteractableUnityEventWrapper _rotateLeft;
        [SerializeField] private InteractableUnityEventWrapper _allReset;
        [SerializeField] private Transform _cockpit;
        [SerializeField] private Text _text;

        float _speed;
        float _angle;

        Vector3 _defaultPos;
        Quaternion _defaultRot;

        void Start()
        {
            if (_cockpit == null) return;

            _defaultPos = _cockpit.position;
            _defaultRot = _cockpit.rotation;

            _move.WhenSelect.AddListener(() => _speed = 5.0f);
            _move.WhenUnselect.AddListener(() => _speed = 0);

            _rotateRight.WhenSelect.AddListener(() => _angle = 10.0f);
            _rotateRight.WhenUnselect.AddListener(() => _angle = 0);

            _rotateLeft.WhenSelect.AddListener(() => _angle = -10.0f);
            _rotateLeft.WhenUnselect.AddListener(() => _angle = 0);

            _allReset.WhenSelect.AddListener(AllReset);
        }

        void Update()
        {
            if (_cockpit == null) return;

            // 移動
            _cockpit.position += _cockpit.forward * Time.deltaTime * _speed;
            // 回転
            _cockpit.Rotate(Vector3.up * Time.deltaTime * _angle);
            // 情報
            string p = _cockpit.position.ToString();
            string r = _cockpit.rotation.ToString();
            string s = _cockpit.localScale.ToString();
            _text.text = $"位置:{p}\n回転:{r}\n大きさ:{s}";
        }

        void AllReset()
        {
            _cockpit.position = _defaultPos;
            _cockpit.rotation = _defaultRot;
        }
    }
}
