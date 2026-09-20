using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMoveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private CartInteractor cartRider;

    [Header("Gun")]
    [SerializeField] private PlayerGun gun;

    [Header("Punch")]
    [SerializeField] private PlayerPunch punch;

    [Header("Runtime")]
    public bool LockMove = false;
    public bool LockRotation = false;
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isSprinting;

    void OnEnable()
    {
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnSprint += OnSprint;
        InputManager.Instance.OnAim += OnAim;
        InputManager.Instance.OnAttack += OnAttack;
    }

    void OnDisable()
    {
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnSprint -= OnSprint;
        InputManager.Instance.OnAim -= OnAim;
        InputManager.Instance.OnAttack -= OnAttack;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isSprinting = true;
        }
        if (ctx.canceled)
        {
            isSprinting = false;
        }
    }
    void OnAim(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            gun.StartAiming();


        }
        if (ctx.canceled)
        {
            gun.CancelAiming();
        }
    }

    void OnAttack(InputAction.CallbackContext ctx)
    {
        if (gun.IsAiming)
        {
            if (ctx.started)
            {
                gun.Shoot();
            }
        }
        else
        {
            // 펀치
            if (ctx.started)
            {
                punch.StartCharging();
            }
            else if (ctx.canceled)
            {
                punch.ReleasePunch();
            }
        }
    }


    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Transform cameraTr = CameraManager.Instance.CameraTr;

        // 카메라 방향
        Vector3 camForward = cameraTr.forward;
        Vector3 camRight = cameraTr.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        // 이동 속도
        currentSpeed = moveSpeed;

        if (gun.IsAiming) // 총 사용 중이면
        {
            currentSpeed = gun.AimMoveSpeed;
        }
        else if (punch.UsePunching) // 펀치 사용 중이면
        {
            currentSpeed = punch.PunchMoveSpeed;
        }
        else if (isSprinting) // 아무것도 아니면 달리기 적용
        {
            currentSpeed = sprintMoveSpeed;
        }


        Vector3 dir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        if (LockMove == false)
        {
            // 이동
            rb.linearVelocity = new Vector3(dir.x * currentSpeed, rb.linearVelocity.y, dir.z * currentSpeed);
        }

        if (LockRotation == false)
        {
            // 회전
            if (gun.IsAiming || punch.UsePunching)
            {
                // 카메라 방향을 바라봄
                if (camForward != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(camForward);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                }
            }
            else
            {
                // 이동 방향을 바라봄
                if (dir != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}
