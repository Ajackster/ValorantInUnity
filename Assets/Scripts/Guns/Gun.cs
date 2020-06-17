using UnityEngine;

public interface Gun
{
    string name { get; }
    int maxMagazineAmmo { get; }
    int maxAmmo { get; }
    float fireRatePerSecond { get; }
    float fallOffDistance { get; }
    float recoilResetTimeSeconds { get; }
    Vector3[] recoilPattern { get; }
}
