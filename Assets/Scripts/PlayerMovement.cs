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
    #endregion

    #region State Tracking
    float gravityScaleAtStart; // Get initial gravity, see "Awake()" method.
    int jumpCount = 0;
    bool playerHasVelocity = false;
    bool isClimbing = false; // This variable is used to determine the whether player holding the ladder when falling down, see "ClimbLadder()" method.
    Vector2 moveInput; // This variable is used to get which direction of player, eg. (-1, 0)...
    bool hasJumped = false; // This variable is used for jumping while laddering.
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

        Run();
        Flip();
        SetJumpState();
        ClimbLadder();
    }

    void SetJumpState()
    {
        //  If player touches ground, set jumpCount = 0.
        bool isTouchingGround = myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));
        
        if (isTouchingGround)
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
            jumpCount = maxJumps - 2; // If player is climbing, set jumpCount = maxJumps - 2. Actually -1, but because OnJump run before Update. This expression just allows jump once when climbing ladder.
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

        // If jumpCount exceed maxJumps, do not do anything. We minus 1 because OnJump run before Update.
        if (jumpCount >= maxJumps - 1) { return; }

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

    void Run()
    {
        playerRigidbody.linearVelocityX = moveInput.x * runSpeed;
        

        if (playerHasVelocity && myFeetCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground")))
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
} 
