using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailBehaviour : MonoBehaviour
{
    private bool deactivated = false;
    [SerializeField] private GameObject projectile;
    private ParticleSystem ps;

    public void SetTarget(GameObject target)
    {
        projectile = target;
    }

    private void OnEnable()
    {
        deactivated = false;
    }

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (deactivated || projectile == null) return;
        if (projectile.activeSelf)
        {
            gameObject.transform.position = projectile.transform.position;
        }
        else
        {
            StartCoroutine(DeactivateSelf());
            deactivated = true;
        }
    }

    private IEnumerator DeactivateSelf()
    {
        yield return new WaitForSeconds(ps.main.startLifetime.constant);
        gameObject.SetActive(false);
    }
}
