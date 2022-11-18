using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    [HideInInspector] public CharacterController2D charC;
    private Rigidbody rb;
    [Range(-1f, 1f)] public float yNormalLineCollision;
    public float normalYmaxInclinasion;
    Vector3 prevVelocity;
    [HideInInspector] public IEnumerator groundCheckEnum;
    public List<Transform> holdableObjects;
    [HideInInspector] public bool holdingBall = false;
    public float moveX;
    public bool inLine;
    private void Start()
    {
        charC = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody>();
        //print(LayerMask.NameToLayer("Collider" + charC.playerType.ToString()));
    }

    private void FixedUpdate()
    {
        StartCoroutine(WaitForPhysics());
        moveX = charC.moveValue.x;
    }

    bool enterAgain;

    private void OnCollisionEnter(Collision collision)
    {
        if(holdingBall && collision.gameObject.tag == "LineCollider")
        {
            //collision.gameObject.layer = LayerMask.NameToLayer("NoCollisionPlayer");
            collision.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 1);
            if (charC.playerType == CharacterController2D.Team.J2)
            {
                collision.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 2);
            }
        }

        if (collision.gameObject.tag == "LineCollider" && collision.contacts[0].normal.y < yNormalLineCollision && (collision.gameObject != charC.meshObj || /*holdingBall ||*/ collision.gameObject.layer == 12 ))
        {
            collision.gameObject.layer = LayerMask.NameToLayer("NoCollisionPlayer");
            rb.velocity = prevVelocity;
        }
        else if (collision.gameObject.tag == "LineCollider" && collision.contacts[0].normal.y >= yNormalLineCollision && (collision.gameObject != charC.meshObj/* || holdingBall*/ || collision.gameObject.layer == 12) && enterAgain)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCorde/Landcorde");
            enterAgain = false;
        }
        else if(collision.gameObject.tag != "LineCollider")
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/MouvementCharacter/Land");
        }

        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > normalYmaxInclinasion)
            {
                charC.groundCheck = true;
                if (groundCheckEnum != null)
                {
                    StopCoroutine(groundCheckEnum);
                    groundCheckEnum = null;
                }
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        charC.groundCheck = false;
        for (int i = 0; i < collision.contacts.Length; i++)
        {
            if (collision.contacts[i].normal.y > normalYmaxInclinasion)
            {
                charC.groundCheck = true;
                if (groundCheckEnum != null)
                {
                    StopCoroutine(groundCheckEnum);
                    groundCheckEnum = null;
                }
            }

        }
        if (collision.contacts[0].normal.y > -0.1 && collision.contacts[0].normal.y < 0.1 && collision.gameObject.CompareTag("Jumpable"))
        {
            charC.jumpable = collision.contacts[0].normal.x;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        groundCheckEnum = waitForGroundCheckOff(charC.ghostInputTimer);
        StartCoroutine(groundCheckEnum);
        StartCoroutine(WaitForSeconds(.2f));

        charC.jumpable = 0;
    }

    private void OnTriggerEnter(Collider other)
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
            inLine = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Ball"))
        {
            other.transform.parent.Find("Highlight").gameObject.SetActive(false);
            holdableObjects.Remove(other.transform.parent);
        }

        if (other.tag == "LineCollider") inLine = false;

        if (other.tag == "LineCollider" && other.gameObject.layer != LayerMask.NameToLayer("Collider" + charC.playerType.ToString()) && other.gameObject != charC.meshObj)
        {
            other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 1);
            if (charC.playerType == CharacterController2D.Team.J1)
            {
                other.gameObject.layer = LayerMask.NameToLayer("ColliderJ" + 2);
            }
            else if(other.name == "Mesh Ball Off")
            {
                other.gameObject.layer = 12;
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
