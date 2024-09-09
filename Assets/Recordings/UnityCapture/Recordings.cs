using System;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
///     パワーシェルでコマンドを実行する機能
/// </summary>
public class Recordings : MonoBehaviour
{
	private const string StartRecordBat =
		@"""C:\Users\vantan\Desktop\IronRain\Assets\Recordings\UnityCapture\RecordStart.bat""";

	private const string StopRecordBat =
		@"""C:\Users\vantan\Desktop\IronRain\Assets\Recordings\UnityCapture\RecordStop.bat""";

	[SerializeField] private int _videoTime = 20;
	[SerializeField] private UnityCaptures _unityCaptures;

	/// <summary>
	///     Batファイルを実行
	/// </summary>
	/// <param name="batFile">BatファイルのURL</param>
	/// <param name="isHide">パワーシェルを出すかどうか</param>
	public void PowerShellCommand(string batFile, bool isHide = true)
	{
		Process process = new();
		ProcessStartInfo startInfo = new();

		if (isHide)
		{
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
		}

		startInfo.FileName = batFile;
		startInfo.UseShellExecute = false;
		startInfo.RedirectStandardError = true;
		startInfo.CreateNoWindow = true;
		process.StartInfo = startInfo;
		process.Start();
		process.WaitForExit();
	}

	///<summary>録画開始</summary>
	public async void StartRecord()
	{
		_unityCaptures.ChangeIsCapture(true);
		PowerShellCommand(StartRecordBat);
		await Task.Delay(_videoTime * 1000);
		StopRecord();
		_unityCaptures.ChangeIsCapture(false);
	}

	///<summary>録画停止</summary>
	public void StopRecord()
	{
		PowerShellCommand(StopRecordBat);
	}
}
