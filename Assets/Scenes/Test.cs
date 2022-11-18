using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Test : MonoBehaviour
{
    public Vector2 p1, p2, p;
    public float distance;
    

    private void OnDrawGizmos()
    {
        var targetPos = p + ((p - p1).normalized + (p - p2).normalized).normalized * distance ;
        if (targetPos.y < p.y)
        {
            targetPos = p - ((p - p1).normalized + (p - p2).normalized).normalized * distance;
        }
        print(Vector2.Distance(p, targetPos));
        Debug.DrawLine(p, targetPos, Color.blue, .1f);
        Debug.DrawLine(p, p1, Color.red, .1f);
        Debug.DrawLine(p, p2, Color.red, .1f);

/*        Debug.DrawLine(targetPos, p1 + targetPos, Color.yellow, .1f);
        Debug.DrawLine(targetPos, p2 + targetPos, Color.yellow, .1f);*/
    }


}
