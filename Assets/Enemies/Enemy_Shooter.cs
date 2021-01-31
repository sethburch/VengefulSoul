using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Shooter : Enemy
{
    public Bullet bulletPrefab;
    public float shootSpeedWhenAggroed = .5f;

    public GameObject rotateAxis;
    public GameObject gun;

    public AudioClip shootSnd;

    float cooldown = 0.0f;
    public float cooldownLength = .5f;

    protected override void Update()
    {
        base.Update();

        cooldown += Time.deltaTime;

        if (possessed)
        {
            rotateAxis.transform.localScale = new Vector3(transform.localScale.x, 1.0f, 1.0f);

            Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotateAxis.transform.rotation = Quaternion.Lerp(rotateAxis.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), .3f);
        }
        else if (aggroedOn != null)
        {
            Vector3 dir = (aggroedOn.transform.position - transform.position).normalized;
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            rotateAxis.transform.rotation = Quaternion.Lerp(rotateAxis.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), .3f);
        }
    }

    public override void PerformAction()
    {
        if (cooldown < cooldownLength) return;
        cooldown = 0.0f;

        GetComponent<AudioSource>().clip = shootSnd;
        GetComponent<AudioSource>().pitch = Random.Range(.90f, 1.1f);
        GetComponent<AudioSource>().volume = .5f;
        GetComponent<AudioSource>().Play();
        Camera.main.GetComponent<CameraShake>().ShakeIt(.05f, .1f);

        if (bulletPrefab)
        {
            Bullet bullet = Instantiate(bulletPrefab, gun.transform.position, Quaternion.identity);
            Vector3 dir;

            if (aggroedOn != null)
            {
                dir = aggroedOn.transform.position - transform.position;
            }
            else
            {
                dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            }
           
            bullet.InitBullet(Vector3.Normalize(dir), gameObject);
        }
    }

    public override void PerformAggroedAction()
    {
        Vector3 dir = (aggroedOn.transform.position - transform.position).normalized;
        if (!startAggro)
        {
            StartCoroutine("Shoot");
            startAggro = true;
        }
    }

    IEnumerator Shoot()
    {
        while(aggroedOn != null)
        {
            PerformAction();
            yield return new WaitForSeconds(shootSpeedWhenAggroed);
        }
    }
}
