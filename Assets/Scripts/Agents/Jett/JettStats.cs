using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JettStats : MonoBehaviour
{
    // Floating
    public static float FloatingGravity = -5.0f;

    // Dash
    [System.NonSerialized] public float dashSpeed = 30f;
    [System.NonSerialized] public float dashDurationSeconds = 0.4f;
    [System.NonSerialized] public int maxDashAttempts = 100;

    // Smokes
    [System.NonSerialized] public int maxSmokeAttempts = 100;
    [System.NonSerialized] public float smokeDelaySeconds = 0.3f;

    // Updraft
    [System.NonSerialized] public int maxUpdraftAttempts = 10;
    [System.NonSerialized] public float updraftHeight = 4.0f;
    [System.NonSerialized] public float updraftDelaySeconds = 0.2f;
}
