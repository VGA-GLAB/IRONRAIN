using Enemy.Control;
using UnityEngine;

namespace Enemy.DebugUse
{
    /// <summary>
    /// 視界のデバッグ
    /// </summary>
    public class FovSensorDebugger : MonoBehaviour
    {
        FovSensor _fovSensor;

        private void Awake()
        {
            _fovSensor = GetComponent<FovSensor>();
        }

        private void OnEnable()
        {
            _fovSensor.OnCaptureEnter += Enter;
            _fovSensor.OnCaptureStay += Stay;
            _fovSensor.OnCaptureExit += Exit;
        }

        private void OnDisable()
        {
            _fovSensor.OnCaptureEnter -= Enter;
            _fovSensor.OnCaptureStay -= Stay;
            _fovSensor.OnCaptureExit -= Exit;
        }

        private void Enter(Collider collider)
        {
            collider.transform.localScale = Vector3.one * 3;
            Debug.Log($"Enter: {collider.name}");
        }

        private void Stay(Collider collider)
        {
            collider.transform.localScale -= Vector3.one * Time.deltaTime;
            Debug.Log($"Stay: {collider.name}");
        }

        private void Exit(Collider collider)
        {
            collider.transform.localScale = Vector3.one * 1.5f;
            Debug.Log($"Exit: {collider.name}");
        }
    }
}
