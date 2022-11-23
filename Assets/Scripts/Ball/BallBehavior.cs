using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private LineCreator lineC;
    Collider2D coll;
    private void Awake()
    {
        lineC = GetComponent<LineCreator>();
        coll = GetComponentInChildren<Collider2D>();
    }

    private void FixedUpdate()
    {
        lineC.LineUpdater();
    }

}
