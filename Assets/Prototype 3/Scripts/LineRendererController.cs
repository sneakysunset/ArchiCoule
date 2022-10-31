using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LineRendererController : MonoBehaviour
{
    public LineRenderer lineR;
    EdgeCollider2D edgeCollider;
    public float distanceToAddPoint = .5f;
    public bool player1;
    Vector2 scaleOffset;
    private void Start()
    {

        edgeCollider = lineR.gameObject.GetComponentInChildren<EdgeCollider2D>();
        scaleOffset = new Vector2(0, transform.localScale.y / 2 + edgeCollider.edgeRadius + .1f);
        
        lineR.SetPosition(0, (Vector2)transform.position - scaleOffset);
    }
    private void FixedUpdate()
    {
        bool condition1 = true ;
        bool condition2;

        for (int i = 0; i < lineR.positionCount; i++)
        {
            if(Mathf.Abs(transform.position.x - lineR.GetPosition(i).x) < distanceToAddPoint)
            {
                condition1 = false;
                break;
            }
        }

        if (player1) condition2 = transform.position.x < lineR.GetPosition(0).x;       
        else condition2 = transform.position.x > lineR.GetPosition(0).x;

        if (condition1)
        {
            for (float i = 0; i < Mathf.Abs(transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x); i += distanceToAddPoint)
            {
                lineR.positionCount++;
                lineR.SetPosition(lineR.positionCount - 1, (Vector2)transform.position - scaleOffset);
                if(condition2)
                {
                    var temp = lineR.GetPosition(0);
                    var temp2 = Vector2.zero;
                    for (int j = 0; j < lineR.positionCount - 1; j++)
                    {
                        temp2 = lineR.GetPosition(j + 1);
                        lineR.SetPosition(j + 1, temp);
                        temp = temp2;
                    }
                    lineR.SetPosition(0, (Vector2)transform.position - scaleOffset);
                    break;
                }
                else
                {
                    lineR.SetPosition(lineR.positionCount - 1, (Vector2)transform.position - scaleOffset);
                }
            }
        }
        else
        {
            float distance = 1000;
            int pointIndex = 0;
            for (int i = 0; i < lineR.positionCount; i++)
            {
                if (Mathf.Abs(lineR.GetPosition(i).x - transform.position.x) < distance)
                {
                    distance = Mathf.Abs(lineR.GetPosition(i).x - transform.position.x);
                    pointIndex = i;
                }
            }
            lineR.SetPosition(pointIndex, new Vector2(lineR.GetPosition(pointIndex).x, transform.position.y - (transform.localScale.y/2 + edgeCollider.edgeRadius + .1f)));
        }
    }



    private void Update()
    {

        Vector2[] tempPoints = new Vector2[lineR.positionCount];
        for (int i = 0; i < lineR.positionCount; i++)
        {
            tempPoints[i] = lineR.GetPosition(i);
        }
        edgeCollider.points = tempPoints;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "EdgeCollider")
        {
            if(collision.contacts[0].normal.y < -0.6f )
            {
                collision.collider.isTrigger = true;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "EdgeCollider")
        { 
                collision.isTrigger = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Respawn")
        {
            transform.position = (Vector2)transform.position + new Vector2(0, 14f);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }
}
