using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public enum Team { J1, J2 };
    public Team playerType;
    public Color colorJ1, colorJ2;
    private Rigidbody2D rb;
    public float jumpStrength;
    public float normalYmaxInclinasion;
    public bool groundCheck = false;
    public float moveSpeed;
    //public float accX, decX;
    public float gravityStrength;
    private float ogGravity;
    public string horizontal;
    KeyCode jumpKey;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        SpriteRenderer pRend = GetComponentInChildren<SpriteRenderer>();
        ogGravity = rb.gravityScale;
        switch (playerType)
        {
            case Team.J1:
                pRend.color = colorJ1;
                horizontal = "HorizontalJ1";
                jumpKey = KeyCode.W;
                break;
            case Team.J2:
                pRend.color = colorJ2;
                jumpKey = KeyCode.UpArrow;
                horizontal = "HorizontalJ2";
                break;
            default:
                return;
        }
    }

    bool jumping;

    private void Update()
    {
        if (groundCheck && Input.GetKeyDown(jumpKey))
            jumping = true;
    }

    private void FixedUpdate()
    {
        Move();
        if (jumping)
        {
            rb.gravityScale = ogGravity;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode2D.Impulse);
            jumping = false;
        }
        else if(!groundCheck)
        {
            rb.gravityScale += Time.deltaTime * gravityStrength;
        }
    }

    private void Move()
    {
        float horizontalAxis = Input.GetAxis(horizontal);
        /*        float acc;
                float acceleration = dir == Vector2.zero  ? acc = accX : acc = decX;
                Vector2 move = Vector2.Lerp(Vector2.zero, dir * moveSpeed * Time.deltaTime, acc);
                rb.velocity = move;*/
        rb.velocity = new Vector2(horizontalAxis * moveSpeed * Time.deltaTime * 100, rb.velocity.y) ;
       
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        groundCheck = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > normalYmaxInclinasion) 
            {
                groundCheck = true;
            }
        }
        
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        groundCheck = false;
    }
}
