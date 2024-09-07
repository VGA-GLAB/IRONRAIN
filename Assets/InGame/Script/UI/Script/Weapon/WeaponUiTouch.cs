using IronRain.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUiTouch : MonoBehaviour
{
    [Header("プレイヤーウェポンモデル")]
    [SerializeField] private PlayerWeaponController _weaponController;

    private void Start()
    {
        _weaponController = FindObjectOfType<PlayerWeaponController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Finger"))
        {
            //パネルに触れた時の音
            CriAudioManager.Instance.SE.Play("SE", "SE_Panel_Tap");
            _weaponController.WeaponModel.WeaponChenge();
        }
    }
}
