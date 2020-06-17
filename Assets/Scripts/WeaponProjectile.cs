using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    private float speed = 200.0f;
    private float maxDistance = 10.0f;
    private Vector3 startingPoint;

    void Start()
    {
        startingPoint = transform.position;
    }

    void Update()
    {
        Vector3 nextPosition = transform.position + (transform.forward * speed * Time.deltaTime);

        if (Vector3.Distance(startingPoint, nextPosition) <= maxDistance)
        {
            transform.position = nextPosition;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }
}
