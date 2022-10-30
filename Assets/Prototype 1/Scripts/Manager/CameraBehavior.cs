using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public Transform PlayerT;
    private Vector3 velocity = Vector3.zero;
    public float smooth;
    private void LateUpdate()
    {
        //La caméra suit le joueur avec un court délai.
        transform.position = Vector3.SmoothDamp(transform.position, PlayerT.position, ref velocity, smooth);
    }
}
