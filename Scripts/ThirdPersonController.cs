
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;
    public float sprintSpeed = 8f;

    public float acceleration = 10f;
    public float deceleration = 15f;
    public float rotationSpeed = 12f;

    [Header("Jump Settings")]
    public float jumpHeight = 2f;
    public float gravity = -20f;

    [Header("Camera Settings")]
    public Transform cameraTransform;
    public Transform cameraTarget;

    public float mouseSensitivity = 2f;
    public float cameraDistance = 5f;
    public float cameraHeight = 0f;
    public float minCameraAngle = -30f;
    public float maxCameraAngle = 70f;
    public float cameraSmoothness = 12f;

    private CharacterController controller;

    private Vector3 currentVelocity;
    private float verticalVelocity;

    private float cameraYaw;
    private float cameraPitch;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        //LOCK CURSOR SHITRRRTT
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (cameraTransform != null)
        {
            Vector3 angles = cameraTransform.eulerAngles;

            cameraYaw = angles.y;
            cameraPitch = angles.x;

            if (cameraPitch > 180f)
                cameraPitch -= 360f;
        }
    }

    private void Update()
    {
        HandleCameraInput();
        HandleMovement();
        HandleJump();
    }

    private void LateUpdate()
    {
        HandleCameraFollow();
    }

    //CAMERA

    private void HandleCameraInput()
    {
        if (cameraTransform == null)
            return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        cameraYaw += mouseX;
        cameraPitch -= mouseY;

        cameraPitch = Mathf.Clamp(
            cameraPitch,
            minCameraAngle,
            maxCameraAngle
        );
    }

    //CAMERA FOLLOW

    private void HandleCameraFollow()
    {
        if (cameraTransform == null || cameraTarget == null)
            return;

        Quaternion cameraRotation = Quaternion.Euler(
            cameraPitch,
            cameraYaw,
            0f
        );

        Vector3 targetPosition = cameraTarget.position;

        targetPosition += Vector3.up * cameraHeight;

        Vector3 desiredPosition = targetPosition -
            cameraRotation * Vector3.forward * cameraDistance;

        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position,
            desiredPosition,
            cameraSmoothness * Time.deltaTime
        );

        cameraTransform.rotation = cameraRotation;
    }

    //PLAYER MOVEMENT

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        //MOVEMENT IN RELATION TO CAMERA
        Vector3 cameraForward = cameraTransform != null
            ? cameraTransform.forward
            : transform.forward;

        Vector3 cameraRight = cameraTransform != null
            ? cameraTransform.right
            : transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection =
            cameraForward * vertical +
            cameraRight * horizontal;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection, 1f
        );

        //SPEED
        float targetSpeed = walkSpeed;

        if (Input.GetKey(KeyCode.LeftControl))
        {
            targetSpeed = runSpeed;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            targetSpeed = sprintSpeed;
        }

        Vector3 targetVelocity = moveDirection * targetSpeed;

        //ACCEL AND DECEL GSDHFJKGSDF
        float smoothSpeed = moveDirection.magnitude > 0.1f
            ? acceleration
            : deceleration;

        currentVelocity = Vector3.MoveTowards(
            currentVelocity,
            targetVelocity,
            smoothSpeed * Time.deltaTime
        );

        // Rotate character towards movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(
                moveDirection
            );

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Apply horizontal movement
        controller.Move(
            currentVelocity * Time.deltaTime
        );
    }

    //JUMPING

    private void HandleJump()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (controller.isGrounded &&
            Input.GetKeyDown(KeyCode.Space))
        {
            verticalVelocity = Mathf.Sqrt(
                jumpHeight * -2f * gravity
            );
        }

        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(
            Vector3.up * verticalVelocity * Time.deltaTime
        );
    }

   //UNLOCK CURSOR
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}

