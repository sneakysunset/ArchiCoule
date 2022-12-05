using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private LineCreator lineC;
    private void Awake()
    {
        lineC = GetComponent<LineCreator>();
    }

    private void FixedUpdate()
    {
        lineC.LineUpdater();
    }

}
