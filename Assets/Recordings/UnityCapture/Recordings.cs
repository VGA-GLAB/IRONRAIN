using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// パワーシェルでコマンドを実行する機能
/// </summary>
public class Recordings : MonoBehaviour
{
    const string StartRecordBat = @"""C:\Users\vantan\Desktop\IronRain\Assets\Recordings\UnityCapture\RecordStart.bat""";
    const string StopRecordBat = @"""C:\Users\vantan\Desktop\IronRain\Assets\Recordings\UnityCapture\RecordStop.bat""";

    private Thread _startProcess = null;
    Process[] processes;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_startProcess != null && _startProcess.IsAlive)
            {
                UnityEngine.Debug.Log("Already running...");
                return;
            }

            processes = Process.GetProcesses();
            foreach (Process process in processes)
            {
                if (process.ProcessName == "obs64")
                {
                    return;
                }
            }

            _startProcess = new Thread(StartRecord);
            _startProcess.Start();
            UnityEngine.Debug.Log("CaptureStart");
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Thread thread = new Thread(StopRecord);
            thread.Start();
            UnityEngine.Debug.Log("CaptureStop");
        }
    }

    /// <summary>
    /// AWSのS3に動画をアップロード機能
    /// </summary>
    /// <param name="action">終了時に実行したい処理があれば</param>
    /// <param name="isHide">パワーシェルを出すかどうか</param>
    public void PowerShellCommand(string batFile, bool isHide = true)
    {
        System.Diagnostics.Process process = new System.Diagnostics.Process();
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

        if (isHide) startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
        startInfo.FileName = batFile;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardError = true;
        startInfo.CreateNoWindow = true;
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();

        UnityEngine.Debug.Log(process.StandardError.ReadLine());
    }

    ///<summary>録画開始</summary>
    public void StartRecord()
    {
       // float timer = ;
        PowerShellCommand(StartRecordBat);
    }

    ///<summary>録画停止</summary>
    public void StopRecord()
    {
        PowerShellCommand(StopRecordBat);

    }
}
