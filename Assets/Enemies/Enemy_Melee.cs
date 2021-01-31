using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Melee : Enemy
{

    public float attackRange = 1.5f;
    public float attackWidth = .3f;

    bool flipRot = false;
    int flip = -1;

    public GameObject rotateAxis;
    public GameObject bat;

    float cooldown = 0.0f;
    public float cooldownLength = .5f;

    public AudioClip swingSnd;



    bool swinging = false;

    protected override void Update()
    {
        base.Update();

        cooldown += Time.deltaTime;

        if (!possessed) return;
        if (swinging) return;

        rotateAxis.transform.localScale = new Vector3(transform.localScale.x, 1.0f, 1.0f);

        Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        rotateAxis.transform.rotation = Quaternion.Lerp(rotateAxis.transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), .3f);
    }

    private void SwingStop()
    {
        swinging = false;
    }

    public override void PerformAggroedAction()
    {
        Vector3 dir = (aggroedOn.transform.position - transform.position).normalized;
        MoveTowards(new Vector2(dir.x, dir.y));
        if (!startAggro)
        {
            StartCoroutine("Swing");
            startAggro = true;
        }
    }

    IEnumerator Swing()
    {
        while (aggroedOn != null)
        {
            PerformAction();
            yield return new WaitForSeconds(1f);
        }
    }

    public override void PerformAction()
    {
        if (cooldown < cooldownLength) return;
        cooldown = 0.0f;
        Camera.main.GetComponent<CameraShake>().ShakeIt(.05f, .1f);
        GetComponent<AudioSource>().clip = swingSnd;
        GetComponent<AudioSource>().pitch  = Random.Range(.90f, 1.1f);
        GetComponent<AudioSource>().volume = .5f;
        GetComponent<AudioSource>().Play();

        LeanTween.rotateLocal(bat.gameObject, new Vector3(0.0f, 0.0f, 90.0f * flip), .3f).setEaseOutElastic().setOnComplete(SwingStop);
        swinging = true;

        if (flipRot)
        {
            flipRot = !flipRot;
            flip = -1;
        }
        else
        {
            flipRot = !flipRot;
            flip = 1;
        }

        foreach (GameObject _enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (_enemy == this.gameObject) continue;

            //GameObject _enemy = enemy.GetComponent<GameObject>();

            float dist = Vector3.Distance(_enemy.transform.position, transform.position);

            if (dist <= attackRange)
            {
                Vector3 facing;
                if (aggroedOn != null)
                {
                    facing = aggroedOn.transform.position - transform.position;
                }
                else
                {
                    facing = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                }
                Vector3 enemy_length = _enemy.transform.position - transform.position;

                float dot = Vector3.Dot((facing * 10000).normalized, enemy_length);

                if (dot > 0.0f)
                {
                    if (_enemy.GetComponent<Destructable>() != null)
                    {
                        _enemy.GetComponent<Destructable>().Destruct();
                    }
                    if (_enemy.GetComponent<Enemy>() != null)
                    {
                        if (!possessed && aggroedOn != null && _enemy.GetComponent<Enemy>() == aggroedOn)
                            _enemy.GetComponent<Enemy>().Die();
                        else if (possessed)
                            _enemy.GetComponent<Enemy>().Die();
                    }

                    // TODO doesnt rotate right
                    if (blood)
                    {
                        Vector3 dir = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        GameObject bl = Instantiate(blood, transform.position, Quaternion.AngleAxis(angle, Vector3.forward));
                    }
                }
            }
        }
    }
}
