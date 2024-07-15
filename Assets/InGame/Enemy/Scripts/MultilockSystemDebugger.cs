using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MultilockSystemExample))]
public class MultilockSystemDebugger : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper _button;
    [SerializeField] private Text _text;

    private MultilockSystemExample _multilock;

    private bool _isButtonPushed;

    private void Awake()
    {
        _multilock = GetComponent<MultilockSystemExample>();
    }

    private void Start()
    {
        // 状態のフラグ操作をコールバックに登録。
        _button.WhenSelect.AddListener(OnButtonPushed);
        _button.WhenUnselect.AddListener(OnButtonReleased);
    }

    private void OnButtonPushed()
    {
        if (!_isButtonPushed)
        {
            _isButtonPushed = true;

            MultilockAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    private void OnButtonReleased()
    {
        _isButtonPushed = false;
    }

    private async UniTaskVoid MultilockAsync(CancellationToken token)
    {
        List<GameObject> result = await _multilock.LockOnAsync(token);

        if (result == null)
        {
            _text.text = "Null";
        }
        else if (result.Count > 0)
        {
            string s = "";
            foreach (GameObject g in result)
            {
                s += $"{g.name}\n";
            }

            _text.text = s;
        }
        else
        {
            _text.text = "Zero";
        }
    }
}