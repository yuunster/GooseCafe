using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    private static UserInput _instance;

    public static UserInput Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UserInput>();
            }
            return _instance;
        }
    }

    private PlayerControls _inputActions;
    private InputAction _moveAction;
    private InputAction _dashAction;
    private InputAction _throwAction;
    private InputAction _interactAction;

    private void Awake()
    {
        _inputActions = new PlayerControls();

        _moveAction = _inputActions.Player.Move;
        _dashAction = _inputActions.Player.Dash;
        _throwAction = _inputActions.Player.Throw;
        _interactAction = _inputActions.Player.Interact;
    }

    private void OnEnable()
    {
        _inputActions.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
    }

    // Movement input (Vector2 for WASD/arrow keys, etc.)
    public Vector2 GetMoveInput()
    {
        return _moveAction.ReadValue<Vector2>();
    }

    // Dash input
    public bool GetDashInput()
    {
        return _dashAction.WasPressedThisFrame();
    }

    // Throw input
    public bool GetThrowInput()
    {
        return _throwAction.WasPressedThisFrame();
    }

    // Interact input
    public bool GetInteractInput()
    {
        return _interactAction.WasPressedThisFrame();
    }
}
