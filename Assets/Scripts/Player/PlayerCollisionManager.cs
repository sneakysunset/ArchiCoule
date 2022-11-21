using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    [HideInInspector] public CharacterController2D charC;
    private Rigidbody2D rb;
    [HideInInspector] public GameObject coll;
    Vector2 prevVelocity;
    [HideInInspector] public IEnumerator groundCheckEnum;
    [HideInInspector] public List<Transform> holdableObjects;
    [HideInInspector] public bool holdingBall = false;

    [Range(-1f, 1f)] public float yNormalLineCollision = -.15f;
    [Range(-1f, 1f)] public float normalYmaxInclinasion = .7f;

    private void Start()
    {
        charC = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody2D>();
        coll = transform.Find("Collider").gameObject;
    }

    private void FixedUpdate()
    {
        StartCoroutine(WaitForPhysics());
    }

    bool enterAgain;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        LineCollisionEnter(collision);
        GroundCheckCollisionEnter(collision);
    }

    private void LineCollisionEnter(Collision2D collision)
    {
        bool condition1 = collision.gameObject.tag == "LineCollider";
        bool condition2 = collision.contacts[0].normal.y < yNormalLineCollision;

        if (condition1 && condition2)
        {
            coll.layer = LayerMask.NameToLayer("PlayerOff");
            rb.velocity = prevVelocity;
        }
        else if (condition1 && !condition2 && enterAgain)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/Landcorde");
            enterAgain = false;
        }
        else if (!condition1)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Land");
        }
    }

    private void GroundCheckCollisionEnter(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > normalYmaxInclinasion)
        {
            charC.groundCheck = true;
            if (groundCheckEnum != null)
            {
                StopCoroutine(groundCheckEnum);
                groundCheckEnum = null;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        GroundCheckCollisionStay(collision);
        WallJumpCollisionStay(collision);
    }

    void GroundCheckCollisionStay(Collision2D collision)
    {
        charC.groundCheck = false;
        if (collision.contacts[0].normal.y > normalYmaxInclinasion)
        {
            charC.groundCheck = true;
            if (groundCheckEnum != null)
            {
                StopCoroutine(groundCheckEnum);
                groundCheckEnum = null;
            }
        }
    }

    void WallJumpCollisionStay(Collision2D collision)
    {
        if (collision.contacts[0].normal.y > -0.3 && collision.contacts[0].normal.y < 0.3 && collision.gameObject.CompareTag("Jumpable") || collision.gameObject.CompareTag("LineCollider"))
        {
            charC.wallJumpable = collision.contacts[0].normal.x;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        groundCheckEnum = waitForGroundCheckOff(charC.ghostInputTimer);
        StartCoroutine(groundCheckEnum);
        StartCoroutine(WaitForSeconds(.2f));

        charC.wallJumpable = 0;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GetBallOnTriggerEnter(other);
    }

    void GetBallOnTriggerEnter(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(true);
            holdableObjects.Add(other.transform.parent);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.tag == "LineCollider")
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerOff");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        RemoveBallTriggerExit(other);
        ExitLineTriggerExit(other);
    }

    void RemoveBallTriggerExit(Collider2D other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(false);
            holdableObjects.Remove(other.transform.parent);
        }
    }
    
    void ExitLineTriggerExit(Collider2D other)
    {
        bool condition1 = other.tag == "LineCollider";
        if (condition1)
        {
            coll.layer = LayerMask.NameToLayer("Player");
            if (holdingBall)
            {
                coll.layer = LayerMask.NameToLayer("PlayerOff");
            }
        }
    } 
    
    IEnumerator WaitForSeconds(float timer)
    {
        yield return new WaitForSeconds(timer);
        enterAgain = true;
    }

    public IEnumerator waitForGroundCheckOff(float timer)
    {
        yield return new WaitForSeconds(timer);
        charC.groundCheck = false;
    }

    public IEnumerator WaitForPhysics()
    {
        yield return new WaitForFixedUpdate();
        prevVelocity = rb.velocity;
    }

}
