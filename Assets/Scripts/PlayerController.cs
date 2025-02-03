using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputSystem_Actions _playerInput;

    public Action GoingToDryingRoomAction;
    public Action GoingToMainRoomAction;

    private static PlayerController _instance;
    public static PlayerController Instance
    {
        get
        {
            if (!_instance)
            {
                // NOTE: read docs to see directory requirements for Resources.Load!
                var prefab = Resources.Load<GameObject>("PlayerController");
                // create the prefab in your scene
                var inScene = Instantiate<GameObject>(prefab);
                // try find the instance inside the prefab
                _instance = inScene.GetComponentInChildren<PlayerController>();
                // guess there isn't one, add one
                if (!_instance)
                    _instance = inScene.AddComponent<PlayerController>();
                // mark root as DontDestroyOnLoad();
                DontDestroyOnLoad(_instance.transform.root.gameObject);
                _instance._playerInput = new InputSystem_Actions();
                _instance._playerInput.Enable();

                _instance._playerInput.Player.MoveToDryingRoom.performed += _instance.MoveToDryingRoom_performed;
                _instance._playerInput.Player.MoveToMainRoom.performed += _instance.MoveToMainRoom_performed;

            }
            return _instance;
        }
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
