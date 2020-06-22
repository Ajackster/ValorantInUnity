using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JettSmokeProjectile : MonoBehaviour
{
    [SerializeField] GameObject smokeBallPrefab = default;

    public bool isControlled { get; private set; }

    private Vector3 startingPosition;
    private float particleMovementSpeed = 20.0f;
    private float maxDistance = 70.0f;
    private float distanceTraveled = 0f;
    private Camera playerCamera;

    private float downwardForce = -2.0f;
    private float downwardForceIncrement = -3.8f;

    void Start()
    {
        startingPosition = transform.position;
    }

    void Update()
    {
        if (isControlled)
        {
            transform.rotation = playerCamera.transform.rotation;
        }

        Vector3 movementVector = (transform.forward * particleMovementSpeed * Time.deltaTime);
        if (!isControlled)
        {
            downwardForce += downwardForceIncrement * Time.deltaTime;
            movementVector += (transform.up * downwardForce * Time.deltaTime);
        }

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

    public void InitializeValues(bool _isControlled, Camera _playerCamera)
    {
        isControlled = _isControlled;
        playerCamera = _playerCamera;
    }

    public void SetIsControlled(bool _isControlled)
    {
        isControlled = _isControlled;
    }

    void OnCreateSmokeBall(Vector3 position)
    {
        Instantiate(smokeBallPrefab, position, transform.rotation);
        Destroy(this.gameObject);
    }
}
