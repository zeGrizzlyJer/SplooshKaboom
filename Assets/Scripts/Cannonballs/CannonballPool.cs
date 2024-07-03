using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class CannonballPool : MonoBehaviour, IProjectile
{
    [SerializeField] private Cannonball prefab;
    [SerializeField] private TrailBehaviour trail;
    public int maxCannonballs = 3;
    private Queue<IProjectile> cannonballs;
    private Queue<TrailBehaviour> trails;

    private void Start()
    {
        cannonballs = new Queue<IProjectile>();
        trails = new Queue<TrailBehaviour>();
        for (int i = 0;  i < maxCannonballs; i++)
        {
            Cannonball temp = Instantiate(prefab);
            temp.transform.parent = transform;
            temp.gameObject.SetActive(false);
            cannonballs.Enqueue(temp);

            TrailBehaviour t = Instantiate(trail);
            t.transform.parent = transform;
            t.SetTarget(temp.gameObject);
            t.gameObject.SetActive(false);
            trails.Enqueue(t);
        }
    }

    public void Launch(Vector3 pos, Quaternion rot, float power)
    {
        IProjectile projectile = cannonballs.Dequeue();
        projectile.Launch(pos, rot, power);
        cannonballs.Enqueue(projectile);

        TrailBehaviour tempTrail = trails.Dequeue();
        tempTrail.gameObject.SetActive(true);
        trails.Enqueue(tempTrail);
    }
}
