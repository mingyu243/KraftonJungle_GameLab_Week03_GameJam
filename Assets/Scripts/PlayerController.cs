using Unity.Cinemachine;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform cameraTr;
    [SerializeField] private CinemachineCamera normalCamera;
    [SerializeField] private CinemachineCamera aimCamera;

    [Header("Move")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private CartRider cartRider;

    [Header("Runtime")]
    [SerializeField] private Vector2 moveInput;
    [SerializeField] private bool isSprinting;

    void OnEnable()
    {
        InputManager.Instance.OnMove += OnMove;
        InputManager.Instance.OnSprint += OnSprint;
    }

    void OnDisable()
    {
        InputManager.Instance.OnMove -= OnMove;
        InputManager.Instance.OnSprint -= OnSprint;
    }

    void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }
    void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.started) isSprinting = true;
        if (ctx.canceled) isSprinting = false;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 카메라 방향
        Vector3 camForward = cameraTr.forward;
        Vector3 camRight = cameraTr.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 dir = (camForward * moveInput.y + camRight * moveInput.x).normalized;

        // 이동
        float currentSpeed = isSprinting ? sprintSpeed : moveSpeed;
        rb.linearVelocity = new Vector3(dir.x * currentSpeed, rb.linearVelocity.y, dir.z * currentSpeed);

        // 회전
        if (dir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    // 조준 시작
    public void EnterAim()
    {

    }

    // 조준 끝
    public void ExitAim()
    {

    }

    // 쏘기
    public void Shoot()
    {

    }


    // 밀치기
    public void Push()
    {

    }
}
