using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JettSmokeProjectile : MonoBehaviour
{
    [SerializeField] GameObject smokeBallPrefab;

    private Vector3 startingPosition;
    private float particleMovementSpeed = 20.0f;
    private float maxDistance = 70.0f;
    private float distanceTraveled = 0f;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        Vector3 movementVector = transform.forward * particleMovementSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position += movementVector;
        distanceTraveled = Vector3.Distance(startingPosition, newPosition);

        if (distanceTraveled > maxDistance)
        {
            // TODO: move the transform to the max distance
            OnCreateSmokeBall(transform.position);
        } else
        {
            transform.position += movementVector;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        OnCreateSmokeBall(collision.contacts[0].point);
    }

    void OnCreateSmokeBall(Vector3 position)
    {
        Instantiate(smokeBallPrefab, position, transform.rotation);
        Destroy(this);
    }
}
