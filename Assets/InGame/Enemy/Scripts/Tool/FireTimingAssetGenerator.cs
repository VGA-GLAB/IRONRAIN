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
    /// 敵が射撃して攻撃するタイミングを入力から作成する。
    /// テキストファイルで出力される。
    /// </summary>
    public class FireTimingAssetGenerator : MonoBehaviour
    {
        [Header("Assetsからの相対パス")]
        [SerializeField] private string _directoryPath = "InGame/Enemy/";
        [Header("保存するファイル名")]
        [SerializeField] private string _fileName = "InputBuffer_Debug";
        [Header("ギズモへの描画設定")]
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
                // 最後の攻撃タイミング以降に押していた分の長さをカット
                _elapsed = _q.Count > 0 ? _q[^1] : 0;

                _isRecording = false;
            }

            if(Input.GetKey(KeyCode.Q))
            {
                // 攻撃タイミングとして記録
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    _q.Add(_elapsed);
                }

                _elapsed += Time.deltaTime;
            }
        }

        // テキストファイルとして出力
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
            GUILayout.Label("Qキー押下中、Spaceキーを押したタイミングを記録", _style);

            if (_isRecording)
            {
                GUILayout.Label("記録中…", _style);
            }
        }

        private void OnDrawGizmos()
        {
            // 縦線の長さ、適当
            float len = 1.5f;
            // 黒背景
            GizmosUtils.Plane(Vector3.zero, 1920.0f, 1080.0f, Color.black);
            // 時間を表す線(白)
            GizmosUtils.Line(new Vector2(_drawOffset, 0), new Vector2(1920.0f, 0), ColorExtensions.ThinWhite);
            // 開始位置(緑)
            GizmosUtils.Line(new Vector2(_drawOffset, -len), new Vector2(_drawOffset, len), Color.green);
            // 攻撃タイミング(赤)
            foreach (float f in _q)
            {
                float p = f + _drawOffset;
                GizmosUtils.Line(new Vector2(p, -len), new Vector2(p, len), Color.red);
            }
            // 記録時間(シアン)
            GizmosUtils.Line(new Vector2(_drawOffset, 0), new Vector2(_elapsed + _drawOffset, 0), Color.cyan);
        }
    }
}
