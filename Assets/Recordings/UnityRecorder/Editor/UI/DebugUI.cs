using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace IronRain.Recording
{
    public class DebugUI : MonoBehaviour
    {
        [FormerlySerializedAs("recorderController")] [FormerlySerializedAs("_recorder")] [SerializeField]
        private CustomRecorderController customRecorderController;

        [SerializeField]
        private TMPro.TextMeshProUGUI _label;
        [SerializeField]
        private Transform _image;

        private void Update()
        {
            _image.gameObject.SetActive(customRecorderController.IsRecording);
            _label.gameObject.SetActive(customRecorderController.IsRecording);

            if (customRecorderController.IsRecording)
            {
                var y = Mathf.Sin(Time.time);
                var x = Mathf.Cos(Time.time);
                _image.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(y, x) * Mathf.Rad2Deg);
            }
        }
    }
}
