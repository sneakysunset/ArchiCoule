using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    public enum Team { J1, J2 };
    public Team playerType;
    public Color colorJ1, colorJ2;
    [HideInInspector] public Color col;
    private Rigidbody rb;
    public float jumpStrength;
    public float normalYmaxInclinasion;
    public bool groundCheck = false;
    public float moveSpeed;
    //public float accX, decX;
    public float gravityStrength;
    private float ogGravity;
    public string horizontal;
    public KeyCode jumpKey;
    [Range(-1f, 1f)] public float yNormalLineCollision;
    private Vector3 prevVelocity;
    public float ghostInputTimer;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        MeshRenderer pRend = GetComponentInChildren<MeshRenderer>();
        ogGravity = rb.mass;
        switch (playerType)
        {
            case Team.J1:
                pRend.material.color = colorJ1;
                col = colorJ1;
                horizontal = "HorizontalJ1";
                break;
            case Team.J2:
                pRend.material.color = colorJ2;
                col = colorJ2;
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
            rb.mass = ogGravity;
            rb.AddForce(Vector3.up * jumpStrength, ForceMode.Impulse);

            jumping = false;
            groundCheck = false;
            if (groundCheckEnum != null)
            {
                StopCoroutine(groundCheckEnum);
                groundCheckEnum = null;
            }
        }
        else if(!groundCheck)
        {
            rb.mass += Time.deltaTime * gravityStrength;
        }
        StartCoroutine(WaitForPhysics());
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

    private void OnCollisionStay(Collision collision)
    {
/*        groundCheck = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > normalYmaxInclinasion) 
            {  
                groundCheck = true;
            }
        }*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "LineCollider" && collision.contacts[0].normal.y < yNormalLineCollision)
        {
            collision.gameObject.layer = LayerMask.NameToLayer("NoCollisionPlayer");
            rb.velocity = prevVelocity;
        }

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > normalYmaxInclinasion)
            {
                groundCheck = true;
                if (groundCheckEnum != null)
                {
                    StopCoroutine(groundCheckEnum);
                    groundCheckEnum = null;
                }
            }
        }
    }
    IEnumerator groundCheckEnum;
    private void OnCollisionExit(Collision collision)
    {
        groundCheckEnum = waitForGroundCheckOff(ghostInputTimer);
        StartCoroutine(groundCheckEnum);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "LineCollider" && other.gameObject.layer != LayerMask.NameToLayer("Collider" + playerType.ToString()))
        {
            other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 1);
            if(playerType == Team.J1) other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 2);
        }
    }

    IEnumerator waitForGroundCheckOff(float timer)
    {
        yield return new WaitForSeconds(timer);
        groundCheck = false;
    }

    IEnumerator WaitForPhysics()
    {
        yield return new WaitForFixedUpdate();
        prevVelocity = rb.velocity;

    }
}
