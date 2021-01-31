using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Audio;


public class Enemy : MonoBehaviour
{
    private Vector3 origScale;
    protected Player player;

    public float moveSpeed = 10.0f;
    public float decel = 0.1f;
    private float accel = 0.0f;
    public float max_accel = 1.0f;

    public bool canKillSelf = false;

    public AudioClip footstepSound;

    public GameObject possessedParticle;

    bool changedCursor = true;

    public GameObject exclamation;

    public Vector2 forwardVector = Vector2.zero;

    public Material outlineMat;
    public Material defaultMat;

    public float aggroRange = 2.0f;

    bool scaledUp = false;

    public AudioClip die;

    private float walkTimer = 0.0f;

    public bool possessed = false;
    bool beingPossessed = false;
    bool firstPossession = false;

    protected bool canControl = true;

    public GameObject blood;
    public GameObject bloodSplat;

    public Enemy aggroedOn = null;

    public bool startAggro = false;

    enum State
    {

    }

    void Start()
    {
        origScale = transform.localScale;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public virtual void Die()
    {
        foreach (GameObject _e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemy = _e.GetComponent<Enemy>();
            if (!enemy) continue;
            if (Vector3.Distance(enemy.transform.position, transform.position) < enemy.aggroRange)
            {
                Debug.DrawRay(transform.position, enemy.transform.position - transform.position, Color.green, 1f);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, enemy.transform.position - transform.position, (enemy.transform.position - transform.position).magnitude, 1 << 9);
                if (!hit)
                    enemy.Aggroed();
            }
        }
        ActuallyDie();
    }

    private void ActuallyDie()
    {
        if (bloodSplat)
        {
            GameObject _bl = Instantiate(bloodSplat, new Vector3(transform.position.x, transform.position.y, 6f), Quaternion.identity);
            _bl.transform.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, UnityEngine.Random.Range(0, 360.0f)));
        }

        GameObject holder = new GameObject();
        AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;
        
        audi.clip = die;
        audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
        audi.volume = .5f;
        audi.Play();
        Destroy(holder, 2.0f);

        Camera.main.GetComponent<CameraShake>().ShakeIt(.05f, .1f);
        if (possessed)
            OnDepossess();
        Destroy(gameObject);
    }

    protected virtual void Aggroed()
    {
        foreach (GameObject _e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemy = _e.GetComponent<Enemy>();
            if (enemy && enemy.possessed && enemy != this)
            {
                exclamation.SetActive(true);
                LeanTween.scale(exclamation, new Vector3(1.5f, 1.5f), .3f).setEaseOutBack().setOnComplete(() => exclamation.SetActive(false));
                aggroedOn = enemy;
                return;
            }
        }
    }

    protected virtual void Update()
    {
        if (possessed)
        {
            if (Input.GetMouseButtonDown(0) && !beingPossessed)
            {
                PerformAction();
            }
            if (Input.GetMouseButtonUp(0) && firstPossession)
            {
                beingPossessed = false;
                firstPossession = false;
            }
            if (Input.GetMouseButtonDown(1) && !beingPossessed)
            {
                Collider2D results = Physics2D.OverlapCircle(transform.position, .7f, 1 << 11);
                if (!results)
                {
                    OnDepossess();
                }
            }
            possessedParticle.SetActive(true);
        }
        else
        {
            if (aggroedOn != null)
            {
                PerformAggroedAction();
            }
            possessedParticle.SetActive(false);
        }

    }

    public virtual void PerformAggroedAction()
    {

    }

    private void FixedUpdate()
    {
        if (possessed)
        {
            if (canControl)
                Move();
        }
    }

    public virtual void PerformAction()
    {
        print("perform action");
    }


    protected virtual void OnMouseOver()
    {
        bool inRange = Vector3.Distance(player.transform.position, transform.position) < player.GetPossessRange();
        if (!player.gameObject.activeInHierarchy)
        {
            if (scaledUp)
                UnHoverEffect();
            return;
        }
        if (Input.GetMouseButton(0) && inRange)
        {
            OnPossess();
        }
        if (inRange)
        {
            HoverEffect();
            scaledUp = true;
        }
        else
        {
            if (scaledUp)
            {
                UnHoverEffect();
                scaledUp = false;
            }
        }
    }

    private void HoverEffect()
    {
        // transform.localScale = origScale + new Vector3(.2f, .2f);
        // GetComponent<SpriteRenderer>().material = outlineMat;
        if (changedCursor)
        {
            player.SetPossessEffect(true);
            changedCursor = !changedCursor;
        }
    }

    private void UnHoverEffect()
    {
        // transform.localScale = origScale - new Vector3(.2f, .2f);
        // if (!possessed)
        //     GetComponent<SpriteRenderer>().material = defaultMat;
        if (!changedCursor)
        {
            player.SetPossessEffect(false);
            changedCursor = !changedCursor;
        }
    }

    protected virtual void OnMouseExit()
    {
        if (scaledUp)
        {
            UnHoverEffect();
            scaledUp = false;
        }
    }

    private void OnPossess()
    {
        player.OnPossess(transform.position);
        // GetComponent<SpriteRenderer>().material = outlineMat;
        possessed = true;
        firstPossession = true;
        beingPossessed = true;
        aggroedOn = null;
    }

    protected void OnDepossess()
    {
        player.OnDepossess(transform.position);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.AddForce(forwardVector * -2, ForceMode2D.Impulse);

        // GetComponent<SpriteRenderer>().material = defaultMat;
        possessed = false;
        firstPossession = false;
        beingPossessed = false;
    }

    private void Move()
    {
        Vector2 axis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (axis != Vector2.zero)
        {
            accel += decel;
        }
        else
        {
            accel -= decel;
        }
        //sprite flipping
        if (axis.x != 0.0f)
        {
            transform.localScale = new Vector3(axis.x, transform.localScale.y);
        }
        if (axis.magnitude != 0.0f && possessed)
        {
            walkTimer += Time.deltaTime;
            if (walkTimer > (.9f / moveSpeed) * 2f)
            {
                walkTimer = 0.0f;
                GameObject holder = new GameObject();
                AudioSource audi = holder.AddComponent(typeof(AudioSource)) as AudioSource;

                audi.clip = footstepSound;
                audi.pitch = UnityEngine.Random.Range(.90f, 1.1f);
                audi.Play();
                Destroy(holder, .5f);
            }
        }
        else
        {
            walkTimer = 0.0f;
        }
        accel = Mathf.Clamp(accel, 0.0f, max_accel);
        axis = new Vector2(axis.x * accel, axis.y * accel);

        //transform.position += new Vector3(axis.x * Time.fixedDeltaTime, axis.y * Time.fixedDeltaTime);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        forwardVector = axis;
        rb.MovePosition(rb.position + new Vector2(axis.x * Time.fixedDeltaTime, axis.y * Time.fixedDeltaTime));
    }

    public void MoveTowards(Vector2 axis)
    {

        if (axis != Vector2.zero)
        {
            accel += decel;
        }
        else
        {
            accel -= decel;
        }
        //sprite flipping
        if (axis.x != 0.0f)
        {
            transform.localScale = new Vector3(Mathf.Sign(axis.x), transform.localScale.y);
        }
        accel = Mathf.Clamp(accel, 0.0f, max_accel);
        axis = new Vector2(axis.x * accel, axis.y * accel);

        //transform.position += new Vector3(axis.x * Time.fixedDeltaTime, axis.y * Time.fixedDeltaTime);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + new Vector2(axis.x * Time.fixedDeltaTime, axis.y * Time.fixedDeltaTime));
    }
}
