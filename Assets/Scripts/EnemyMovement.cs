using Unity.VisualScripting;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float onTimeDirection = 2f;
    [SerializeField] float knockbackForceTime = 0.5f;
    [SerializeField] float knockbackForce = 5f;
    float currentTime;
    float knockTime;
    Rigidbody2D myRigidbody2d;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRigidbody2d = GetComponent<Rigidbody2D>();
        currentTime = onTimeDirection;
        knockTime = knockbackForceTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (knockTime <= 0)
        {
            myRigidbody2d.linearVelocityX = moveSpeed;
            currentTime -= Time.deltaTime;

            if (currentTime <= 0)
            {
                Flip();
            }
        }
        else
        {
            knockTime -= Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Arrow"))
        {
            Destroy(gameObject);
        }

        if (other.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Flip();   
        }
        else
        {
            myRigidbody2d.linearVelocityX = -Mathf.Sign(myRigidbody2d.linearVelocityX) * knockbackForce;
            knockTime = knockbackForceTime;
        }
    }

    void Flip()
    {
        moveSpeed = -moveSpeed;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;

        currentTime = onTimeDirection;
        knockTime = 0;
    }
}
