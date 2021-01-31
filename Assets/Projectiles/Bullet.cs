using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 dir = Vector2.zero;
    protected GameObject owner = null;

    public float moveSpeed = 5.0f;

    private void Start()
    {
        Destroy(gameObject, 4.0f);
    }

    public void InitBullet(Vector2 direction, GameObject _owner)
    {
        dir = direction;
        owner = _owner;
        
        Vector3 rot = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void Update()
    {
        transform.position += new Vector3(dir.x * 100000, dir.y * 100000).normalized * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurt(other.gameObject);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        Hurt(other.gameObject);
    }

    private void Hurt(GameObject other)
    {
        if (other == owner) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy)
        {
            if (owner == null)
                enemy.Die();
            else if (!owner.GetComponent<Enemy>().possessed && owner.GetComponent<Enemy>().aggroedOn != null && enemy == owner.GetComponent<Enemy>().aggroedOn)
                enemy.Die();
            else if (owner.GetComponent<Enemy>().possessed)
                enemy.Die();
        }

        GetComponentInChildren<ParticleSystem>().Stop();
        Destroy(GetComponentInChildren<ParticleSystem>(), 2.0f);
        GetComponentInChildren<ParticleSystem>().transform.parent = null;

        Destroy(gameObject);
    }
}
