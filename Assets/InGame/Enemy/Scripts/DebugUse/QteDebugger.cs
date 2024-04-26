using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    public class QteDebugger : MonoBehaviour
    {
        private GUIStyle _style = new GUIStyle();
        private GUIStyleState _state = new GUIStyleState();

        private bool _isRunning;

        private void Awake()
        {
            _style.fontSize = 30;
            _state.textColor = Color.white;
            _style.normal = _state;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _isRunning = !_isRunning;
            }

            ProvidePlayerInformation.TimeScale = _isRunning ? 0.1f : 1;
        }

        private void OnGUI()
        {
            GUILayout.Label($"Enemyë§Ç≈QTEÇÃTimeScaleÇëÄçÏíÜ", _style);
            GUILayout.Label($"QTEíÜ: {_isRunning}", _style);
        }
    }
}
