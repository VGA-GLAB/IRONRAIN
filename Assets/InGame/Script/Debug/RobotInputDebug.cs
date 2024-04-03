using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotInputDebug : MonoBehaviour
{
    [SerializeField] private PlayerController _robotController;
    [SerializeField] private GameObject _ui;
    [SerializeField] private Text _rightInput;
    [SerializeField] private Text _leftInput;
    [SerializeField] private Text _moveState;

    private PlayerMove _playerMove;
    private bool _active = false;

    private void Start()
    {
        _playerMove = _robotController.SeachState<PlayerMove>();
    }

    private void Update()
    {
        SetView();
    }

    public void Active()
    {
        _active = true;
        _ui.SetActive(true);
    }

    private void SetView()
    {
        if (!_active) return;

        _leftInput.text = _playerMove.MoveModel.Dir.x.ToString();
        _rightInput.text = _playerMove.MoveModel.Dir.y.ToString();

        //if (_robotController.MoveState == MoveState.Forward)
        //{
        //    _moveState.text = "ëOêi";
        //}
        //else if (_robotController.MoveState == MoveState.Back)
        //{
        //    _moveState.text = "å„ëﬁ";
        //}
        //else if (_robotController.MoveState == MoveState.Right)
        //{
        //    _moveState.text = "âEê˘âÒ";
        //}
        //else if (_robotController.MoveState == MoveState.Left)
        //{
        //    _moveState.text = "ç∂ê˘âÒ";
        //}
    }
}
