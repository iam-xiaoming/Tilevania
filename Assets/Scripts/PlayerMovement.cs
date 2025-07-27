using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    #region Movement Settings
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 10f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] int maxJumps = 2;
    [SerializeField] CompositeCollider2D waterCollider;
    [SerializeField] float underWaterSpeed = 2f;
    [SerializeField] TimeCounter timeOnWaterCounter;
    #endregion

    #region State Tracking
    float gravityScaleAtStart; // Get initial gravity, see "Awake()" method.
    int jumpCount = 0;
    bool playerHasVelocity = false;
    bool isClimbing = false; // This variable is used to determine the whether player holding the ladder when falling down, see "ClimbLadder()" method.
    Vector2 moveInput; // This variable is used to get which direction of player, eg. (-1, 0)...
    bool hasJumped = false; // This variable is used for jumping while laddering.
    bool isUnderTheWaterState = false;
    #endregion

    #region Components
    Rigidbody2D playerRigidbody;
    Animator playerAnimator;
    CapsuleCollider2D myBodyCollider2D;
    BoxCollider2D myFeetCollider2D;
    PlayerMortality playerMortality;
    #endregion

    void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myFeetCollider2D = GetComponent<BoxCollider2D>();
        playerMortality = GetComponent<PlayerMortality>();
        gravityScaleAtStart = playerRigidbody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Die();

        if (!playerMortality.IsAlive) { return; }

        CheckPlyingState();
        Run();
        Flip();
        ClimbLadder();
    }

    void CheckPlyingState()
    {
        if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")) || myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder")) || myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            playerAnimator.SetBool("isFlying", false);
        }
    }

    void SetJumpState()
    {
        //  If player touches ground, set jumpCount = 0.
        bool isTouchingGround = myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool isTouchingWater = myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Water"));
        
        if (isUnderTheWaterState)
        {
            playerAnimator.SetBool("isFlying", true);
            jumpCount = maxJumps - 1;
        }
        else if (isTouchingGround || isTouchingWater)
        {
            playerAnimator.SetBool("isFlying", false);
            jumpCount = 0;
        }
        else if (!isTouchingGround && !isClimbing)
        {
            playerAnimator.SetBool("isFlying", true);
        }
        else if (isClimbing)
        {
            playerAnimator.SetBool("isFlying", false);
            jumpCount = maxJumps - 1; // If player is climbing, set jumpCount = maxJumps - 1. This expression just allows jump once when climbing ladder.
        }
    }

    void OnMove(InputValue value)
    {
        if (!playerMortality.IsAlive) { return; }
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!playerMortality.IsAlive) { return; }

        SetJumpState();

        // If jumpCount exceed maxJumps, do not do anything. We minus 1 because OnJump run before Update.
        if (jumpCount >= maxJumps)
        {
            return;
        }

        if (value.isPressed)
        {
            hasJumped = true;
            playerRigidbody.linearVelocityY += jumpSpeed;
            ++jumpCount;
        }
        else
        {
            hasJumped = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isUnderTheWaterState = false;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water"))
        {
            isUnderTheWaterState = true;
        }
    }

    void Run()
    {
        if (isUnderTheWaterState)
        {
            playerRigidbody.linearVelocityX = moveInput.x * underWaterSpeed;
            playerAnimator.speed = 0.5f;
        }
        else if (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            playerRigidbody.linearVelocityX = moveInput.x * (runSpeed - 2);
            bool isVelocity = Mathf.Abs(playerRigidbody.linearVelocityX) > 0;

            if (isVelocity) { timeOnWaterCounter.Reset(); }

            if (timeOnWaterCounter.GetInterval() < 0.2f)
            {
                waterCollider.isTrigger = false;
                playerAnimator.speed = 1.5f;
            }
            else
            {
                waterCollider.isTrigger = true;
            }
        }
        else
        {
            playerRigidbody.linearVelocityX = moveInput.x * runSpeed;
            playerAnimator.speed = 1f;
        }

        if (playerHasVelocity && (myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")) || myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Water"))))
        {
            playerAnimator.SetBool("isRunning", true);
        }
        else
        {
            playerAnimator.SetBool("isRunning", false);
        }
    }

    void Flip()
    {
        playerHasVelocity = Mathf.Abs(playerRigidbody.linearVelocityX) > Mathf.Epsilon;
        if (playerHasVelocity)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidbody.linearVelocityX), 1f);
        }
    }

    void ClimbLadder()
    {
        bool isTouchingLadder = myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Ladder"));
        
        // When the player doesn't touch the ladder or has jumped when laddering, then set every parameters to default.
        if (!isTouchingLadder || hasJumped)
        {
            isClimbing = false;
            hasJumped = false;
            playerRigidbody.gravityScale = gravityScaleAtStart;
            playerAnimator.SetBool("isClimbing", false);
            playerAnimator.SetBool("isIdlingOnLadder", false);
            return;
        }

        bool isTouchingGround = myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));
        
        // If velocity > epsilon, means the player is climbing, otherwise.
        bool hasClimbingVelocity = Mathf.Abs(playerRigidbody.linearVelocityY) > Mathf.Epsilon;

        // To know when the player starts climbing, check if user holding "W" or "upper arrow".
        if (moveInput.y > Mathf.Epsilon) { isClimbing = true; }

        // This condition means when player goes through the ladder, we don't do anything.
        if (isTouchingLadder && !isClimbing) { return; }

        playerRigidbody.linearVelocityY = moveInput.y * climbSpeed;
        playerRigidbody.gravityScale = 0;

        // If hasClimbingVelocity = true, play climbing animation, otherwise play idling on ladder's animation with a condition the player doesn't touch the ground.
        if (!isTouchingGround)
        {
            playerAnimator.SetBool("isClimbing", hasClimbingVelocity);
            playerAnimator.SetBool("isIdlingOnLadder", !hasClimbingVelocity);
        }
        else
        {
            // If touch ground, we stop playing climbing animation.
            playerAnimator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (!playerMortality.IsAlive)
        {
            playerAnimator.SetTrigger("isDead");
        }
    }

    public Vector3 GetLocalScale()
    {
        return gameObject.transform.localScale;
    }
} 
