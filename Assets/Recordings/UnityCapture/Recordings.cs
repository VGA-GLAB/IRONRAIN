using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;

/// <summary>
///     パワーシェルでコマンドを実行する機能
/// </summary>
public class Recordings : MonoBehaviour
{
	private const string StartRecordBat =
		@""".\Assets\Recordings\UnityCapture\RecordStart.bat""";

	private const string StopRecordBat =
		@""".\Assets\Recordings\UnityCapture\RecordStop.bat""";

	[SerializeField] private UnityCaptures _unityCaptures;
	private Thread _thread;
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
	public void StartRecord()
	{
		_unityCaptures.ChangeIsCapture(true);
		_thread = new Thread(()=>PowerShellCommand(StartRecordBat));
		_thread.Start();
		Debug.Log("録画開始");
	}

	///<summary>録画停止</summary>
	public void StopRecord()
	{
		_thread = new Thread(()=>PowerShellCommand(StopRecordBat));
		_thread.Start();
		_unityCaptures.ChangeIsCapture(false);
		Debug.Log("録画停止");
	}
}
