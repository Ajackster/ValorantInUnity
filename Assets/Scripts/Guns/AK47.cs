using System;
using System.Collections.Generic;
using UnityEngine;

class AK47 : Gun
{
    public string name { get; private set; } = "AK-47";
    public int maxMagazineAmmo { get; private set; } = 25;
    public int maxAmmo { get; private set; } = 100;
    public float fireRatePerSecond { get; private set; } = 8f;
    public float fallOffDistance { get; private set; } = 150f;
    public float recoilResetTimeSeconds { get; private set; } = 0.5f;
    public Vector3[] recoilPattern { get; private set; } = new Vector3[25]
    {
        new Vector3(-0.5f, 0, 0),
        new Vector3(-0.5f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(-0.8f, 0, 0),
        new Vector3(0, 0.8f, 0),
        new Vector3(0, 0.8f, 0),
        new Vector3(0, 0.8f, 0),
        new Vector3(0, 0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0),
        new Vector3(0, -0.8f, 0)
    };
}
