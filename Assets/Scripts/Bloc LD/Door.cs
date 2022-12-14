using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum doorTypes { KeyDoor, ButtonDoor};
    public KeyScript[] keys;
    public Vector2 doorDestination;
    public float doorOpenSpeed = 1;
    public float doorCloseSpeed = 1;
    public AnimationCurve openCurve;
    public AnimationCurve closeCurve;
   public int nOpen;
    private Vector2 ogPos;
    private IEnumerator doorOpening, doorClosing;

    private void Start()
    {
        foreach(KeyScript key in keys)
        {
            key.door = this;
        }
        ogPos = transform.position;
        doorOpenSpeed = Mathf.Clamp(doorOpenSpeed, 0.01f, 100);
        doorCloseSpeed = Mathf.Clamp(doorCloseSpeed, 0.01f, 100);
    }

    public void KeyTriggered()
    {

        nOpen = 0;
        foreach (KeyScript key in keys)
        {
            if (key.activated == true) nOpen++;
        }
        if (nOpen == keys.Length)
        {
            if (doorClosing != null)
            {
                StopCoroutine(doorClosing);
                doorClosing = null;
            }
            if (doorOpening == null && (Vector2)transform.position != doorDestination)
            {
                print(2);
                doorOpening = DoorOpen(transform.position);
                StartCoroutine(doorOpening);
            }
        }
        else
        {


            if (doorOpening != null)
            {
                StopCoroutine(doorOpening);
                doorOpening = null;
            }
            if (doorClosing == null && (Vector2)transform.position != ogPos)
            {
                doorClosing = DoorClose(transform.position);
                StartCoroutine(doorClosing);
            }
        }
    }


    IEnumerator DoorOpen(Vector2 startPos)
    {
        float i = Vector2.Distance(ogPos, transform.position) / Vector2.Distance(ogPos, doorDestination) ;
        print(i);
        while(i < 1)
        {
            i += Time.deltaTime * doorOpenSpeed;
            transform.position = Vector2.Lerp(startPos, doorDestination, openCurve.Evaluate(i));
            yield return new WaitForEndOfFrame();
        }
        transform.position = doorDestination;
        yield return null;
    }

    IEnumerator DoorClose(Vector2 startPos)
    {
        float i = Vector2.Distance(ogPos, transform.position) / Vector2.Distance(ogPos, doorDestination);
        while (i < 1)
        {
            i += Time.deltaTime * doorOpenSpeed;
            transform.position = Vector2.Lerp(startPos, ogPos, openCurve.Evaluate(i));
            yield return new WaitForEndOfFrame();
        }
        transform.position = ogPos;
        yield return null;
    }
}

