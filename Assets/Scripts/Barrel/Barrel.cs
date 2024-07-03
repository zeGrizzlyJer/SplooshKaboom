using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Barrel : MonoBehaviour
{
    [SerializeField] private GameObject hitEffect;
    [SerializeField] private AudioClip hitSound;
    [SerializeField] private AudioClip explosionSound;

    private void OnTriggerEnter(Collider other)
    {
        Kaboom();
        gameObject.SetActive(false);
        GameManager.Instance.BarrelsRemaining--;
    }

    public void Kaboom()
    {
        if (hitEffect)
        {
            Vector3 spawnPoint = transform.position;
            spawnPoint.y = 1.2f;
            Quaternion rotation = Quaternion.Euler(-90f, 0, 0);
            GameObject temp = Instantiate(hitEffect, spawnPoint, rotation);
            Destroy(temp, 6f);
        }
        if (hitSound)
        {
            AudioManager.Instance.Play2DSFX(hitSound);
        }
        if (explosionSound)
        {
            AudioManager.Instance.Play3DAudio(explosionSound, transform.position);
        }
    }
}
