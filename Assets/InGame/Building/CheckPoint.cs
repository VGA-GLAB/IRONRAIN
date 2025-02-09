using UnityEngine;

public class CheckPoint : MonoBehaviour
{
   [SerializeField] private Building _building;
   [SerializeField] private GameObject _effect;

    private bool _isChecked;
    private ParticleSystem _particleSystem;

    private void Start()
    {
        _isChecked = false;
        _particleSystem = _effect.GetComponent<ParticleSystem>();
    }

    // コライダーにぶつかった時にビルのAnimationを実行する
    private void OnTriggerEnter(Collider other)
    {
        if (_isChecked || !other.CompareTag("Player"))
            return;

        _particleSystem.Play();
        _building.StartBuildingAnimation();
        _isChecked = true;
    }
}