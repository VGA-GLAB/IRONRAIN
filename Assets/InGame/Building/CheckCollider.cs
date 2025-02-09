using UnityEngine;

public class CheckCollider : MonoBehaviour
{
    private bool _isHit;

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        _isHit = false;
    }
    // ToDo : HitAnimationを再生
    private void PlayHitAnimation()
    {
        Debug.Log("Hit");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(_isHit || ! other.CompareTag("Player"))
            return;

        PlayHitAnimation();
        _isHit = true;
    }
}
