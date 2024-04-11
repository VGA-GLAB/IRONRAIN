using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// ��ʏ�ɕ�����\�����郁����
    /// </summary>
    public class MemoPad : MonoBehaviour
    {
        private GUIStyle _style = new GUIStyle();
        private GUIStyleState _state = new GUIStyleState();

        [SerializeField] private string _memo;
        [SerializeField] private int _size;
        [SerializeField] private Rect _area;

        private void Awake()
        {
            _style.fontSize = _size;
            _state.textColor = Color.white;
            _style.normal = _state;
        }

        private void OnGUI()
        {
            GUILayout.BeginArea(_area);
            GUILayout.Label($"AI�g�p: {_memo}", _style);
            GUILayout.EndArea();
        }
    }
}
