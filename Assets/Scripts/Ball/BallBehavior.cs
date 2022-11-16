using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBehavior : MonoBehaviour
{
    private ConstantMeshGeneration meshG;
    Collider coll;
    private void Awake()
    {
        meshG = GetComponent<ConstantMeshGeneration>();
        coll = GetComponentInChildren<Collider>();
    }

    private void FixedUpdate()
    {

        meshG.MeshCreator();
    }

}
