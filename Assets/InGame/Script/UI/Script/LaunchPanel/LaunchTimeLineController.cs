using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace IronRain.SequenceSystem
{
    public class LaunchTimeLineController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _playableDirector;  // PlayableDirectorコンポーネントへの参照
        [SerializeField] private TimelineAsset _timeline;  // 再生するTimelineアセット
        [Header("Uiの親オブジェクト")]
        [SerializeField] private GameObject _launchUi;
        [Header("それぞれのUiアニメーションスクリプト")]
        [SerializeField] private RightHighCircle[] _rightHighCircles;
        [SerializeField] private SideBarController[] _sideBarController;
        [SerializeField] private TextBoxController _textBoxController;
        [SerializeField] private PulseController _pulseController;
        [SerializeField] private CenterCircleManager _centerCircleManager;

        // Start is called before the first frame update
        void Start()
        {
            // TimelineAssetをPlayableDirectorに設定
            _playableDirector.playableAsset = _timeline;
            _launchUi.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                _playableDirector.Play();
            }
        }

        public void StartAnimation()
        {
            for(int i = 0; i < _rightHighCircles.Length; i++)
            {
                _rightHighCircles[i].ActiveGauge();
            }

            for(int i = 0; i < _sideBarController.Length; i++)
            {
                _sideBarController[i].StartGaugeAnimation();
            }

            _textBoxController.WriteText();
            _pulseController.StartScroll();
            _centerCircleManager.ActiveAnimation();
        }
    }
}
