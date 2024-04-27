//日本語対応
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// Inputの情報を提供するクラス
/// </summary>
public class InputProvider
{
    /// <summary>右レバーの向き</summary>
    public Vector3 RightLeverDir { get; set; }
    /// <summary>左レバーの向き</summary>
    public Vector3 LeftLeverDir { get; set; }
    /// <summary>右レバーに加わっている入力の方向</summary>
    public Vector3 RightLeverInputDir { get; private set; }
    /// <summary>左レバーに加わっている入力の方向</summary>
    public Vector3 LeftLeverInputDir { get; private set; }
    /// <summary>右レバーが動かせるか</summary>
    public bool IsRightLeverMove { get; private set; }    
    /// <summary>左レバーが動かせるか</summary>
    public bool IsLeftLeverMove { get; private set; }
    public static InputProvider Instance => _instance;

    [Tooltip("InputSystemで生成したクラス")]
    private XRIDefaultInputActions _inputMap;
    [Tooltip("入力直後")]
    private Dictionary<InputType, Action> _onEnterInputDic = new Dictionary<InputType, Action>();
    [Tooltip("入力直後(Async)")]
    private Dictionary<InputType, Func<UniTaskVoid>> _onEnterInputAsyncDic = new Dictionary<InputType, Func<UniTaskVoid>>();
    [Tooltip("入力解除")]
    private Dictionary<InputType, Action> _onExitInputDic = new Dictionary<InputType, Action>();
    [Tooltip("入力直後(Async)")]
    private Dictionary<InputType, Func<UniTaskVoid>> _onExitInputAsyncDic = new Dictionary<InputType, Func<UniTaskVoid>>();
    [Tooltip("入力中")]
    private Dictionary<InputType, bool> _isStayInputDic = new Dictionary<InputType, bool>();

    private bool _isInstanced = false;
    private static InputProvider _instance = new InputProvider();

    public InputProvider()
    {
        Initialize();
    }

    ~InputProvider() 
    {
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        _inputMap = new XRIDefaultInputActions();
        _inputMap.Enable();
        InirializeInput();
        _inputMap.Lever.Arrow.performed += context => RightLeverInputDir = context.ReadValue<Vector2>();
        _inputMap.Lever.Arrow.canceled += context => RightLeverInputDir = Vector2.zero;
        _inputMap.Lever.WASD.performed += context => LeftLeverInputDir = context.ReadValue<Vector2>();
        _inputMap.Lever.WASD.canceled += context => LeftLeverInputDir = Vector2.zero;
        _inputMap.Lever.RightButton.performed += context =>
        {
            var dir = context.ReadValue<Vector2>();
            if (dir.x == 1)
            {
                Debug.Log("Right2");
                ExecuteInput(InputType.RightButton2, InputMode.Enter);
            }
            else if(dir.x == -1)
            {
                Debug.Log("Right1");
                ExecuteInput(InputType.RightButton1, InputMode.Enter);
            }
        };
        _inputMap.Lever.RightButton.canceled += context =>
        {
            var dir = context.ReadValue<Vector2>();
            if (dir.x == 1)
            {
                ExecuteInput(InputType.RightButton2, InputMode.Exit);
            }
            else if (dir.x == -1)
            {
                ExecuteInput(InputType.RightButton1, InputMode.Exit);
            }
        };
        _inputMap.XRIRightHandInteraction.Select.performed += context => IsRightLeverMove = true; 
        _inputMap.XRIRightHandInteraction.Select.canceled += context => IsRightLeverMove = false;
        _inputMap.XRILeftHandInteraction.Select.performed += context => IsLeftLeverMove = true;
        _inputMap.XRILeftHandInteraction.Select.canceled += context => IsLeftLeverMove = false;
        _inputMap.XRIRightHandInteraction.Activate.performed += context => ExecuteInput(InputType.Shot, InputMode.Enter);
        _inputMap.XRIRightHandInteraction.Activate.canceled += context => ExecuteInput(InputType.Shot, InputMode.Exit);
        _inputMap.XRIRightHandInteraction.ActivateValue.performed += context => ExecuteInput(InputType.WeaponChenge, InputMode.Enter);
        _inputMap.XRIRightHandInteraction.ActivateValue.canceled += context => ExecuteInput(InputType.WeaponChenge, InputMode.Exit);
        _inputMap.XRILeftHandInteraction.Select.performed += context => ExecuteInput(InputType.LeftLeverMove, InputMode.Enter);
        _inputMap.XRILeftHandInteraction.Select.canceled += context => ExecuteInput(InputType.LeftLeverMove, InputMode.Exit);
        _inputMap.XRIRightHandInteraction.Select.performed += context => ExecuteInput(InputType.RightLeverMove, InputMode.Enter);
        _inputMap.XRIRightHandInteraction.Select.canceled += context => ExecuteInput(InputType.RightLeverMove, InputMode.Exit);

        _isInstanced = true;
    }

