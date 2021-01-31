using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 10.0f;
    public float decel = 0.1f;
    private float accel = 0.0f;
    public float max_accel = 1.0f;

    public Texture2D cursorDefault;
    public Texture2D possessCursor;

    public AudioClip possessSound;
    public AudioClip unpossessSound;

    public float possess_range = 20.0f;

    bool canMove = true;

    void FixedUpdate()
    {
        if (canMove)
            Move();
    }

    public void SetPossessEffect(bool mode)
    {
        if (!mode) // possess
            Cursor.SetCursor(cursorDefault, new Vector2(possessCursor.width / 2, possessCursor.height / 2), CursorMode.Auto);
        else
            Cursor.SetCursor(possessCursor, new Vector2(0, 0), CursorMode.Auto);
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, -1f);
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
            transform.localScale = new Vector3(axis.x, transform.localScale.y);
        accel = Mathf.Clamp(accel, 0.0f, max_accel);
        axis = new Vector2(axis.x * accel, axis.y * accel);


        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(rb.position + new Vector2(axis.x * Time.fixedDeltaTime, axis.y * Time.fixedDeltaTime));
    }

    public void OnPossess(Vector3 enemyPos)
    {
        canMove = false;
        LeanTween.move(gameObject, enemyPos, .2f);
        LeanTween.scale(gameObject, new Vector3(0.0f, 0.0f), .2f).setOnComplete(() => gameObject.SetActive(false));
        
        GetComponent<AudioSource>().clip = possessSound;
        GetComponent<AudioSource>().pitch = Random.Range(.95f, 1.05f);
        GetComponent<AudioSource>().Play();
    }
    public void OnDepossess(Vector3 pos)
    {
        canMove = true;
        transform.position = pos;
        LeanTween.cancel(gameObject);
        gameObject.SetActive(true);

        GetComponent<AudioSource>().clip = unpossessSound;
        GetComponent<AudioSource>().pitch = Random.Range(.90f, 1.1f);
        GetComponent<AudioSource>().Play();

        LeanTween.scale(gameObject, new Vector3(1.0f, 1.0f), .2f);
    }

    public float GetPossessRange()
    {
        return possess_range;
    }
}
