using UnityEngine;

public class TimeCounter : MonoBehaviour
{
    float previous = 0;
    float next = 0;

    // Update is called once per frame
    void Update()
    {
        next += Time.deltaTime;

        if (GetInterval() > 1000)
        {
            Reset();
        }
    }

    public float GetInterval()
    {
        return next - previous;
    }

    public void Reset()
    {
        previous = 0;
        next = 0;
    }
}
