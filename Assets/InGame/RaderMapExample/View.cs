using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RaderMapExample
{
    public class View : MonoBehaviour
    {
        [SerializeField] private Transform _parent;
        [SerializeField] private Image _iconPrefab;
        [SerializeField] private float _scale = 1.0f;
        [SerializeField] private float _distance = 2.5f;

        private Rader _rader;
        private Image[] _icons;

        void Start()
        {
            _rader = FindObjectOfType<Rader>();

            // レーダーで捉える事が出来る限界と同じ数だけアイコンを生成しておく。
            _icons = new Image[_rader.Capacity];
            for (int i = 0; i < _icons.Length; i++)
            {
                _icons[i] = Instantiate(_iconPrefab, _parent);
                _icons[i].transform.position = _parent.position;
                _icons[i].enabled = false;
            }
        }

        void Update()
        {
            DisableAllIcons();

            foreach(Vector2 vec in _rader.GetResultVec2())
            {
                EnableIcon(vec);
            }
        }

        private void DisableAllIcons()
        {
            foreach (Image i in _icons) i.enabled = false;
        }

        private void EnableIcon(Vector2 vec)
        {
            Vector3 offset = _parent.position;
            Vector3 diff = (Vector3)vec * _scale;

            if (diff.sqrMagnitude > _distance * _distance) return;

            if (TryGetDisableIcon(out Image icon))
            {
                icon.transform.localPosition = diff;
            }
        }

        private bool TryGetDisableIcon(out Image icon)
        {
            foreach (Image i in _icons)
            {
                if (!i.enabled) 
                { 
                    icon = i;
                    icon.enabled = true;
                    return true; 
                }
            }

            icon = null;
            return false;
        }
    }
}
