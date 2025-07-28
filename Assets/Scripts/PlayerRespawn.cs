using UnityEngine;
using Cinemachine;
using System;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] float timeWaitToRespawn = 5f;
    public bool isActiveRespawn = false;
    float timeCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeCounter = timeWaitToRespawn;
    }

    // Update is called once per frame
    void Update()
    {
        if (isActiveRespawn)
        {
            timeCounter -= Time.deltaTime;
            if (timeCounter <= 0)
            {
                timeCounter = timeWaitToRespawn;
                isActiveRespawn = false;
                GameObject newPlayer = Instantiate(player, transform.position, Quaternion.identity);

                CinemachineVirtualCamera[] cams = FindObjectsByType<CinemachineVirtualCamera>(FindObjectsSortMode.None);

                foreach (CinemachineVirtualCamera cam in cams)
                {
                    cam.Follow = newPlayer.transform;
                }
            }
        }
    }
}
