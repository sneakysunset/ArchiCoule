using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBumper : MonoBehaviour
{
    public float bumperStrength;
    public enum bumperTypes { DirectionalBumper, NormalBumper };
    public bumperTypes bumperType;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
        print(collision.gameObject.tag);
            if (bumperType == bumperTypes.DirectionalBumper) collision.rigidbody.AddForce(transform.up * bumperStrength, ForceMode.Impulse);
            else if (bumperType == bumperTypes.NormalBumper) collision.rigidbody.AddForce(-collision.contacts[0].normal * bumperStrength, ForceMode.Impulse);
        }
    }
}
