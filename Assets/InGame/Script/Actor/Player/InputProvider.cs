//���{��Ή�
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;
using Cysharp.Threading.Tasks;

/// <summary>
/// Input�̏���񋟂���N���X
/// </summary>
public class InputProvider
{
    /// <summary>�E���o�[�̌���</summary>
    public Vector3 RightLeverDir { get; set; }
    /// <summary>�����o�[�̌���</summary>
    public Vector3 LeftLeverDir { get; set; }
    /// <summary>�E���o�[�ɉ�����Ă�����͂̕���</summary>
    public Vector3 RightLeverInputDir { get; private set; }
    /// <summary>�����o�[�ɉ�����Ă�����͂̕���</summary>
    public Vector3 LeftLeverInputDir { get; private set; }
    /// <summary>�E���o�[���������邩</summary>
    public bool IsRightLeverMove { get; private set; }    
    /// <summary>�����o�[���������邩</summary>
    public bool IsLeftLeverMove { get; private set; }
    public static InputProvider Instance => _instance;

    [Tooltip("InputSystem�Ő��������N���X")]
    private XRIDefaultInputActions _inputMap;
    [Tooltip("���͒���")]
    private Dictionary<InputType, Action> _onEnterInputDic = new Dictionary<InputType, Action>();
    [Tooltip("���͒���(Async)")]
    private Dictionary<InputType, Func<UniTaskVoid>> _onEnterInputAsyncDic = new Dictionary<InputType, Func<UniTaskVoid>>();
    [Tooltip("���͉���")]
    private Dictionary<InputType, Action> _onExitInputDic = new Dictionary<InputType, Action>();
    [Tooltip("���͒���(Async)")]
    private Dictionary<InputType, Func<UniTaskVoid>> _onExitInputAsyncDic = new Dictionary<InputType, Func<UniTaskVoid>>();
    [Tooltip("���͒�")]
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
    /// ����������
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
    /// ���͏����̏��������s��
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
    /// ���͊J�n���͉��������Ƃ��ɌĂ΂��֐�
    /// </summary>
    /// <param name="input"></param>
    private void ExecuteInput(InputType input, InputMode type)
    {
        switch (type)
        {
            case InputMode.Enter:
                //���͊J�n���������s����
                SetStayInput(input, true);
                _onEnterInputDic[input]?.Invoke();
                _onEnterInputAsyncDic[input]?.Invoke();
                break;
            case InputMode.Exit:
                // ���͉������������s����
                SetStayInput(input, false);
                _onExitInputDic[input]?.Invoke();
                _onExitInputAsyncDic[input]?.Invoke();
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// ����InputType�����͒����ǂ����t���O��Ԃ�
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public bool GetStayInput(InputType type)
    {
        return _isStayInputDic[type];
    }

    /// <summary>
    ///����̓��͂ŌĂяo��Action��o�^����
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
    /// ����̓��͂��Ăяo��
    /// </summary>
    public void CallEnterInput(InputType inputType) 
    {
        ExecuteInput(inputType, InputMode.Enter);
    }

    /// <summary>
    /// ����̓��͉������Ăяo��
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
    ///����̓��͏I��������ɌĂяo��Action��o�^����
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
    /// ����̓��͂ŌĂяo�����o�^����Action���폜����
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
    ///����̓��͏I��������ɌĂяo�����o�^����Action���폜����
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
    /// ���͂̃^�C�~���O
    /// </summary>
    public enum InputMode
    {
        /// <summary>���͎�</summary>
        Enter,
        /// <summary>���͏I����</summary>
        Exit,
    }

    /// <summary>
    /// ���͂̎��
    /// </summary>
    public enum InputType
    {
        /// <summary>�ˌ�</summary>
        Shot,
        /// <summary>����؂�ւ�</summary>
        WeaponChenge,
        /// <summary>�E���o�[�������Ă��邩</summary>
        RightLeverMove,     
        /// <summary>�����o�[�������Ă��邩</summary>
        LeftLeverMove,
        /// <summary>�E�{�^������1</summary>
        RightButton1,
        /// <summary>�E�{�^������2</summary>
        RightButton2,
    }
}
