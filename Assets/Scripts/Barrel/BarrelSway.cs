using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;

public class BarrelSway : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    [SerializeField] float sideSwaySpeed;
    [SerializeField] float sideSwayDistance;
    [SerializeField] float forwardSwaySpeed;
    [SerializeField] float forwardSwayDistance;
    [SerializeField] float verticalSwaySpeed;
    [SerializeField] float verticalSwayDistance;

    private float swayVariance = 0f;
    private float distanceVariance = 0f;

    private Vector3 startPos;
    private Quaternion startRot;

    private void OnEnable()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
        swayVariance = Random.Range(Mathf.PI, 2 * Mathf.PI);
        distanceVariance = Random.Range(0.5f, 0.8f);
    }

    private void FixedUpdate()
    {
        float xRot = (distanceVariance * sideSwayDistance * Mathf.Sin(((swayVariance / (2 * Mathf.PI)) + Time.time) * sideSwaySpeed));
        float yRot = swayVariance * Mathf.Rad2Deg + rotationSpeed * Time.time;
        if (yRot > 360f) yRot -= 360f;
        float zRot = (distanceVariance * forwardSwayDistance * Mathf.Sin(((swayVariance / (2 * Mathf.PI)) + Time.time) * forwardSwaySpeed));

        Vector3 verticalOffset = new Vector3(0f, (distanceVariance * verticalSwayDistance * Mathf.Sin(((swayVariance / (2 * Mathf.PI)) + Time.time) * verticalSwaySpeed)), 0f);

        transform.localPosition = startPos + verticalOffset;
        transform.localRotation = startRot * Quaternion.Euler(xRot, yRot, zRot);
    }
}
