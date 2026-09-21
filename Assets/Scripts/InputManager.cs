using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private InputSystem_Actions inputActions;

    public event Action<InputAction.CallbackContext> OnMove;
    public event Action<InputAction.CallbackContext> OnSprint;
    public event Action<InputAction.CallbackContext> OnAim;
    public event Action<InputAction.CallbackContext> OnAttack;
    public event Action<InputAction.CallbackContext> OnSubWeapon;
    public event Action<InputAction.CallbackContext> OnInventory;

    void Awake()
    {
        Instance = this;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.started += Move;
        inputActions.Player.Move.performed += Move;
        inputActions.Player.Move.canceled += Move;

        inputActions.Player.Sprint.started += Sprint;
        inputActions.Player.Sprint.canceled += Sprint;

        inputActions.Player.Aim.started += Aim;
        inputActions.Player.Aim.canceled += Aim;

        inputActions.Player.Attack.started += Attack;
        inputActions.Player.Attack.canceled += Attack;

        inputActions.Player.Inventory.started += Inventory;
        inputActions.Player.Inventory.canceled += Inventory;
    }

    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Player.Move.started -= Move;
        inputActions.Player.Move.performed -= Move;
        inputActions.Player.Move.canceled -= Move;

        inputActions.Player.Sprint.started -= Sprint;
        inputActions.Player.Sprint.canceled -= Sprint;

        inputActions.Player.Aim.started -= Aim;
        inputActions.Player.Aim.canceled -= Aim;

        inputActions.Player.Attack.started -= Attack;
        inputActions.Player.Attack.canceled -= Attack;

        inputActions.Player.Inventory.started -= Inventory;
        inputActions.Player.Inventory.canceled -= Inventory;
    }

    private void Move(InputAction.CallbackContext ctx) => OnMove?.Invoke(ctx);
    private void Sprint(InputAction.CallbackContext ctx) => OnSprint?.Invoke(ctx);
    private void Aim(InputAction.CallbackContext ctx) => OnAim?.Invoke(ctx);
    private void Attack(InputAction.CallbackContext ctx) => OnAttack?.Invoke(ctx);
    private void Inventory(InputAction.CallbackContext ctx) => OnInventory?.Invoke(ctx);
}
