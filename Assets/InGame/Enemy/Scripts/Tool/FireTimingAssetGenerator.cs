using Enemy.DebugUse;
using Enemy.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;

namespace Enemy.Tool
{
    /// <summary>
    /// �G���ˌ����čU������^�C�~���O����͂���쐬����B
    /// �e�L�X�g�t�@�C���ŏo�͂����B
    /// </summary>
    public class FireTimingAssetGenerator : MonoBehaviour
    {
        [Header("Assets����̑��΃p�X")]
        [SerializeField] private string _directoryPath = "InGame/Enemy/";
        [Header("�ۑ�����t�@�C����")]
        [SerializeField] private string _fileName = "InputBuffer_Debug";
        [Header("�M�Y���ւ̕`��ݒ�")]
        [SerializeField] private float _drawOffset;

        private GUIStyle _style = new GUIStyle();
        private GUIStyleState _state = new GUIStyleState();

        private List<float> _q = new List<float>();
        private float _elapsed;
        private bool _isRecording;

        private void Awake()
        {
            _style.fontSize = 30;
            _state.textColor = Color.white;
            _style.normal = _state;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _elapsed = 0;
                _q.Clear();

                _isRecording = true;
            }
            if (Input.GetKeyUp(KeyCode.Q))
            {
                Print();
                // �Ō�̍U���^�C�~���O�ȍ~�ɉ����Ă������̒������J�b�g
                _elapsed = _q.Count > 0 ? _q[^1] : 0;

                _isRecording = false;
            }

            if(Input.GetKey(KeyCode.Q))
            {
                // �U���^�C�~���O�Ƃ��ċL�^
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _q.Add(_elapsed);
                }

                _elapsed += Time.deltaTime;
            }
        }

        // �e�L�X�g�t�@�C���Ƃ��ďo��
        private void Print()
        {
            string path = $"{Application.dataPath}/{_directoryPath}{_fileName}.txt";
            using (StreamWriter sw = new StreamWriter(path, append: false))
            {
                foreach (float f in _q)
                {
                    sw.WriteLine(f);
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Q�L�[�������ASpace�L�[���������^�C�~���O���L�^", _style);

            if (_isRecording)
            {
                GUILayout.Label("�L�^���c", _style);
            }
        }

        private void OnDrawGizmos()
        {
            // �c���̒����A�K��
            float len = 1.5f;
            // ���w�i
            GizmosUtils.Plane(Vector3.zero, 1920.0f, 1080.0f, Color.black);
            // ���Ԃ�\����(��)
            GizmosUtils.Line(new Vector2(_drawOffset, 0), new Vector2(1920.0f, 0), ColorExtensions.ThinWhite);
            // �J�n�ʒu(��)
            GizmosUtils.Line(new Vector2(_drawOffset, -len), new Vector2(_drawOffset, len), Color.green);
            // �U���^�C�~���O(��)
            foreach (float f in _q)
            {
                float p = f + _drawOffset;
                GizmosUtils.Line(new Vector2(p, -len), new Vector2(p, len), Color.red);
            }
            // �L�^����(�V�A��)
            GizmosUtils.Line(new Vector2(_drawOffset, 0), new Vector2(_elapsed + _drawOffset, 0), Color.cyan);
        }
    }
}
