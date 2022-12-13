using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : MonoBehaviour
{
    public bool activated;
    [HideInInspector] public Door door;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = true;
            door.KeyTriggered();
            GetComponent<SpriteRenderer>().color = Color.blue;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("LineCollider"))
        {
            activated = false;
            door.KeyTriggered();
            GetComponent<SpriteRenderer>().color = Color.white;

        }
    }
}
