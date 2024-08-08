using UnityEngine;
using UnityEngine.UI;

namespace Enemy.DebugUse
{
    public class EnemyMotionCheck : MonoBehaviour
    {
        [System.Serializable]
        public class PlayButton
        {
            public string ClipName;
            public Button Button;
        }

        [System.Serializable]
        public class Actor
        {
            public string Name;
            public Button Button;
            public GameObject Object;
            public GameObject MyMotionSelect;
            public string DefaultAnimName = "Idle";
            public Slider MoveParamSlider;
            public PlayButton[] PlayButton;

            public Animator Animator
            {
                get => Object.GetComponentInChildren<Animator>();
            }

            public void PlayOnButtonClick()
            {
                for (int i = 0; i < PlayButton.Length; i++)
                {
                    // 適当な変数に代入せずiを引数にすると挙動がおかしくなる。
                    int x = i;
                    PlayButton[i].Button.onClick.AddListener(() => Play(PlayButton[x].ClipName));
                }
            }

            public void View(bool value)
            {
                Object.SetActive(value);
                MyMotionSelect.SetActive(value);

                // ボタンの色変更
                Color c = value ? Color.green : Color.white;
                Button.GetComponent<Image>().color = c;
            }

            public void Play(string animName)
            {
                if (Animator != null) Animator.Play(animName);
            }

            public void PlayDefault()
            {
                Play(DefaultAnimName);
            }

            public void ApplyMoveParam()
            {
                if (Animator != null && MoveParamSlider != null)
                {
                    Animator.SetFloat("SpeedY", MoveParamSlider.value);
                }
            }
        }

        [SerializeField] private Actor[] _data;
        [Header("カメラ操作")]
        [SerializeField] private Transform _cameraRoot;
        [SerializeField] private Slider _cameraRotationSlider;


        private void Start()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                // 再生対象の選択をボタンに登録。
                // 適当な変数に代入せずiを引数にすると挙動がおかしくなる。
                int x = i; 
                _data[i].Button.onClick.AddListener(() => ChangeTarget(x));

                // 再生ボタンにアニメーションの再生を登録。
                _data[i].PlayOnButtonClick();
            }

            // 先頭のもの以外を消しておく。
            ChangeTarget(0);
        }

        private void Update()
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i].ApplyMoveParam();
            }

            CameraControl();
        }

        // 表示を切り替える。
        private void ChangeTarget(int index)
        {
            for (int i = 0; i < _data.Length; i++)
            {
                _data[i].View(index == i);
            }

            // デフォルトのアニメーションを再生。
            _data[index].PlayDefault();
        }

        // カメラの制御
        private void CameraControl()
        {
            _cameraRoot.rotation = Quaternion.Euler(0, _cameraRotationSlider.value * 360.0f, 0);
        }
    }
}
