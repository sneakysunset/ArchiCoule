using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerExitTest : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        print(1);
    }
}
