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
    private void Start()
    {
        edgeCollider = lineR.gameObject.GetComponentInChildren<EdgeCollider2D>();
        lineR.SetPosition(0, transform.position);
    }
    private void FixedUpdate()
    {
        bool condition1;
        bool condition2;
        if (!player1)
        {
            condition1 = Mathf.Abs(transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x) > distanceToAddPoint;
            condition2 = transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x < 0;
        }
        else
        {
            condition1 = Mathf.Abs(transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x) > distanceToAddPoint;
            condition2 = transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x > 0;
            //print((transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x) + " " +  condition2);
            
        }
        if (condition1 && !condition2)
        {
            for (float i = 0; i < Mathf.Abs(transform.position.x - lineR.GetPosition(lineR.positionCount - 1).x); i += distanceToAddPoint)
            {
                lineR.positionCount++;
                lineR.SetPosition(lineR.positionCount - 1, transform.position);

            }
        }
        else if (condition2)
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
            lineR.SetPosition(pointIndex, new Vector2(lineR.GetPosition(pointIndex).x, transform.position.y));
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
}
