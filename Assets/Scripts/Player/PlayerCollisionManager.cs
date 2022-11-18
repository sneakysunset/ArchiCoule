using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    [HideInInspector] public CharacterController2D charC;
    private Rigidbody rb;
    [HideInInspector] public GameObject coll;
    Vector3 prevVelocity;
    [HideInInspector] public IEnumerator groundCheckEnum;
    [HideInInspector] public List<Transform> holdableObjects;
    [HideInInspector] public bool holdingBall = false;

    [Range(-1f, 1f)] public float yNormalLineCollision = -.15f;
    [Range(-1f, 1f)] public float normalYmaxInclinasion = .7f;

    private void Start()
    {
        charC = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody>();
        coll = transform.Find("Collider").gameObject;
    }

    private void FixedUpdate()
    {
        StartCoroutine(WaitForPhysics());
    }

    bool enterAgain;

    private void OnCollisionEnter(Collision collision)
    {
        LineCollisionEnter(collision);
        GroundCheckCollisionEnter(collision);
    }

    private void LineCollisionEnter(Collision collision)
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

    private void GroundCheckCollisionEnter(Collision collision)
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

    private void OnCollisionStay(Collision collision)
    {
        GroundCheckCollisionStay(collision);
        WallJumpCollisionStay(collision);
    }

    void GroundCheckCollisionStay(Collision collision)
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

    void WallJumpCollisionStay(Collision collision)
    {
        if (collision.contacts[0].normal.y > -0.3 && collision.contacts[0].normal.y < 0.3 && collision.gameObject.CompareTag("Jumpable") || collision.gameObject.CompareTag("LineCollider"))
        {
            charC.wallJumpable = collision.contacts[0].normal.x;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        groundCheckEnum = waitForGroundCheckOff(charC.ghostInputTimer);
        StartCoroutine(groundCheckEnum);
        StartCoroutine(WaitForSeconds(.2f));

        charC.wallJumpable = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetBallOnTriggerEnter(other);
    }

    void GetBallOnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(true);
            holdableObjects.Add(other.transform.parent);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "LineCollider")
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerOff");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        RemoveBallTriggerExit(other);
        ExitLineTriggerExit(other);
    }

    void RemoveBallTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(false);
            holdableObjects.Remove(other.transform.parent);
        }
    }
    
    void ExitLineTriggerExit(Collider other)
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
