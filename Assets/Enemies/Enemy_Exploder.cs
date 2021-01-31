using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Exploder : Enemy
{
    bool charging = false;
    Vector3 chargeDir = Vector3.zero;
    Vector3 chargeTo = Vector3.zero;
    public float chargeSpeed = 15.0f;

    public Explosion explosionPrefab;

    public override void PerformAction()
    {
        if (charging) return;

        canControl = false;
        charging = true;

        if (aggroedOn == null)
            chargeTo = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        else
            chargeTo = aggroedOn.transform.position;

        chargeDir = chargeTo - transform.position;
        chargeDir = new Vector3(chargeDir.x, chargeDir.y, 0.0f);
    }

    public override void PerformAggroedAction()
    {
        PerformAction();
    }

    protected override void Update()
    {
        base.Update();

        if (charging)
        {
            transform.position += (chargeDir * 100000).normalized * chargeSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (charging)
            Die();
    }

    public override void Die()
    {
        base.Die();
        if (possessed)
            OnDepossess();
        Explosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        explosion.owner = gameObject;
        Destroy(gameObject);
    }
}
