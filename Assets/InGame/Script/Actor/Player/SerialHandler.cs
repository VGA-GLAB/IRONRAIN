using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using Cysharp.Threading.Tasks;
public class SerialHandler
{
    public event Action<string> OnDataReceived;

    private string _portName = "COM6";
    private int _baudRate = 9600;
    [Tooltip("タイムアウトする秒数")]
    private const float _timeoutDuration = 2.0f;

    private SerialPort _serialPort;
    private Thread _thread;
    private bool _isRunning = false;

    private string _message;
    private bool _isNewMessageReceived = false;

    private float _timeoutTimer;
    private bool _isRead = false;


    public SerialHandler() 
    {

    }

    ~SerialHandler() 
    {
        Close();
    }

    public void Start(string portName) 
    {
        _portName = portName;
        Open();
        Update().Forget();
        Debug.Log("Start");
    }

    private async UniTask Update()
    {
        while (true)
        {
            if (_isNewMessageReceived)
            {
                Debug.Log(_message);
                OnDataReceived?.Invoke(_message);
            }
            _isNewMessageReceived = false;

            //タイマー機能を追加
            if (_isRead)
            {
                _timeoutTimer += Time.deltaTime;
                if (_timeoutTimer >= _timeoutDuration)
                {
                    Close();
                    _isRead = false;
                }//タイムアウトする秒数
            }

            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    private void Open()
    {
        _serialPort = new SerialPort(_portName, _baudRate, System.IO.Ports.Parity.None, 8, System.IO.Ports.StopBits.One);

        _serialPort.Open();

        _isRunning = true;

        _thread = new Thread(Read);
        _thread.Start();
    }

    private void Close()
    {
        _isNewMessageReceived = false;
        _isRunning = false;

        if (_thread != null && _thread.IsAlive)
        {
            _thread.Join();
        }

        if (_serialPort != null && _serialPort.IsOpen)
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }

    private void Read()
    {
        while (_isRunning && _serialPort != null && _serialPort.IsOpen)
        {
            try
            {
                _message = _serialPort.ReadLine();
                _isNewMessageReceived = true;

                //タイマーフラグ
                _timeoutTimer = 0;
                _isRead = true;
            }
            catch (System.Exception e)
            {
                Debug.LogWarning(e.Message);
            }
        }
    }

    public void Write(string message)
    {
        try
        {
            //_serialPort.Write(message);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
}
