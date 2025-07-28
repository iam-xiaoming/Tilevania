using UnityEngine;
using UnityEngine.InputSystem;

public class Attack : MonoBehaviour
{
    [SerializeField] Transform gun;
    [SerializeField] GameObject arrow;
    PlayerMortality playerMortality;

    void Awake()
    {
        playerMortality = GetComponent<PlayerMortality>();
    }

    void OnFire(InputValue value)
    {
        if (!playerMortality.IsAlive) { return; }

        GameObject newArrow = Instantiate(arrow, gun.position, arrow.transform.rotation);

        Vector3 localScale = newArrow.transform.localScale;
        Vector3 playerLocalScale = transform.localScale;

        localScale.x = playerLocalScale.x;
        newArrow.transform.localScale = localScale;
    }
}
