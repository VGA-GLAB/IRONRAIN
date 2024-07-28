using UnityEngine;

/// <summary>追跡パートのBGM</summary>
public class BGM : MonoBehaviour
{
    [SerializeField, Header("BGMを再生するか")] 
    private bool _isPlay = false;

    [SerializeField, Header("BGMの音量"), Range(0, 1.0f)] 
    private float _volume = 1.0f;

    void Start()
    {
        if( _isPlay )
        {
            CriSoundManager.Instance.BGM.Play("BGM", "BGM_MoonField", _volume);
        }
    }
}
