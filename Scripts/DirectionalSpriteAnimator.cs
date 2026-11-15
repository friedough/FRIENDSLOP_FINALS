
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DirectionalSpriteAnimator : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Animation Settings")]
    public float framesPerSecond = 8f;

    [Header("Forward Animations")]
    public Sprite[] forwardIdle;
    public Sprite[] forwardWalk;
    public Sprite[] forwardCarryIdle;
    public Sprite[] forwardCarryWalk;

    [Header("Backward Animations")]
    public Sprite[] backwardIdle;
    public Sprite[] backwardWalk;
    public Sprite[] backwardCarryIdle;
    public Sprite[] backwardCarryWalk;

    [Header("Left Animations")]
    public Sprite[] leftIdle;
    public Sprite[] leftWalk;
    public Sprite[] leftCarryIdle;
    public Sprite[] leftCarryWalk;

    [Header("Right Animations")]
    public Sprite[] rightIdle;
    public Sprite[] rightWalk;
    public Sprite[] rightCarryIdle;
    public Sprite[] rightCarryWalk;

    private SpriteRenderer spriteRenderer;

    private Sprite[] currentAnimation;
    private int currentFrame;
    private float frameTimer;

    private bool isWalking;
    private bool isCarrying;

    private enum Direction
    {
        Forward,
        Backward,
        Left,
        Right
    }

    private Direction currentDirection = Direction.Forward;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        UpdateCurrentAnimation();
    }

    private void Update()
    {
        HandleAnimation();
        AnimateSprite();
    }

    private void LateUpdate()
    {
        HandleBillboard();
    }

    private void HandleAnimation()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        isWalking = Mathf.Abs(horizontal) > 0.1f ||
                    Mathf.Abs(vertical) > 0.1f;

        if (isWalking)
        {
            if (Mathf.Abs(horizontal) > Mathf.Abs(vertical))
            {
                currentDirection = horizontal > 0
                    ? Direction.Right
                    : Direction.Left;
            }
            else
            {
                currentDirection = vertical > 0
                    ? Direction.Forward
                    : Direction.Backward;
            }
        }

        UpdateCurrentAnimation();
    }

    private void UpdateCurrentAnimation()
    {
        Sprite[] selectedAnimation = null;

        //MAKE IT READ THE CARRYING SPRITES FIRST
        if (isCarrying)
        {
            switch (currentDirection)
            {
                case Direction.Forward:
                    selectedAnimation = isWalking
                        ? forwardCarryWalk
                        : forwardCarryIdle;
                    break;

                case Direction.Backward:
                    selectedAnimation = isWalking
                        ? backwardCarryWalk
                        : backwardCarryIdle;
                    break;

                case Direction.Left:
                    selectedAnimation = isWalking
                        ? leftCarryWalk
                        : leftCarryIdle;
                    break;

                case Direction.Right:
                    selectedAnimation = isWalking
                        ? rightCarryWalk
                        : rightCarryIdle;
                    break;
            }
        }

        // GOES BACK TO NORMAL IF CARRYING IS NOT ASSIGNED
        if (selectedAnimation == null ||
            selectedAnimation.Length == 0)
        {
            switch (currentDirection)
            {
                case Direction.Forward:
                    selectedAnimation = isWalking
                        ? forwardWalk : forwardIdle;
                    break;

                case Direction.Backward:
                    selectedAnimation = isWalking
                        ? backwardWalk : backwardIdle;
                    break;

                case Direction.Left:
                    selectedAnimation = isWalking
                        ? leftWalk : leftIdle;
                    break;

                case Direction.Right:
                    selectedAnimation = isWalking
                        ? rightWalk : rightIdle;
                    break;
            }
        }

        SetAnimation(selectedAnimation);
    }

    private void SetAnimation(Sprite[] newAnimation)
    {
        if (newAnimation == null || newAnimation.Length == 0)
            return;

        if (currentAnimation == newAnimation)
            return;

        currentAnimation = newAnimation;

        currentFrame = 0;
        frameTimer = 0f;

        spriteRenderer.sprite = currentAnimation[currentFrame];
    }

    private void AnimateSprite()
    {
        if (currentAnimation == null ||
            currentAnimation.Length == 0)
            return;

        if (currentAnimation.Length == 1)
            return;

        frameTimer += Time.deltaTime;

        float frameDuration = 1f /
            Mathf.Max(framesPerSecond, 0.01f);

        if (frameTimer >= frameDuration)
        {
            frameTimer -= frameDuration;

            currentFrame++;

            if (currentFrame >= currentAnimation.Length)
                currentFrame = 0;

            spriteRenderer.sprite =
                currentAnimation[currentFrame];
        }
    }

    private void HandleBillboard()
    {
        if (cameraTransform == null)
            return;

        Vector3 direction =
            cameraTransform.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    public void SetCarrying(bool value)
    {
        if (isCarrying == value)
            return;

        isCarrying = value;

        UpdateCurrentAnimation();
    }
}
