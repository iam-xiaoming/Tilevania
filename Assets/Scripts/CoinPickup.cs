using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    GameSession gameSession;

    void Start()
    {
        gameSession = FindFirstObjectByType<GameSession>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position, 1f);

        GameObject tempGO = new GameObject("TempAudio");
        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = coinPickupSFX;
        aSource.spatialBlend = 0f; // 2D sound
        aSource.Play();

        Destroy(tempGO, coinPickupSFX.length);

        gameSession.UpdateScoreText();

        Destroy(gameObject);

		// Debug.Break();
    }
}
