using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RobotShotMock : MonoBehaviour
{
    [SerializeField] private InputActionProperty _fireInput;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletInsPos;
    [Header("î≠éÀä‘äu")]
    [SerializeField] private float _fireRate;
    [Header("ÉpÅ[ÉeÉBÉNÉã")]
    [SerializeField] private GameObject _particleEffect;

    private float _currentTime;
    private bool _isShot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shot(_fireInput.action.ReadValue<float>());
    }

    /// <summary>
    /// íeÇî≠éÀÇ∑ÇÈ
    /// </summary>
    private void Shot(float fireInput)
    {
        if (0 < fireInput && _isShot)
        {
            Instantiate(_bulletPrefab, _bulletInsPos);
            _isShot = false;
            _currentTime = 0;
            Debug.Log("íeÇë≈Ç¡ÇΩ");
        }
        else if (!_isShot)
        {
            _currentTime += Time.deltaTime;
            if (_currentTime > _fireRate)
            {
                _isShot = true;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(_bulletPrefab, _bulletInsPos);
            Instantiate(_particleEffect, _bulletInsPos);
            Debug.Log("íeÇë≈Ç¡ÇΩ");
        }
    }
}
