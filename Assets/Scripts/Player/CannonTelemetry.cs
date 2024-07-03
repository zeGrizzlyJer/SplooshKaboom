using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonTelemetry : MonoBehaviour
{
    [SerializeField] private GameObject telemetryPointPrefab;
    /// <summary>
    /// Ratio of visible telemetry points to the total points caculated behind the scenes. Ie, a ratio of 1 is 1 visual point per calculated point. A ratio of 
    /// 3 is 1 visual point every 3 calculated points. Make sure totalPoints is divisible by this ratio value.
    /// </summary>
    public int visualTelemetryRatio = 1;
    public int totalPoints = 20;
    public float maxTime = 2.0f;
    private float timeStep;

    List<GameObject> telemetryPoints;
    private CannonController cannon;

    private void Start()
    {
        cannon = GetComponent<CannonController>();
        timeStep = maxTime / totalPoints;
        telemetryPoints = new List<GameObject>();
        for (int i = 0; i < totalPoints; i++)
        {
            GameObject point = Instantiate(telemetryPointPrefab);
            point.SetActive(false);
            telemetryPoints.Add(point);
        }
    }

    public void UpdateTelemetry(Vector3 startPosition, Vector3 startVelocity)
    {
        if (startVelocity.magnitude <= 1f)
        {
            for (int i = 0; i < totalPoints; i++)
            {
                telemetryPoints[i].SetActive(false);
            }
            return;
        }
        Vector3 previousPosition = startPosition;
        Vector3 previousVelocity = startVelocity;
        for (int i = 0; i < totalPoints; i++)
        {
            Vector3 pointPosition = CalculatePositionAtTime(previousPosition, previousVelocity, timeStep);
            Vector3 pointVelocity = CalculateVelocityAtTime(previousPosition, previousVelocity, timeStep);
            previousPosition = pointPosition;
            previousVelocity = pointVelocity;

            telemetryPoints[i].transform.position = pointPosition;
            telemetryPoints[i].SetActive(true);
        }
    }

    Vector3 CalculatePositionAtTime(Vector3 previousPosition, Vector3 previousVelocity, float time)
    {
        return previousPosition + (previousVelocity * time) + (0.5f * CustomGravity.GetGravity(previousPosition) * time * time); 
    }
    
    Vector3 CalculateVelocityAtTime(Vector3 previousPositon, Vector3 previousVelocity, float time)
    {
        return previousVelocity + (CustomGravity.GetGravity(previousPositon) * time);
    }
}
