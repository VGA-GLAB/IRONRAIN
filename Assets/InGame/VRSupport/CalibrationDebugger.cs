using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRSupport;

namespace IronRain.SequenceSystem
{
    [RequireComponent(typeof(CalibrationWithSceneModel))]
    public class CalibrationDebugger : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MRUKRoom room = FindAnyObjectByType<MRUKRoom>();
                GetComponent<CalibrationWithSceneModel>().Calibration(room);
            }
        }
    }
}
