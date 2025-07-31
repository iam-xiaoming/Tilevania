using UnityEngine;

public class ScenePersist : MonoBehaviour
{
    void Awake()
    {
        int numScenePersist = FindObjectsByType<ScenePersist>(FindObjectsSortMode.None).Length;

        if (numScenePersist > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject); // Don't destroy object when load scene. It will maintain scores,... you already have.
        }
    }

    public void DestroyScenePersist()
    {
        Destroy(gameObject);
    }
}
