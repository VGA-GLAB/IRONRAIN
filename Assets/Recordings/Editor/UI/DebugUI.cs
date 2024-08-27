using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IronRain.Recording
{
    public class DebugUI : MonoBehaviour
    {
        [SerializeField]
        private Recorder _recorder;

        [SerializeField]
        private TMPro.TextMeshProUGUI _label;
        [SerializeField]
        private Transform _image;

        private void Update()
        {
            _image.gameObject.SetActive(_recorder.IsRecording);
            _label.gameObject.SetActive(_recorder.IsRecording);

            if (_recorder.IsRecording)
            {
                var y = Mathf.Sin(Time.time);
                var x = Mathf.Cos(Time.time);
                _image.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
            }
        }
    }
}
