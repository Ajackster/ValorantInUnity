using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeBall : MonoBehaviour
{
    [SerializeField] float durationSeconds;

    private float startTime;
    private float timeElapsed = 0.0f;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        timeElapsed = Time.time - startTime;

        if (timeElapsed >= durationSeconds)
        {
            Vanish();
        }
    }

    void Vanish()
    {
        // TODO:// Trigger some effect
        Destroy(this.gameObject);
    }
}
