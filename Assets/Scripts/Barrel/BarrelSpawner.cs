using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class BarrelSpawner : MonoBehaviour, IRequireCleanup
{
    [SerializeField] private GameObject barrelPrefab;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float minimumRange;
    [SerializeField] private float spawnRange;
    [SerializeField] private float spawnAngle = 40f;
    [SerializeField] private float spawnRadius;
    [SerializeField] private Vector3 centerPos;
    [SerializeField] private Vector3 direction;
    private List<GameObject> barrelList;

    private void Start()
    {
        barrelList = new List<GameObject>();
        GameManager.Instance.OnRoundStart += OnRoundStart;
        GameManager.Instance.OnApplicationCleanup += OnCleanup;
    }

    public void OnDisable()
    {
        if (!GameManager.cleanedUp) OnCleanup();
    }

    public void OnCleanup()
    {
        Debug.Log($"{name}: Unsubscribing in progress...");
        GameManager.Instance.OnRoundStart -= OnRoundStart;
        GameManager.Instance.OnApplicationCleanup -= OnCleanup;
    }

    public void OnRoundStart()
    {
        GameManager.Instance.ShotsRemaining = GameManager.Instance.maxShots;
        for (int i = 0; i < GameManager.Instance.maxBarrels; i++)
        {
            if (i >= barrelList.Count)
            {
                GameObject temp = Instantiate(barrelPrefab);
                temp.transform.position = centerPos;
                temp.transform.SetParent(gameObject.transform);
                temp.SetActive(false);
                barrelList.Add(temp);
            }

            barrelList[i].transform.position = ChooseBarrelLocation(i);
            barrelList[i].SetActive(true);
            GameManager.Instance.BarrelsRemaining++;
        }

        if (GameManager.Instance.maxBarrels >= barrelList.Count) return;
        for (int i = GameManager.Instance.maxBarrels; i < barrelList.Count; i++)
        {
            barrelList[i].transform.position = centerPos;
            barrelList[i].SetActive(false);
        }
    }

    private Vector3 ChooseBarrelLocation(int i)
    {
        bool validPos;
        Vector3 pos;
        Vector3 dir;

        do
        {
            pos = centerPos;
            pos.y = spawnHeight;
            dir = direction;

            float angle = Random.Range(-spawnAngle, spawnAngle);
            float range = Random.Range(minimumRange, spawnRange);

            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            dir = rotation * dir;
            dir *= range;

            pos = pos + dir;
            if (i == 0) return pos;

            validPos = CheckPositionValidity(pos, i);
        }
        while (!validPos);

        return pos;
    }

    private bool CheckPositionValidity(Vector3 pos, int index)
    {
        for (int i = 0; i < index; i++)
        {
            if (Vector3.Distance(pos, barrelList[i].transform.position) < spawnRadius)
            {
                return false;
            }
        }
        return true;
    }

    public Vector3 GetAccessiblePosition()
    {
        Vector3 pos = centerPos;
        pos.y = spawnHeight;
        Vector3 dir = direction;

        float angle = Random.Range(-spawnAngle, spawnAngle);
        float range = Random.Range(minimumRange, spawnRange);

        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
        dir = rotation * dir;
        dir *= range;

        pos = pos + dir;

        return pos;
    }
}