    /// <summary>
    /// 入力処理の初期化を行う
    /// </summary>
    private void InirializeInput()
    {
        if (_isInstanced)
        {
            for (int i = 0; i < Enum.GetValues(typeof(InputType)).Length; i++)
            {
                _onEnterInputDic[(InputType)i] = null;
                _onEnterInputAsyncDic[(InputType)i] = null;
                _onExitInputDic[(InputType)i] = null;
                _onExitInputAsyncDic[(InputType)i] = null;
                _isStayInputDic[(InputType)i] = false;
            }
            return;
        }
        for (int i = 0; i < Enum.GetValues(typeof(InputType)).Length; i++)
        {
            _onEnterInputDic.Add((InputType)i, null);
            _onEnterInputAsyncDic.Add((InputType)i, null);
            _onExitInputDic.Add((InputType)i, null);
            _onExitInputAsyncDic.Add((InputType)i, null);
            _isStayInputDic.Add((InputType)i, false);
        }
    }

    /// <summary>
    /// 入力開始入力解除したときに呼ばれる関数
    /// </summary>
    /// <param name="input"></param>
    private void ExecuteInput(InputType input, InputMode type)
    {
        switch (type)
        {
            case InputMode.Enter:
                //入力開始処理を実行する
                SetStayInput(input, true);
                _onEnterInputDic[input]?.Invoke();
                _onEnterInputAsyncDic[input]?.Invoke();
                break;
            case InputMode.Exit:
                // 入力解除処理を実行する
                SetStayInput(input, false);
                _onExitInputDic[input]?.Invoke();
                _onExitInputAsyncDic[input]?.Invoke();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// そのInputTypeが入力中かどうかフラグを返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool GetStayInput(InputType type)
    {
        return _isStayInputDic[type];
    }

    /// <summary>
    ///特定の入力で呼び出すActionを登録する
    /// </summary>
    public void SetEnterInput(InputType type, Action action)
    {
        _onEnterInputDic[type] += action;
    }

    public void SetEnterInputAsync(InputType type, Func<UniTaskVoid> func)
    {
        _onEnterInputAsyncDic[type] += func;
    }

    /// <summary>
    /// 特定の入力を呼び出す
    /// </summary>
    public void CallEnterInput(InputType inputType) 
    {
        ExecuteInput(inputType, InputMode.Enter);
    }

    /// <summary>
    /// 特定の入力解除を呼び出す
    /// </summary>
    public void CallExitInput(InputType inputType)
    {
        ExecuteInput(inputType, InputMode.Exit);
    }

    private void SetStayInput(InputType type, bool isBool)
    {
        _isStayInputDic[type] = isBool;
    }

    /// <summary>
    ///特定の入力終わった時に呼び出すActionを登録する
    /// </summary>
    public void SetExitInput(InputType type, Action action)
    {
        _onExitInputDic[type] += action;
    }

    public void SetExitInputAsync(InputType type, Func<UniTaskVoid> func)
    {
        _onExitInputAsyncDic[type] += func;
    }

    /// <summary>
    /// 特定の入力で呼び出される登録したActionを削除する
    /// </summary>
    public void LiftEnterInput(InputType type, Action action)
    {
        _onEnterInputDic[type] -= action;
    }

    public void LiftEnterInputAsync(InputType type, Func<UniTaskVoid> func)
    {
        _onEnterInputAsyncDic[type] -= func;
    }

    /// <summary>
    ///特定の入力終わった時に呼び出される登録したActionを削除する
    /// </summary>
    public void LiftExitInput(InputType type, Action action)
    {
        _onExitInputDic[type] -= action;
    }

    public void LiftExitInputAsync(InputType type, Func<UniTaskVoid> func)
    {
        _onExitInputAsyncDic[type] -= func;
    }


    /// <summary>
    /// 入力のタイミング
    /// </summary>
    public enum InputMode
    {
        /// <summary>入力時</summary>
        Enter,
        /// <summary>入力終了時</summary>
        Exit,
    }

    /// <summary>
    /// 入力の種類
    /// </summary>
    public enum InputType
    {
        /// <summary>射撃</summary>
        Shot,
        /// <summary>武器切り替え</summary>
        WeaponChenge,
        /// <summary>右レバーが動いているか</summary>
        RightLeverMove,     
        /// <summary>左レバーが動いているか</summary>
        LeftLeverMove,
        /// <summary>右ボタンその1</summary>
        RightButton1,
        /// <summary>右ボタンその2</summary>
        RightButton2,
    }
}
