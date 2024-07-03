using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugButton : MonoBehaviour
{
    [SerializeField] private WindSource wind;
    public Vector3 direction;
    public float magnitude;

    private void Start()
    {
        if (!wind) wind = FindFirstObjectByType<WindSource>();
    }

    public void DEBUGWIND()
    {
        magnitude = Random.Range(0f, 30f);
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(0f, 1f)).normalized;
        wind.ChangeWindDirection(direction * magnitude);
    }

    public void DEBUGGAMEROUND()
    {
        TornadoController temp = FindAnyObjectByType<TornadoController>();
        if (temp != null)
        {
            temp.previousCount = 0;
        }
        GameManager.Instance.GameRound++;
    }
}
