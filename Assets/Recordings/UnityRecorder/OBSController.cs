//using System.Net.WebSockets;
//using UnityEngine;

//public class OBSController : MonoBehaviour
//{
//    private WebSocket ws;

//    void Start()
//    {
//        // WebSocketの接続先をOBSのWebSocketサーバーに設定
//        ws = new WebSocket("ws://localhost:4455"); // デフォルトのポートは4455です
//        ws.OnMessage += (sender, e) =>
//        {
//            Debug.Log("Message from OBS: " + e.Data);
//        };
//        ws.Connect();

//        // パスワードが必要な場合は認証を行います
//        AuthenticateWithOBS("your_password_here");
//    }

//    // OBSに認証を行う
//    void AuthenticateWithOBS(string password)
//    {
//        var authMessage = new
//        {
//            op = 1,  // Authenticate Request
//            d = new
//            {
//                rpcVersion = 1, // RPCバージョン
//                authentication = password
//            }
//        };
//        ws.Send(JsonUtility.ToJson(authMessage));
//    }

//    // 録画開始
//    public void StartRecording()
//    {
//        var startRecordingMessage = new
//        {
//            op = 6,  // Start Recording
//            d = new { }
//        };
//        ws.Send(JsonUtility.ToJson(startRecordingMessage));
//        Debug.Log("Start recording command sent.");
//    }

//    // 録画終了
//    public void StopRecording()
//    {
//        var stopRecordingMessage = new
//        {
//            op = 7,  // Stop Recording
//            d = new { }
//        };
//        ws.Send(JsonUtility.ToJson(stopRecordingMessage));
//        Debug.Log("Stop recording command sent.");
//    }

//    void OnApplicationQuit()
//    {
//        // WebSocketの接続を閉じる
//        ws.Close();
//    }
//}
