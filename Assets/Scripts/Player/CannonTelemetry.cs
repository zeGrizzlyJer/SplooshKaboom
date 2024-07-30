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
    public float telemetryScale = 2f;
    public float cycleTime = 0.2f;
    private int currentIndex = 0;
    private int previousIndex = 0;
    private float telemetryTimer = 0f;
    public Color activeColour = Color.white;
    public Color normalColour = Color.yellow;

    List<GameObject> telemetryPoints;
    private CannonController cannon;

    private void Start()
    {
        cannon = GetComponent<CannonController>();
        telemetryPoints = new List<GameObject>();
        for (int i = 0; i < totalPoints; i++)
        {
            GameObject point = Instantiate(telemetryPointPrefab);
            point.SetActive(false);
            telemetryPoints.Add(point);
        }
        telemetryPoints[0].transform.localScale *= telemetryScale;
    }

    private void Update()
    {
        telemetryTimer += Time.deltaTime;
        if (telemetryTimer >= cycleTime)
        {
            telemetryTimer = 0f;
            currentIndex = (currentIndex + (1 * visualTelemetryRatio));
            if (currentIndex >= telemetryPoints.Count) currentIndex = 0;
            ChangeTelemetrySettings(currentIndex, telemetryScale, activeColour);
            /*telemetryPoints[currentIndex].transform.localScale *= telemetryScale;
            var mat = telemetryPoints[currentIndex].GetComponent<MeshRenderer>().material;
            mat.SetColor("_EmissionColor", activeColour);*/
            ChangeTelemetrySettings(previousIndex, 1/telemetryScale, normalColour);
            previousIndex = currentIndex;
        }
    }

    private void ChangeTelemetrySettings(int index, float scale, Color newColor)
    {
        var mat = telemetryPoints[index].GetComponentInChildren<MeshRenderer>().material;
        mat.SetColor("_EmissionColor", newColor);
        telemetryPoints[index].transform.localScale *= scale;
    }

    public void UpdateTelemetry(Vector3 startPosition, Vector3 startVelocity)
    {
        timeStep = maxTime / totalPoints;
        if (startVelocity.magnitude <= 1f)
        {
            for (int i = 0; i < totalPoints; i++)
            {
                telemetryPoints[i].SetActive(false);
            }
            return;
        }
        Vector3 currentPosition = startPosition;
        Vector3 currentVelocity = startVelocity;
        Vector3 currentAcceleration = CustomGravity.GetGravity(currentPosition);
        for (int i = 0; i < totalPoints; i++)
        {
            Vector3 newPosition = currentPosition + currentVelocity * timeStep + 0.5f * currentAcceleration * Mathf.Pow(timeStep, 2);
            Vector3 newAcceleration = CustomGravity.GetGravity(newPosition);
            Vector3 newVelocity = currentVelocity + 0.5f * (currentAcceleration + newAcceleration) * timeStep;

            telemetryPoints[i].transform.position = newPosition;

            if (i % visualTelemetryRatio == 0) telemetryPoints[i].SetActive(true);
            else telemetryPoints[i].SetActive(false);
            

            currentPosition = newPosition;
            currentVelocity = newVelocity;
            currentAcceleration = newAcceleration;
        }
    }

    public void DisableTelemetry()
    {
        foreach(GameObject temp in telemetryPoints)
        {
            temp.SetActive(false);
        }
    }
}
