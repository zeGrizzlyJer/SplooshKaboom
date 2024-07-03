using System;
using System.Collections.Generic;
using UnityEngine;

public class WindSource : GravitySource, IRequireCleanup
{
    [SerializeField] private float noiseScale;
    [SerializeField] private Vector3 windDirection;
    [SerializeField] private float transitionTime = 1f;
    public float maxWindStrength = 30f;
    public Vector3 WindDirection
    {
        get
        {
            return windDirection;
        }
        set
        {
            if (value == windDirection) return;
            windDirection = value;
            windDirectionChanged?.Invoke(windDirection);
        }
    }

    public event Action<Vector3> windDirectionChanged;
    private bool InTransition = false;
    private float t = 0f;
    private Vector3 StartVector, TargetVector = Vector3.zero;

    public override Vector3 GetGravity(Vector3 position)
    {
        float perlinX = Mathf.Clamp(Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale), 0f, 1f);
        float perlinY = Mathf.Clamp(Mathf.PerlinNoise(position.y * noiseScale, position.z * noiseScale), 0f, 1f);
        float perlinZ = Mathf.Clamp(Mathf.PerlinNoise(position.z * noiseScale, position.x * noiseScale), 0f, 1f);

        Vector3 wind = new Vector3(perlinX * WindDirection.x, perlinY * WindDirection.y, perlinZ * WindDirection.z);
        return wind;
    }

    public void RandomizeWindDirection()
    {
        float magnitude = ((float)GameManager.Instance.GameRound / (float)GameManager.Instance.roundsToWin) * maxWindStrength;
        Vector3 direction = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(0, 1f)).normalized;
        ChangeWindDirection(direction * magnitude);
    }

    public void ChangeWindDirection(Vector3 direction)
    {
        if (direction == WindDirection) return;
        StartVector = WindDirection;
        TargetVector = direction;
        t = 0f;
        InTransition = true;
    }

    private void Update()
    {
        if (!InTransition) return;
        if (t >= transitionTime)
        {
            InTransition = false;
            WindDirection = TargetVector;
            return;
        }
        t += Time.deltaTime;
        WindDirection = Vector3.Lerp(StartVector, TargetVector, t / transitionTime);
    }

    private void Start()
    {
        GameManager.Instance.OnRoundStart += RandomizeWindDirection;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
    }

    #region Cleanup
    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        GameManager.Instance.OnRoundStart -= RandomizeWindDirection;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
    }
    #endregion
}
