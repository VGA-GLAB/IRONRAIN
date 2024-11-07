using Cysharp.Threading.Tasks;
using Oculus.Interaction;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MultilockSystemDebugger : MonoBehaviour
{
    [SerializeField] private InteractableUnityEventWrapper _button;
    [SerializeField] private Text _text;

    private LockOnSystem _multiLock;

    private bool _isButtonPushed;

    private void Start()
    {
        _multiLock = FindAnyObjectByType<LockOnSystem>();

        // 状態のフラグ操作をコールバックに登録。
        _button.WhenSelect.AddListener(OnButtonPushed);
        _button.WhenUnselect.AddListener(OnButtonReleased);
    }

    private void OnButtonPushed()
    {
        if (!_isButtonPushed)
        {
            _isButtonPushed = true;

            _text.text = "Running...";

            MultiLockAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
    }

    private void OnButtonReleased()
    {
        _isButtonPushed = false;
    }

    private async UniTaskVoid MultiLockAsync(CancellationToken token)
    {
        List<GameObject> result = await _multiLock.MultiLockOnAsync(token);

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