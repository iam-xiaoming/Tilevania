using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [SerializeField] Transform gun;
    [SerializeField] GameObject arrow;
    PlayerMortality playerMortality;
    PlayerMovement playerMovement;

    void Awake()
    {
        playerMortality = GetComponent<PlayerMortality>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnFire(InputValue value)
    {
        if (!playerMortality.IsAlive) { return; }
        GameObject newArrow = Instantiate(arrow, gun.position, arrow.transform.rotation);

        Vector3 localScale = newArrow.transform.localScale;
        Vector3 playerLocalScale = playerMovement.GetLocalScale();

        localScale.y = playerLocalScale.x;
        newArrow.transform.localScale = localScale;
    }
}
