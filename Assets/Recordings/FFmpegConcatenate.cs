#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
#endif
using UnityEngine;

public class FFmpegConcatenate : MonoBehaviour
{
#if UNITY_EDITOR
    // 動画ファイルのディレクトリを指定
    private string videoFilesDirectory = @"C:\Users\vantan\Desktop\IronRain\Assets\Recordings\Videos";

    // 結合する動画ファイルのリスト
    private string[] videoFiles = { "Part 1.mp4", "Part 2.mp4", "Part 3.mp4" };

    public void ConcatenateVideos()
    {
        // ファイルリストを含むテキストファイルを作成
        string fileListPath = Path.Combine(videoFilesDirectory, "filelist.txt");
        using (StreamWriter sw = new StreamWriter(fileListPath))
        {
            foreach (string videoFile in videoFiles)
            {
                sw.WriteLine($"file '{Path.Combine(videoFilesDirectory, videoFile)}'");
            }
        }

        // FFmpegコマンドの準備
        string ffmpegPath = "ffmpeg"; // 環境変数に追加している場合
        string arguments = $@"-f concat -safe 0 -i ""{fileListPath}"" -c copy -y ""{Path.Combine(videoFilesDirectory, "output.mp4")}""";

        // FFmpegプロセスの開始
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            Arguments = arguments,
            RedirectStandardOutput = false,
            RedirectStandardError = false,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        using (Process process = new Process())
        {
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
        }

        AssetDatabase.Refresh();
    }

    // 例として、Unityのボタンから呼び出すメソッド
    public void Splice()
    {
        ConcatenateVideos();
    }
#endif
}