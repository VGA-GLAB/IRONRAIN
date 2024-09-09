using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
#endif

namespace IronRain.Recording
{
	public class CustomRecorderController : MonoBehaviour
	{
#if UNITY_EDITOR
		/// <summary>コントローラー</summary>
		private RecorderController _controller;

		/// <summary>コントローラー設定</summary>
		private RecorderControllerSettings _controllerSettings;

		/// <summary>レコーダー設定</summary>
		private MovieRecorderSettings _recorderSettings;

		/// <summary>レコーディングした回数</summary>
		private int _recordingCount = 1;

		//-------------------------------------------------------------------------------
		// RecorderControllerの設定
		//-------------------------------------------------------------------------------

		/// <summary>フレームレートの制御方法</summary>
		[SerializeField] [Header("フレームレートの制御方法")]
		private FrameRatePlayback _playback;

		/// <summary>フレームレート</summary>
		[SerializeField] [Header("フレームレート")] private float _frameRate = 30f;

		/// <summary>フレームレート制限</summary>
		[SerializeField] [Header("フレームレート制限")] private bool _isCapped = true;

		//-------------------------------------------------------------------------------
		// 録画設定
		//-------------------------------------------------------------------------------

		/// <summary>出力の横幅</summary>
		[SerializeField] [Header("出力の横幅")] private int _width = 1920;

		/// <summary>出力の縦幅</summary>
		[SerializeField] [Header("出力の縦幅")] private int _height = 1080;

		//-------------------------------------------------------------------------------
		// 録音設定
		//-------------------------------------------------------------------------------

		[SerializeField] [Header("録音するかどうか")] private bool _isAudioPreserved = true;

		//-------------------------------------------------------------------------------
		// フォーマット設定
		//-------------------------------------------------------------------------------

		/// <summary>出力形式</summary>
		[SerializeField] [Header("出力形式")] private MovieRecorderSettings.VideoRecorderOutputFormat _format;

		/// <summary>ビデオ品質</summary>
		[SerializeField] [Header("ビデオ品質")] private VideoBitrateMode _quality;

		//-------------------------------------------------------------------------------
		// 出力設定
		//-------------------------------------------------------------------------------

		/// <summary>ファイル名</summary>
		private readonly string _fileName = "Part";

		private void Start()
		{
			SetUpRecorderController();
			SetUpRecorder();
		}

		/// <summary>コントローラーの設定を行う</summary>
		private void ConfigureRecorderControllerSettings()
		{
			_controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();

			// レコーディングモードを設定
			_controllerSettings.SetRecordModeToManual();

			// フレームレートの制御方法
			_controllerSettings.FrameRatePlayback = _playback;

			// フレームレート
			_controllerSettings.FrameRate = _frameRate;

			// フレームレートを制限するか
			_controllerSettings.CapFrameRate = _isCapped;
		}

		/// <summary>レコーダーの設定を行う</summary>
		private void ConfigureMovieRecorderSettings()
		{
			_recorderSettings = ScriptableObject.CreateInstance<MovieRecorderSettings>();

			// フレームレートの制御方法
			_recorderSettings.FrameRatePlayback = _playback;

			// フレームレート
			_recorderSettings.FrameRate = _frameRate;

			// フレームレートを制限するか
			_recorderSettings.CapFrameRate = _isCapped;

			// 出力形式
			_recorderSettings.OutputFormat = _format;

			// ビデオ品質
			_recorderSettings.VideoBitRateMode = _quality;

			// 録画設定
			_recorderSettings.ImageInputSettings =
				new GameViewInputSettings { OutputWidth = _width, OutputHeight = _height };

			// 録音設定
			// _recorderSettings.CaptureAudio = false;
			_recorderSettings.AudioInputSettings.PreserveAudio = _isAudioPreserved;

			// 有効化
			_recorderSettings.Enabled = true;
		}

		/// <summary>コントローラーのセットアップ</summary>
		private void SetUpRecorderController()
		{
			// コントローラーを設定
			ConfigureRecorderControllerSettings();

			// コントローラーを作成
			_controller = new RecorderController(_controllerSettings);
		}

		/// <summary>レコーダーのセットアップ</summary>
		private void SetUpRecorder()
		{
			// レコーダーを設定
			ConfigureMovieRecorderSettings();
			// レコーダーの設定をコントローラーの設定に追加
			_controllerSettings.AddRecorderSettings(_recorderSettings);
		}

		public bool IsRecording;

		/// <summary>レコーディングを開始する</summary>
		public void StartRecording()
		{
			if (IsRecording)
			{
				Debug.Log("Already recording.");
				return;
			}

			IsRecording = true;

			// 出力ファイル名
			_recorderSettings.OutputFile = $"Assets\\Recordings\\Videos\\{_fileName} {_recordingCount}";
			// 録画準備
			_controller.PrepareRecording();
			// 録画開始
			_controller.StartRecording();
		}

		/// <summary>レコーディングを終了する</summary>
		public void StopRecording()
		{
			// 録画終了
			_controller.StopRecording();

			// レコーディングの回数を増やす
			_recordingCount++;

			AssetDatabase.Refresh();

			IsRecording = false;
		}
#endif
	}
}
