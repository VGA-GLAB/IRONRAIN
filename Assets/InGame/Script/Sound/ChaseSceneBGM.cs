using UnityEngine;

/// <summary>追跡パートのBGM</summary>
public class ChaseSceneBGM : MonoBehaviour
{
    [SerializeField,Header("BGMを再生するか")] 
    private bool _isPlay = false;
    
    [SerializeField, Header("BGMの音量"), Range(0, 1)]
    private float _volume = 0;

    void Start()
    {
        if (_isPlay) 
            CriSoundManager.Instance.BGM.Play("BGM", "BGM_Field", _volume);
    }
}
