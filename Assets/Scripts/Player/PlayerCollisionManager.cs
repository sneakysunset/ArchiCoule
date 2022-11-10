using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager : MonoBehaviour
{
    private CharacterController2D charC;
    private Rigidbody rb;
    [Range(-1f, 1f)] public float yNormalLineCollision;
    public float normalYmaxInclinasion;
    Vector3 prevVelocity;
    [HideInInspector] public IEnumerator groundCheckEnum;

    private void Start()
    {
        charC = GetComponent<CharacterController2D>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        StartCoroutine(WaitForPhysics());
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
    }

    private void OnCollisionExit(Collision collision)
    {
        groundCheckEnum = waitForGroundCheckOff(charC.ghostInputTimer);
        StartCoroutine(groundCheckEnum);
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
