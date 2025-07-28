using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] float arrowSpeed = 10f;
    Rigidbody2D arrowRigidbody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        arrowRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        arrowRigidbody.linearVelocityX = transform.localScale.x * arrowSpeed;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);    
    }
}
