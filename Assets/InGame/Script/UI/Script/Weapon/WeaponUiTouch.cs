using IronRain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUiTouch : MonoBehaviour
{
    [Header("プレイヤーウェポンモデル")]
    [SerializeField] private PlayerWeaponController _weaponController;
    [Header("音を鳴らす位置")]
    [SerializeField] private Transform _soundTransform;

    private void Start()
    {
        if( _weaponController == null)
            _weaponController = FindObjectOfType<PlayerWeaponController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finger"))
        {
            //パネルに触れた時の音
            CriAudioManager.Instance.CockpitSE.Play3D(_soundTransform.position, "SE", "SE_Panel_Tap");
            _weaponController.WeaponModel.WeaponChenge();
        }
    }
}
