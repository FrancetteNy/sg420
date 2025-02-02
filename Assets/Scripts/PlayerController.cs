using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    InputSystem_Actions _playerInput;

    public Action GoingToDryingRoomAction;
    public Action GoingToMainRoomAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;

        _playerInput = new InputSystem_Actions();
        _playerInput.Enable();

        _playerInput.Player.MoveToDryingRoom.performed += MoveToDryingRoom_performed;
        _playerInput.Player.MoveToMainRoom.performed += MoveToMainRoom_performed;
    }

    private void MoveToMainRoom_performed(InputAction.CallbackContext obj)
    {
        GoingToMainRoomAction?.Invoke();
    }

    private void MoveToDryingRoom_performed(InputAction.CallbackContext obj)
    {
        GoingToDryingRoomAction?.Invoke();
    }
}
