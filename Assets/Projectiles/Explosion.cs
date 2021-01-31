using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float explosionRadius = 5.0f;
    public float explosionLife = .3f;
    public AudioClip explosionSnd;

    public GameObject owner = null;

    private void Start()
    {
        GetComponent<AudioSource>().clip = explosionSnd;
        GetComponent<AudioSource>().pitch = Random.Range(.90f, 1.1f);
        GetComponent<AudioSource>().volume = .5f;
        GetComponent<AudioSource>().Play();
        Destroy(gameObject, explosionLife);
    }

    private void Update()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy _enemy = enemy.GetComponent<Enemy>();

            if (!_enemy) continue;

            float dist = Vector3.Distance(_enemy.transform.position, transform.position);

            if (dist <= explosionRadius)
            {
                // if (!owner.GetComponent<Enemy>().possessed && owner.GetComponent<Enemy>().aggroedOn != null && enemy == owner.GetComponent<Enemy>().aggroedOn)
                //     _enemy.Die();
                // else
                _enemy.Die();
            }
        }
    }
}
