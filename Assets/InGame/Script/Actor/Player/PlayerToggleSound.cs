using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle1, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle2, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle3, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle4, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle5, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
        InputProvider.Instance.SetEnterInput(InputProvider.InputType.Toggle6, () => CriAudioManager.Instance.SE.Play("SE", "SE_Toggle"));
    }
}
