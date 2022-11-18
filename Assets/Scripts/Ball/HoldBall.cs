using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class HoldBall : MonoBehaviour
{
    [HideInInspector] public Transform bB;
    private Rigidbody bRb;
    private Collider bCol;
    private ConstantMeshGeneration cMG;
    private PlayerCollisionManager playerCollisionM;
    
    public Transform holdPoint;
    public float ThrowStrength = 5;

    private void Awake()
    {
        playerCollisionM = GetComponent<PlayerCollisionManager>();
        bB = null;
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if(bB != null && context.started /*&& playerCollisionM.holdableObjects.Count == 0*/)
        {
            bRb.isKinematic = false;
            bCol.isTrigger = false;

            if (bB != null)
            {
                bB.tag = "Ball";
                bB.GetComponentInChildren<Collider>().tag = "Ball";
            }
            playerCollisionM.holdableObjects.Add(bB);
            cMG.meshF.name = "Mesh Ball Free";
            bB = null;
            bRb = null;
            cMG = null;
            playerCollisionM.coll.layer = LayerMask.NameToLayer("PlayerOff");
        }
        else if (playerCollisionM.holdableObjects.Count > 0 && context.started)
        {
            bB = closestItemFinder(playerCollisionM.holdableObjects);
            bCol = bB.GetComponentInChildren<Collider>();
            cMG = bB.GetComponent<ConstantMeshGeneration>();
            cMG.meshF.name = "Mesh Ball Held";
            bCol.isTrigger = true;
            bB.tag = "Held";
            bB.GetComponentInChildren<Collider>().tag = "Held";
            bRb = bB.GetComponent<Rigidbody>();
            bRb.isKinematic = true;
            playerCollisionM.holdableObjects.Remove(bB);
        }
    }

    private Transform closestItemFinder(List<Transform> items)
    {
        Transform closestItem = items[0];
        foreach(Transform item in items)
        {
            if(Vector2.Distance(item.position, transform.position) < Vector2.Distance(closestItem.position, transform.position))
            {
                closestItem = item;
            }
        }
        return closestItem;
    }


    private void Update()
    {
        if(bB != null && bRb?.isKinematic == true)
        {
            bB.transform.position = holdPoint.position;
            playerCollisionM.holdingBall = true;
            playerCollisionM.coll.layer = LayerMask.NameToLayer("PlayerOff");
        }
        else
        {
            playerCollisionM.holdingBall = false;

        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if(bB != null && context.performed)
        {
            bRb.isKinematic = false;
            bCol.isTrigger = false;
            bB.tag = "Ball";
            //if (playerCollisionM.inLine) cMG.meshF.gameObject.layer = LayerMask.NameToLayer("Collider" + playerCollisionM.charC.playerType.ToString());
            playerCollisionM.holdableObjects.Add(bB);
            //bB.GetComponent<ConstantMeshGeneration>().meshF.gameObject.layer = 12;
            bB = null;
            bRb.AddForce(GetComponent<CharacterController2D>().moveValue * ThrowStrength);


            bRb = null;
        }
        else if(bB != null)
        {
            //Preview de la simulation de la courbe de lancé
        }
    }

    public void OnDrop(InputAction.CallbackContext context)
    {
        if (bB != null)
        {
            bRb.isKinematic = false;
            bCol.isTrigger = false;
            bB.tag = "Ball";
            bB = null;
            bRb = null;
        }
    }
}
