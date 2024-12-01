using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2 MoveInput { get; private set; }
    public bool InteractInput { get; private set; }
    public bool ThrowInput { get; private set; }
    public bool DashInput { get; private set; }

    private InputAction _moveAction;
    private InputAction _interactAction;
    private InputAction _throwAction;
    private InputAction _dashAction;

    private PlayerInput _playerInput;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();
        SetUpInputActions();
    }

    private void SetUpInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _interactAction = _playerInput.actions["Interact"];
        _throwAction = _playerInput.actions["Throw"];
        _dashAction = _playerInput.actions["Dash"];

        if (_moveAction == null) Debug.LogError("Move action not found in PlayerInput actions!");
        if (_interactAction == null) Debug.LogError("Interact action not found in PlayerInput actions!");
        if (_throwAction == null) Debug.LogError("Throw action not found in PlayerInput actions!");
        if (_dashAction == null) Debug.LogError("Dash action not found in PlayerInput actions!");
    }

    private void OnEnable()
    {
        _moveAction?.Enable();
        _moveAction.performed += OnMovePerformed;
        _moveAction.canceled += OnMoveCanceled;

        _interactAction?.Enable();
        _interactAction.performed += OnInteractPerformed;
        _interactAction.canceled += OnInteractCanceled;

        _throwAction?.Enable();
        _throwAction.performed += OnThrowPerformed;
        _throwAction.canceled += OnThrowCanceled;

        _dashAction?.Enable();
        _dashAction.performed += OnDashPerformed;
        _dashAction.canceled += OnDashCanceled;
    }

    private void OnDisable()
    {
        _moveAction?.Disable();
        _moveAction.performed -= OnMovePerformed;
        _moveAction.canceled -= OnMoveCanceled;

        _interactAction?.Disable();
        _interactAction.performed -= OnInteractPerformed;
        _interactAction.canceled -= OnInteractCanceled;

        _throwAction?.Disable();
        _throwAction.performed -= OnThrowPerformed;
        _throwAction.canceled -= OnThrowCanceled;

        _dashAction?.Disable();
        _dashAction.performed -= OnDashPerformed;
        _dashAction.canceled -= OnDashCanceled;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        MoveInput = Vector2.zero;
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        InteractInput = true;
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        InteractInput = false;
    }

    private void OnThrowPerformed(InputAction.CallbackContext context)
    {
        ThrowInput = true;
    }

    private void OnThrowCanceled(InputAction.CallbackContext context)
    {
        ThrowInput = false;
    }

    private void OnDashPerformed(InputAction.CallbackContext context)
    {
        DashInput = true;
    }

    private void OnDashCanceled(InputAction.CallbackContext context)
    {
        DashInput = false;
    }
}
