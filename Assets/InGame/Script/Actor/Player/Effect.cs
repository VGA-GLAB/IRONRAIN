using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IronRain.Player;
using UnityEngine.VFX;

public class Effect : MonoBehaviour
{

    [SerializeField] private VisualEffect _effect;
    [Header("再生時間")]
    [SerializeField] private float _time;
    private ObjectPool _pool;

    private void Start()
    {
        
    }

    public void SetUp(ObjectPool pool) 
    {
        _pool = pool;
    }

    private void OnEnable()
    {
        StartCoroutine(Play());
    }

    private IEnumerator Play() 
    {
        _effect.Play();
        yield return new WaitForSeconds(_time);
        _effect.Stop();
        _pool.ReleaseEffect(this);
    }
}
