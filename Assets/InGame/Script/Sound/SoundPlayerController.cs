using UnityEngine;

public class SoundPlayerController : MonoBehaviour
{
    private void OnDestroy()
    {
        CriAudioManager.Instance.SE.Reset3DPlayer();
        CriAudioManager.Instance.CockpitSE.Reset3DPlayer();
        CriAudioManager.Instance.Voice.Reset3DPlayer();
    }
}
