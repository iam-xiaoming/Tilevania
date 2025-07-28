using UnityEngine;

public class PlayerMortality : MonoBehaviour
{
    [SerializeField] float deadKickSpeed = 7f;
    [SerializeField] float deadBouncing = 0.5f;
    [SerializeField] float timeExit = 2f;
    bool isAlive = true;
    bool bounced = false;
    Rigidbody2D myRigidbody2D;
    PlayerRespawn playerRespawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        myRigidbody2D = GetComponent<Rigidbody2D>();
        playerRespawn = FindFirstObjectByType<PlayerRespawn>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive)
        {
            timeExit -= Time.deltaTime;

            if (timeExit <= 0)
            {
                playerRespawn.isActiveRespawn = true;
                Destroy(gameObject);
            }

            if (bounced) { SetColliderTrigger(true); }
        }

    }

    void SetColliderTrigger(bool trigger)
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
        {
            collider.isTrigger = trigger;
        }
    }

    // This method called before Update method.
    void OnCollisionEnter2D(Collision2D other)
    {
        bool isTouchEnemies = other.gameObject.layer == LayerMask.NameToLayer("Enemies");
        bool isTouchHazard = other.gameObject.layer == LayerMask.NameToLayer("Hazard");

        if (isAlive && (isTouchEnemies || isTouchHazard))
        {
            isAlive = false;
            PhysicsMaterial2D deadMaterial = new PhysicsMaterial2D()
            {
                bounciness = deadBouncing
            };
            // deadMaterial.bounciness = deadBouncing;

            myRigidbody2D.sharedMaterial = deadMaterial;
            myRigidbody2D.linearVelocityY = deadKickSpeed;
        }
        else if (!isAlive)
        {
            bounced = true;
        }
    }

    public bool IsAlive
    {
        get { return isAlive; }
    }
}
