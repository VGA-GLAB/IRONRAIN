using System;
using System.IO;
using UnityEngine;

/// <summary>
/// パワーシェルでコマンドを実行する機能
/// </summary>
public class Recordings : MonoBehaviour
{
	const string StartRecordBat = @""".\Assets\Recording\Editor\RecordStart.bat""";
	const string StopRecordBat = @""".\Assets\Recording\Editor\RecordStop.bat""";


	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Slash))
		{
			StartRecord();
		}

		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			StopRecord();
		}
	}

	/// <summary>
	/// AWSのS3に動画をアップロード機能
	/// </summary>
	/// <param name="action">終了時に実行したい処理があれば</param>
	/// <param name="isHide">パワーシェルを出すかどうか</param>
	public static void PowerShellCommand(string batFile, bool isHide = true)
	{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

		if (isHide) startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = batFile;
		process.StartInfo = startInfo;
		startInfo.RedirectStandardError = true;
		process.Start();
		process.WaitForExit();
		Debug.Log(process.StandardError.ReadLine());
	}

	///<summary>録画開始</summary>
	public static void StartRecord()
	{
		PowerShellCommand(StartRecordBat);
	}

	///<summary>録画停止</summary>
	public static void StopRecord()
	{
		PowerShellCommand(StopRecordBat);
	}
}
